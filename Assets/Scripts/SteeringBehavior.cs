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
        float radian = target.orientation - agent.orientation;
        return DynamicRotate(radian);
    }
    public float FaceAngular()
    {
       
        if (target.transform.position == agent.transform.position)
        {
            return 0;
        }
        float radian = Mathf.Atan2(target.transform.position.x-agent.transform.position.x,
            target.transform.position.z - agent.transform.position.z)
            - agent.orientation;
       
        return DynamicRotate(radian);
      
    }

    
}
