using System;
using Unity.MLAgents;
using UnityEngine;

public class GoalManager : MonoBehaviour {

    public static GoalManager Instance { get; private set; }

    [SerializeField] GameObject[] stableSides; //Reihenfolge: vorne, hinten, links, rechts
    private int openSidesOverride = 3;

    private bool spawnObstacles = false;
    private bool randomGoalLocation = false;


    private void Awake() {
        Instance = this;
    }

    private void Start() {

        openSidesOverride = GameManger.Instance.difficulty;
        UpdateGoalSides();
    }

    public void UpdateGoalSides() {

        //float openSidesValue = Academy.Instance.EnvironmentParameters.GetWithDefault("goal_open_sides", 4);
        //int openSides = Mathf.RoundToInt(openSidesValue);

        foreach (GameObject go in stableSides) {
            go.SetActive(true);
        }

        switch (openSidesOverride) {
            case 1: //Vierseitiges Ziel, mit Obstacles, mit Random Goal
                stableSides[0].SetActive(true);
                stableSides[1].SetActive(true);
                stableSides[2].SetActive(true);
                stableSides[3].SetActive(true);
                spawnObstacles = true;
                randomGoalLocation = true;
                Debug.Log("Mode 1");
                break;

            case 2: //Vierseitiges Ziel, keine Obstacles, Random Goal
                stableSides[0].SetActive(true);
                stableSides[1].SetActive(true);
                stableSides[2].SetActive(true);
                stableSides[3].SetActive(true);
                spawnObstacles = false;
                randomGoalLocation = true;
                Debug.Log("Mode 2");
                break;

            case 3: //Vierseitiges Ziel, keine Obstacles, kein random Goal
                stableSides[0].SetActive(true);
                stableSides[1].SetActive(true);
                stableSides[2].SetActive(true);
                stableSides[3].SetActive(true);
                spawnObstacles = false;
                randomGoalLocation = false;
                Debug.Log("Mode 3");
                break;

            default:
                Debug.Log("Kein gültiger Wert (nicht 1-5)");
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
