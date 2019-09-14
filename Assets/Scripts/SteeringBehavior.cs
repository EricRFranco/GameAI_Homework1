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

    protected float DynamicRotate(float radian)
    {
        float rotationAccerlation = 0;
        //Map to (-pi,pi)
        if (radian > 0)
        {
            while (radian > Mathf.PI)
            {
                radian -= 2 * Mathf.PI;

            }
        }
        else
        {
            while (radian < -Mathf.PI)
            {
                radian += 2 * Mathf.PI;

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
            targetAngularSpeed = maxRotation * Mathf.Abs(radian) / slowRadiusA;
        }
        //Check targetAngularSpeed's direction
        targetAngularSpeed = radian < 0 ? -targetAngularSpeed : targetAngularSpeed;
        // a = v/t
        rotationAccerlation = (targetAngularSpeed - agent.rotation) / timeToTarget;
        //Set rotationAccerlation = maxAngularAccerlation
        if (Mathf.Abs(rotationAccerlation) > maxAngularAcceleration)
        {
            rotationAccerlation = rotationAccerlation > 0 ? maxAngularAcceleration : -maxAngularAcceleration;
        }
        return rotationAccerlation;
    }

    public float AlignAngular()
    {
        //Calculate radian
        float radian = target.orientation - agent.orientation;
        return DynamicRotate(radian);
    }
    public float FaceAngular()
    {
        //Check whether needs to rotate
        if (target.transform.position == agent.transform.position)
        {
            return 0;
        }
        //Calculate radian
        float radian = Mathf.Atan2(target.transform.position.x-agent.transform.position.x,
            target.transform.position.z - agent.transform.position.z)
            - agent.orientation;
        return DynamicRotate(radian);
      
    }
    public float Wander(out Vector3 linear)
    {
        //Helper function that change an orientation into a vector3
        Vector3 AsVector(float orientation)
        {
            return new Vector3(Mathf.Sin(orientation), 0, Mathf.Cos(orientation));
        }
        //Find the circle of wander
        wanderOrientation += (Random.Range(0f, 1f) - Random.Range(0f, 1f)) * wanderRate;
        float targetOrientation = wanderOrientation + agent.orientation;
        
        Vector3 circleCenter = agent.position + wanderOffset * AsVector(agent.orientation);
        agent.DrawCircle(circleCenter, wanderRadius);
        //Find targetPosition
        Vector3 targetPosition = circleCenter + wanderRadius * AsVector(targetOrientation);
           
        //Same as Face
        if (targetPosition == agent.transform.position)
        {
            linear = new Vector3(0, 0, 0);
            return 0;
        }
        float radian = Mathf.Atan2(targetPosition.x - agent.transform.position.x,
            targetPosition.z - agent.transform.position.z)
            - agent.orientation;
        float rotationAccerlation = DynamicRotate(radian);
        //Calculate linear
        linear = maxAcceleration * AsVector(agent.orientation);
        return rotationAccerlation;

    }
   
}
