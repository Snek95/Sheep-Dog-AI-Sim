using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using UnityEngine;
using UnityEngine.InputSystem;

public class RLSheepBehaviour : Agent
{
    [HideInInspector]
    public RLSheepController controller;
    [HideInInspector]
    public Rigidbody agentRb;

    [HideInInspector] public int CurrentEpisode = 0;
    [HideInInspector] public float CumulativeReward = 0f;

    public override void Initialize()
    {
        CurrentEpisode = -1;
        CumulativeReward = 0f;
        agentRb = GetComponent<Rigidbody>();
        agentRb.maxAngularVelocity = 500;
    }

    public void MoveAgent(ActionBuffers act)
    {
        
        var rotate = act.ContinuousActions[1];
        var rawMoveSpeed = act.ContinuousActions[0];
        float moveSpeed = rawMoveSpeed + 0.5f;
        transform.Rotate(Vector3.up, controller.sheepRotationSpeed * rotate);
        AddReward(-0.01f * Math.Abs(rotate)); 
        
        agentRb.AddForce(transform.forward * controller.maxSheepMoveSpeed * moveSpeed, ForceMode.VelocityChange);
        if(moveSpeed > 0 && moveSpeed < 0.75)
        {
            //AddReward(0.03f);
        }
        else
        {
            AddReward(-0.01f);
        }
                

    }


    public override void OnEpisodeBegin()
    {
        CurrentEpisode++;
        CumulativeReward = 0f;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        MoveAgent(actions);
        CumulativeReward = GetCumulativeReward();
        //float distanceToAvg = Vector3.Distance(transform.position, controller.avgPosition);
        //AddReward(-distanceToAvg * controller.sheepDistanceToAverageMultiplier);
        //Debug.Log(-distanceToAvg * controller.sheepDistanceToAverageMultiplier);

        //Dog fear
        if (controller.dog != null)
        {
            float distanceToDog = Vector3.Distance(transform.position, controller.dog.transform.position);
            if (distanceToDog < controller.dogFearRadius)
            {
                AddReward(-0.5f - 0.5f * (controller.dogFearRadius - distanceToDog) / controller.dogFearRadius);
            }
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Fance") || collision.gameObject.CompareTag("Obstacle"))
        {
            AddReward(-0.5f);
        } else if (collision.gameObject.CompareTag("Sheep"))
        {
            AddReward(-0.1f);
        }
        
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Fance") || collision.gameObject.CompareTag("Obstacle"))
        {
            AddReward(-0.5f);
        } else if (collision.gameObject.CompareTag("Sheep"))
        {
            AddReward(-0.1f);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = 1f;
        if (Keyboard.current.wKey.isPressed)
        {
            discreteActionsOut[0] = 1; // Move forward
        }
        else if (Keyboard.current.sKey.isPressed)
        {
            discreteActionsOut[0] = 2; // Move backward
        }
        else
        {
            discreteActionsOut[0] = 0; // No movement
        }
        if (Keyboard.current.aKey.isPressed)
        {
            discreteActionsOut[1] = 1; // Rotate left
        }
        else if (Keyboard.current.dKey.isPressed)
        {
            discreteActionsOut[1] = 2; // Rotate right
        }
        else
        {
            discreteActionsOut[1] = 0; // No rotation
        }
    }
}
