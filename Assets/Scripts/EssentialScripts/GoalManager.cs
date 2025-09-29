using System;
using Unity.MLAgents;
using UnityEngine;

public class GoalManager : MonoBehaviour {

    public static GoalManager Instance { get; private set; }

    private bool spawnObstacles = true;
    private bool randomGoalLocation = true;


    private void Awake() {
        Instance = this;
    } 

    

    public bool getSpawnObstacles() {
        return spawnObstacles;
    }

    public bool getRandomGoalLocation() {
        return randomGoalLocation;
    }

    
}
