using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.InputSystem;
using System.Collections.Generic;


public class Dog : Agent
{   
    [SerializeField] private Transform goal;
    [SerializeField] private float moveSpeed= 1f;
    [SerializeField] private float roationSpeed =0.001f;
    [SerializeField] private int sheepCount =4;
    [SerializeField] private GameObject sheepPrefab;
    [SerializeField] private SheepController sheepController;
    public float minSheepDistance = 1f;
    public float maxSheepDistance = 2.5f;
    public float timeScale = 1f;
    public List<GameObject> obstacles;
    public int obstacleAmount;

    [HideInInspector] public int CurrentEpisode = 0;
    [HideInInspector] public float CumulativeReward = 0f;
    private float oldDistanceSG;
    private float oldDistanceHS;
    private Rigidbody rb;

    Transform firstSheep;

    public override void Initialize()
    {
        CurrentEpisode = 0;
        CumulativeReward = 0f;
        rb =GetComponent<Rigidbody>();
        Time.timeScale = timeScale;
        
        
    }
    public override void OnEpisodeBegin()
    {
        CurrentEpisode++;
        CumulativeReward = 0f;

        SpawnObjects();
    }
    
    public override void OnActionReceived(ActionBuffers actions) 
    {
        MoveAgent(actions.DiscreteActions);

        AddReward(-2f / MaxStep);

        CumulativeReward = GetCumulativeReward();

        /**
        if (firstSheep != null)
        {
            float newDistanceSG = Vector3.Distance(firstSheep.position, goal.transform.position);
            if (newDistanceSG < oldDistanceSG) AddReward(0.01f);
            if (newDistanceSG > oldDistanceSG) AddReward(-0.01f);
            oldDistanceSG = newDistanceSG;

            float newDistanceHS = Vector3.Distance(transform.position, firstSheep.position);
            if (newDistanceHS < oldDistanceHS) AddReward(0.00001f);
            if (newDistanceHS > oldDistanceHS) AddReward(-0.00001f);
            oldDistanceHS = newDistanceHS;
        }
        **/
        
        
    }

    private void SpawnObjects()
    {
        // Hund zurücksetzen
        transform.localRotation = Quaternion.identity;
        transform.localPosition = new Vector3(0f, 0.3f, 0f);

        // Zufällige Position für das Goal
        float goalZ = Random.Range(-12f, 12f); // angepasst an 24x24
        float goalX = Random.value < 0.5f ? -24f : 24f; // zufällig links oder rechts
        goal.localPosition = new Vector3(goalX, goal.localPosition.y, goalZ);
        
        SpawnObstacles();
        // Schafe spawnen
        sheepController.DestroyAllChildren();
        sheepController.Spawn();
        if (sheepController.transform.childCount > 0)
        {
            firstSheep = sheepController.transform.GetChild(0).transform;
            oldDistanceSG = Vector3.Distance(firstSheep.position, goal.transform.position);
            oldDistanceHS = Vector3.Distance(transform.position, firstSheep.position);
        }
        else
        {
            Debug.LogWarning("No sheep spawned!");
        }

        rb.linearDamping = 15f;

        // Hindernisse spawnen
        
    }

    private void SpawnObstacles()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Obstacle"))
        {
            Destroy(obj);
            
        }

        // Neue Hindernisse spawnen
        for (int i = 0; i < obstacleAmount; i++)
        {
            if (obstacles.Count == 0) return;

            GameObject prefab = obstacles[Random.Range(0, obstacles.Count)];
            float x = Random.Range(-12f, 12f);
            float z = Random.Range(-12f, 12f);
            Vector3 position = new Vector3(x, 0f, z);
            Quaternion rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            GameObject instance = Instantiate(prefab, position, rotation);
            instance.tag = "Obstacle";
          
        }
    }

    private void MoveAgent(ActionSegment<int> act)
    {
        var action = act[0];
        float maxSpeed = 2.5f;
        //set Branch size in behavior parameter to case n
        switch (action)
        {
            case 0:
                AddReward(-0.01f);
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
                AddReward(-0.01f);
                break;
            case 3: //Rotate right
                transform.Rotate(0f, roationSpeed * Time.deltaTime, 0f);
                AddReward(-0.01f);
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
        if (Keyboard.current.spaceKey.isPressed) {
            EndEpisode();
        }
    }
    
    public void GoalReached()
    {
        AddReward(3.0f);
        CumulativeReward= GetCumulativeReward();
        if(NoMoreSheepsLeft()) EndEpisode();

    }
    private bool NoMoreSheepsLeft()
    {
        int activeChildCount = 0;

        foreach (Transform child in sheepController.transform)
        {
            if (child.gameObject.activeSelf)
            {
                activeChildCount++;
            }
        }

        if (activeChildCount > 0)
        {
            return false;
        }
        return true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Fance")||collision.gameObject.CompareTag("Obstacle"))
        {
            AddReward(-0.01f);
        }
        
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Fance") || collision.gameObject.CompareTag("Obstacle"))
        {
            AddReward(-0.01f);
        }
    }
    
}
