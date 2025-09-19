using System;
using Unity.Barracuda;
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
        private Vector3 previousVelocity = Vector3.zero;

    [SerializeField] private LayerMask sheepLayerMask;

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
        if (moveSpeed > 0.2f && moveSpeed < 0.75f)
        {
            AddReward(0.01f);
        }
        else
        {
            AddReward(-0.01f);
        }

        // Penalty for abrupt changes in velocity
        Vector3 currentVelocity = agentRb.linearVelocity;
        float velocityChange = Math.Abs(currentVelocity.magnitude - previousVelocity.magnitude);
        AddReward(-0.05f * velocityChange); // Adjust multiplier as needed
        previousVelocity = currentVelocity;
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
        Collider[] neighbors = Physics.OverlapSphere(transform.position, controller.neighborDist, sheepLayerMask);
        AddReward(0.1f * (neighbors.Length - 1));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Obstacle"))
        {
            AddReward(-0.5f);
        } else if (collision.gameObject.CompareTag("Sheep"))
        {
            AddReward(-0.2f);
        }
        
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Obstacle"))
        {
            AddReward(-0.5f);
        } else if (collision.gameObject.CompareTag("Sheep"))
        {
            AddReward(-0.2f);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        // Move speed: W/S keys
        if (Keyboard.current.wKey.isPressed)
        {
            continuousActionsOut[0] = 1f; // Forward
        }
        else if (Keyboard.current.sKey.isPressed)
        {
            continuousActionsOut[0] = -1f; // Backward
        }
        else
        {
            continuousActionsOut[0] = -0.5f; // No movement
        }

        // Rotation: A/D keys
        if (Keyboard.current.aKey.isPressed)
        {
            continuousActionsOut[1] = -1f; // Rotate left
        }
        else if (Keyboard.current.dKey.isPressed)
        {
            continuousActionsOut[1] = 1f; // Rotate right
        }
        else
        {
            continuousActionsOut[1] = 0f; // No rotation
        }
    }
}
