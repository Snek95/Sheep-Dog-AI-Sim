using System;
using Unity.MLAgents;
using UnityEngine;

public class GoalManager : MonoBehaviour {

    public static GoalManager Instance { get; private set; }

    [SerializeField] GameObject trigger; //Reihenfolge: vorne, hinten, links, rechts
    private int openSidesOverride = 3;

    private bool spawnObstacles = false;
    private bool randomGoalLocation = false;


    private void Awake() {
        Instance = this;
    }

    private void Start() {

        openSidesOverride = GameManager.Instance.difficulty;
        UpdateGoalSides();
    }

    public void UpdateGoalSides() {

        //float openSidesValue = Academy.Instance.EnvironmentParameters.GetWithDefault("goal_open_sides", 4);
        //int openSides = Mathf.RoundToInt(openSidesValue);

        switch (openSidesOverride) {
            case 1: //Vierseitiges Ziel, mit Obstacles, mit Random Goal
                trigger.SetActive(true);
                spawnObstacles = true;
                randomGoalLocation = true;
                Debug.Log("Mode 1");
                break;

            case 2: //Vierseitiges Ziel, keine Obstacles, Random Goal
                trigger.SetActive(true);
                spawnObstacles = false;
                randomGoalLocation = true;
                Debug.Log("Mode 2");
                break;

            case 3: //Vierseitiges Ziel, keine Obstacles, kein random Goal
                trigger.SetActive(true);
                spawnObstacles = false;
                randomGoalLocation = false;
                Debug.Log("Mode 3");
                break;

            default:
                Debug.Log("Kein gueltiger Wert (nicht 1-3)");
                break;
        }
    }

    public bool getSpawnObstacles() {
        return spawnObstacles;
    }

    public bool getRandomGoalLocation() {
        return randomGoalLocation;
    }

    public void SetMode(int mode) {

        openSidesOverride = mode;

    }
}
