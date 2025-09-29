using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;

public class PlayerDog : MonoBehaviour {
    [SerializeField] private Transform goal;
    
      
    
    [SerializeField] private SheepController sheepController;
    [SerializeField] private RLSheepController rlSheepController;
    [SerializeField] private Transform spawnReference;
    [SerializeField] private List<GameObject> obstacles;

    private int obstacleCount;
    private int maxSheep = 10;
    private bool useRLSheep = false;

    

    private Vector3 ogStablePos = new Vector3(-11.79f, 2.93f, -2.42f);
    private List<Transform> activeSheep = new List<Transform>();

    [HideInInspector] public int CurrentEpisode = 0;
    [HideInInspector] public float CumulativeReward = 0f;

    private int sheepsInGoal = 0;
  
     void Start() {

        GameManager.Instance.OnStateChanged += GM_OnStateChanged;
    }

    private void Restart() {

        obstacleCount = GameManager.Instance.obstacleCount;
        maxSheep = GameManager.Instance.SheepCount;
        useRLSheep = GameManager.Instance.useRLSheep;
        sheepsInGoal = 0;
        SpawnObjects();
        transform.localPosition = new Vector3(-8.45f, 1f, -28f);
    }

    private void GM_OnStateChanged(object sender, System.EventArgs e) {
        if (GameManager.Instance.IsPlayingFP()) {
            transform.localPosition = new Vector3(-8.45f, 3f, -28f);
            GameManager.Instance.SetDogRef(gameObject.transform);
            Restart();
        }
    }

    


    private void SpawnObjects() {
        transform.localRotation = Quaternion.identity;
        transform.localPosition = new Vector3(0f, 3f, 0f);

        if (GoalManager.Instance.getRandomGoalLocation()) {
            SpawnRandomGoal();
        } else {
            goal.transform.localPosition = ogStablePos;
        }

        if (GoalManager.Instance.getSpawnObstacles()) {
            SpawnObstacles();
        }

        if (useRLSheep)
        {
            if (rlSheepController == null) return;
            rlSheepController.ResetScene();
            foreach (RLSheepBehaviour sheepBehaviour in rlSheepController.sheepList)
            {
                if (sheepBehaviour != null && sheepBehaviour.gameObject.activeSelf)
                    activeSheep.Add(sheepBehaviour.transform);
            }
        }
        else
        {
            if (sheepController == null) return;
            sheepController.DestroyAllChildren();
            sheepController.Spawn();
            foreach (Transform sheep in sheepController.transform) {
                if (sheep != null && sheep.gameObject.activeSelf)
                    activeSheep.Add(sheep);
            }
        }

        
    }

    private void SpawnRandomGoal() {
        float goalX = Random.Range(-12f, 12f);
        float goalZ = Random.Range(-12f, 12f);

        Vector3 randomGoalPos = new Vector3(spawnReference.position.x + goalX, 0.63f, spawnReference.position.z + goalZ);

        goal.transform.position = randomGoalPos;
    }

    private void SpawnObstacles() {
        foreach (Transform child in spawnReference.transform) {
            if (child.CompareTag("Obstacle")) Destroy(child.gameObject);
        }

        for (int i = 0; i < obstacleCount; i++) {
            if (obstacles.Count == 0) return;

            GameObject prefab = obstacles[Random.Range(0, obstacles.Count)];
            float x = Random.Range(-12f, 12f);
            float z = Random.Range(-12f, 12f);
            Vector3 pos = new Vector3(spawnReference.position.x + x, 0f, spawnReference.position.z + z);
            Quaternion rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            GameObject instance = Instantiate(prefab, pos, rot, spawnReference);
            instance.tag = "Obstacle";
        }
    }

    

    public void GoalReached() {        
        sheepsInGoal++;   

        activeSheep = activeSheep.Where(s => s != null && s.gameObject.activeSelf).ToList();

        if (NoMoreSheepsLeft()) Restart();
    }

    private bool NoMoreSheepsLeft() {
        return activeSheep.Count == 0;
    }

    
}
