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

    protected float DynamicRotate(float radian) //Tony's Work
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
        float targetAngularSpeed = 0;
        //Don't need rotation

        if (Mathf.Abs(radian) < targetRadiusA)
        {
            targetAngularSpeed = 0;
        }

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

    public float AlignAngular() //Tony's Work
    {
        //Calculate radian
        float radian = target.orientation - agent.orientation;
        return DynamicRotate(radian);
    }
    public float FaceAngular() //Tony's Work
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
    public float Wander(out Vector3 linear) //Tony's Work
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

    public Vector3 Seek() //Eric's Work
    {
        Vector3 linearAcc = target.position - agent.position;
        linearAcc.Normalize();
        linearAcc *= maxAcceleration;
        return linearAcc;
    }

    public Vector3 Flee() //Eric's Work
    {
        Vector3 linearAcc = agent.position - target.position;
        linearAcc.Normalize();
        linearAcc *= maxAcceleration;
        return linearAcc;
    }

    public Vector3 Pursue() //Eric's Work
    {
        // Calculate direction and distance away from target
        Vector3 direction = target.position - agent.position;
        float distance = direction.magnitude;

        // Speed required to judge prediction threshold
        float speed = agent.velocity.magnitude;

        // Set prediction threshold based on distance away from target
        float prediction = (speed <= distance / maxPrediction) ? maxPrediction : distance / speed;
        // Update target position to be slightly ahead of its current path, then call Seek() on this new target position
        agent.DrawCircle(target.position + target.velocity * prediction, targetRadiusL);
        target.position += target.velocity * prediction;

        return Arrive();
    }

    public Vector3 Arrive() //Eric's Work
    {
        Vector3 direction = target.position - agent.position;
        float distance = direction.magnitude;
        float targetSpeed = 0;
        if (distance < targetRadiusL)
        {
            targetSpeed = 0;
        }
        else
        {
            targetSpeed = (distance > slowRadiusL) ? maxSpeed : maxSpeed * distance / slowRadiusA;
        }
 

        Vector3 targetVelocity = direction.normalized * targetSpeed;
        Vector3 linearAcc = (targetVelocity - agent.velocity) / timeToTarget;

        if (linearAcc.magnitude > maxAcceleration)
        {
            linearAcc.Normalize();
            linearAcc *= maxAcceleration;
        }

        return linearAcc;
    }
     
    public Vector3 Evade() //Eric's Work
    {
        // Calculate direction and distance away from target
        Vector3 direction = agent.position - target.position;
        float distance = direction.magnitude;

        // Speed required to judge prediction threshold
        float speed = agent.velocity.magnitude;

        // Set prediction threshold based on distance away from target
        float prediction = (speed <= distance / maxPrediction) ? maxPrediction : distance / speed;
        // Update target position to be slightly ahead of its current path, then call Seek() on this new target position
        target.position += target.velocity * prediction;

        return Flee();
    }
}
