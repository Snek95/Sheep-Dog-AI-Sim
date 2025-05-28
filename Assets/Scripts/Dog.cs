using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.InputSystem;


public class Dog : Agent
{   
    [SerializeField] private Transform goal;
    [SerializeField] private Transform sheep;
    [SerializeField] private float moveSpeed= 1f;
    [SerializeField] private float roationSpeed =0.001f;

    [HideInInspector] public int CurrentEpisode = 0;
    [HideInInspector] public float CumulativeReward = 0f;
    private float oldDistanceSG;
    private float oldDistanceHS;
    private Rigidbody rb;

    public override void Initialize()
    {
        CurrentEpisode = 0;
        CumulativeReward = 0f;
        oldDistanceSG = Vector3.Distance(sheep.transform.position, goal.transform.position);
        oldDistanceHS = Vector3.Distance(transform.position, sheep.transform.position);
        rb =GetComponent<Rigidbody>();
    }
    public override void OnEpisodeBegin()
    {
        CurrentEpisode++;
        CumulativeReward = 0f;

        SpawnObjects();
    }

    

    public override void CollectObservations(VectorSensor sensor)
    {
        //Position of Environment Objects
        float goalPosX_normalized = goal.localPosition.x;
        float goalPosZ_normalized = goal.localPosition.z;

        float sheepPosX_normalized = sheep.localPosition.x;
        float sheepPosZ_normalized = sheep.localPosition.z;
        float sheepRoation_normalized = (sheep.localRotation.eulerAngles.y / 360) * 2f - 1f;

        float dogPosX_normalized = transform.localPosition.x;
        float dogPosZ_normalized = transform.localPosition.z ;
        float dogRoation_normalized = (transform.localRotation.eulerAngles.y/360) * 2f -1f;

        //Space Size in Behavior Script needs do match to the amount of Observations 
        sensor.AddObservation(goalPosX_normalized);
        sensor.AddObservation(goalPosZ_normalized);
        sensor.AddObservation(dogPosX_normalized);
        sensor.AddObservation(dogPosZ_normalized);
        sensor.AddObservation(dogRoation_normalized);
        sensor.AddObservation(sheepPosX_normalized);
        sensor.AddObservation(sheepPosZ_normalized);
        sensor.AddObservation(sheepRoation_normalized);
    }
    public override void OnActionReceived(ActionBuffers actions) 
    {
        MoveAgent(actions.DiscreteActions);

        AddReward(-2f / MaxStep);

        CumulativeReward = GetCumulativeReward();


        float newDistanceSG = Vector3.Distance(sheep.transform.position, goal.transform.position);
        if (newDistanceSG < oldDistanceSG) AddReward(0.01f);
        if (newDistanceSG > oldDistanceSG) AddReward(-0.001f);
        oldDistanceSG = newDistanceSG;
        
        float newDistanceHS = Vector3.Distance(transform.position, sheep.transform.position);
        if (newDistanceHS < oldDistanceHS) AddReward(0.001f);
        if (newDistanceHS > oldDistanceHS) AddReward(-0.0001f);
        oldDistanceHS = newDistanceHS;
    }

    private void SpawnObjects()
    {
        transform.localRotation = Quaternion.identity;
        transform.localPosition = new Vector3 (0, 0.3f, 0);

        sheep.localPosition = new Vector3(1,0.3f,1);
        sheep.localRotation = Quaternion.identity;
        sheep.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        sheep.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;


        float randomAngle = Random.Range(0f, 360f);
        Vector3 randomDirection = Quaternion.Euler(0f, randomAngle, 0f) * Vector3.forward;
        rb.linearDamping = 15f;

        float randomDistance = Random.Range(1f, 2.5f);

        Vector3 sheepPosition = transform.localPosition + randomDirection * randomDistance;
        sheep.localPosition = sheepPosition;


    }
    private void MoveAgent(ActionSegment<int> act)
    {
        var action = act[0];
        float maxSpeed = 2.5f;
        //set Branch size in behavior parameter to case n
        switch (action)
        {
            case 0:
                AddReward(-0.005f);
                break;

            case 1: //Move forward
                rb.AddForce(transform.forward * moveSpeed, ForceMode.VelocityChange);
                if (rb.linearVelocity.magnitude > maxSpeed)
                {
                    rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
                }
                break;

            case 2: //Rotate left
                transform.Rotate(0f, -roationSpeed * Time.deltaTime, 0f);
                AddReward(-0.005f);
                break;
            case 3: //Rotate right
                transform.Rotate(0f, roationSpeed * Time.deltaTime, 0f);
                AddReward(-0.005f);
                break;

        }
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;

        if (Keyboard.current.dKey.isPressed)
        {
            discreteActionsOut[0] = 3;
        }
        else if (Keyboard.current.wKey.isPressed)
        {
            discreteActionsOut[0] = 1;
        }
        else if (Keyboard.current.aKey.isPressed)
        {
            discreteActionsOut[0] = 2;
        }
    }
    private void Update()
    {
        
    }
    
    public void GoalReached()
    {
        AddReward(5.0f);
        CumulativeReward= GetCumulativeReward();
        EndEpisode();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Fance"))
        {
            AddReward(-0.01f);
        }
        
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Fance"))
        {
            AddReward(-0.01f);
        }
    }
    
}
