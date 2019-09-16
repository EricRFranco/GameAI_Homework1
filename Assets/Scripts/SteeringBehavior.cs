using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the place to put all of the various steering behavior methods we're going
/// to be using. Probably best to put them all here, not in NPCController.
/// </summary>

public class SteeringBehavior : MonoBehaviour {

    // The agent at hand here, and whatever target it is dealing with
    public NPCController agent;
    public NPCController target;

    // Below are a bunch of variable declarations that will be used for the next few
    // assignments. Only a few of them are needed for the first assignment.

    // For pursue and evade functions
    public float maxPrediction;
    public float maxAcceleration;

    // For arrive function
    public float maxSpeed;
    public float targetRadiusL;
    public float slowRadiusL;
    public float timeToTarget;

    // For Face function
    public float maxRotation;
    public float maxAngularAcceleration;
    public float targetRadiusA;
    public float slowRadiusA;

    // For wander function
    public float wanderOffset;
    public float wanderRadius;
    public float wanderRate;
    private float wanderOrientation;

    // Holds the path to follow
    public GameObject[] Path;
    public int current = 0;

    protected void Start() {
        agent = GetComponent<NPCController>();
        //wanderOrientation = agent.orientation;
    }
    public Vector3 Align_Linear()
    {
        return new Vector3(0,0,0);
    }
    public float Align_Angular()
    {
        
        float rotationAccerlation = 0;
        float radian = target.orientation - agent.orientation;
       
        //Map to (-pi,pi)
        if (radian > 0)
        {
            while (radian > Mathf.PI)
            {
                radian -= 2*Mathf.PI;
                
            }
        }
        else
        {
            while(radian < -Mathf.PI)
            {
                radian += 2*Mathf.PI;
                
            }
        }
      
        //Don't need rotation
        if (Mathf.Abs(radian) < targetRadiusA)
        {
            agent.rotation = 0;
        }
        float targetAngularSpeed = 0;
        
        if (Mathf.Abs(radian) > slowRadiusA)
        {
            targetAngularSpeed = maxRotation;
        }
        else
        {
            targetAngularSpeed = maxRotation* Mathf.Abs(radian) / slowRadiusA;
        }
        //Check targetAngularSpeed's direction
        if (radian < 0)
        {
            targetAngularSpeed *= -1;
        }
        // a = v/t
        rotationAccerlation = (targetAngularSpeed - agent.rotation) / timeToTarget;
        
        //Set rotationAccerlation = maxAngularAccerlation
        if (Mathf.Abs(rotationAccerlation) > maxAngularAcceleration)
        {
            rotationAccerlation = rotationAccerlation > 0 ? maxAngularAcceleration : -maxAngularAcceleration;
        }
        print(rotationAccerlation);
        return rotationAccerlation;
      

    }
    public float Face_Angular()
    {

        return 1;
    }

    public Vector3 Seek()
    {
        Vector3 linearAcc = target.position - agent.position;
        linearAcc.Normalize();
        linearAcc *= maxAcceleration;
        //float angular = 0f;
        return linearAcc;
    }

    public Vector3 Flee()
    {
        Vector3 linearAcc = agent.position - target.position;
        linearAcc.Normalize();
        linearAcc *= maxAcceleration;
        return linearAcc;
    }

    public Vector3 Pursue()
    {
        Vector3 direction = target.position - agent.position;
        float distance = direction.magnitude;

        float speed = agent.velocity.magnitude;

        float prediction = (speed <= distance / maxPrediction) ? maxPrediction : distance / speed;
        target.position += target.velocity * prediction;

        return Seek();
    }

    public Vector3 Evade()
    {
        Vector3 direction = agent.position - target.position;
        float distance = direction.magnitude;

        float speed = agent.velocity.magnitude;

        float prediction = (speed <= distance / maxPrediction) ? maxPrediction : distance / speed;
        target.position += target.velocity * prediction;

        return Flee();
    }
}
