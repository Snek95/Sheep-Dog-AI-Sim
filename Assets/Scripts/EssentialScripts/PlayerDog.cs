using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;


public class PlayerDog : MonoBehaviour {
    [SerializeField] private Transform goal;
    [SerializeField] private GameObject moveProvider;
    [SerializeField] private SheepController sheepController;
    [SerializeField] private RLSheepController rlSheepController;
    [SerializeField] private Transform spawnReference;
    [SerializeField] private List<GameObject> obstacles;
    [SerializeField] private Transform spawnpointTP;
    [SerializeField] private Transform bonePointerOrigin;
    [SerializeField] private GameObject dumbDog;
    [SerializeField] private GameObject bone;
    private int obstacleAmount;
    private int maxSheep = 10;
    private bool useRLSheep = false;

    private PlayerInput playerInput;
    private LineRenderer lineRenderer;

    

    private Vector3 ogStablePos = new Vector3(-11.79f, 2.93f, -2.42f);
    private List<Transform> activeSheep = new List<Transform>();

    [HideInInspector] public int CurrentEpisode = 0;
    [HideInInspector] public float CumulativeReward = 0f;

    private int sheepsInGoal = 0;

    private void Awake() {
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogError("PlayerInput component not found on PlayerDog.");
        }
    }

    void Start()
    {

        GameManager.Instance.OnStateChanged += GM_OnStateChanged;
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void StartFP() {

        obstacleAmount = GameManager.Instance.obstacleCount;
        maxSheep = GameManager.Instance.SheepCount;
        useRLSheep = GameManager.Instance.useRLSheep;
        sheepsInGoal = 0;
        SpawnObjects();
        transform.localPosition = new Vector3(-8.45f, 3f, -28f);
    }

    private void StartTP()
    {
        obstacleAmount = GameManager.Instance.obstacleCount;
        maxSheep = GameManager.Instance.SheepCount;
        useRLSheep = GameManager.Instance.useRLSheep;
        sheepsInGoal = 0;
        SpawnObjects();
        transform.localPosition = spawnpointTP.position;
        bone.transform.SetPositionAndRotation(dumbDog.transform.position, Quaternion.identity);
        GameManager.Instance.SetDogRef(dumbDog.transform);
        moveProvider.SetActive(true);
    }

    private void GM_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsPlayingFP())
        {
            GameManager.Instance.SetDogRef(gameObject.transform);
            StartFP();
        }
        if (GameManager.Instance.IsPlayingTP())
        {
            bone.SetActive(true);
            dumbDog.SetActive(true);
            GameManager.Instance.SetDogRef(dumbDog.transform);
            StartTP();

        }
    }

    void Update()
    {
        if (GameManager.Instance.IsPlayingTP())
        {
            // Shoot raycast from right VR controller when trigger is pressed using OpenXR and save last hit position
            if (playerInput.actions["Activate"].IsPressed())
            {
                Ray ray = new Ray(bonePointerOrigin.position, bonePointerOrigin.forward);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Ground")))
                {
                    bone.transform.SetPositionAndRotation(hit.point, Quaternion.identity);

                    lineRenderer.SetPosition(0, bonePointerOrigin.position);
                    lineRenderer.SetPosition(1, hit.point);
                    lineRenderer.enabled = true;
                } else
                {
                    lineRenderer.enabled = false;
                }
            } else
            {
                lineRenderer.enabled = false;
            }
            
        }
    }




    private void SpawnObjects()
    {
        transform.localRotation = Quaternion.identity;
        transform.localPosition = new Vector3(0f, 3f, 0f);

        if (GoalManager.Instance.getRandomGoalLocation())
        {
            SpawnRandomGoal();
        }
        else
        {
            goal.transform.localPosition = ogStablePos;
        }

        if (GoalManager.Instance.getSpawnObstacles())
        {
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
            foreach (Transform sheep in sheepController.transform)
            {
                if (sheep != null && sheep.gameObject.activeSelf)
                    activeSheep.Add(sheep);
            }
        }


    }

    private void SpawnRandomGoal() {
        float goalX = UnityEngine.Random.Range(-12f, 12f);
        float goalZ = UnityEngine.Random.Range(-12f, 12f);

        Vector3 randomGoalPos = new Vector3(spawnReference.position.x + goalX, 0.63f, spawnReference.position.z + goalZ);

        goal.transform.position = randomGoalPos;
    }

    private void SpawnObstacles() {
        foreach (Transform child in spawnReference.transform) {
            if (child.CompareTag("Obstacle")) Destroy(child.gameObject);
        }

        for (int i = 0; i < obstacleAmount; i++) {
            if (obstacles.Count == 0) return;

            GameObject prefab = obstacles[UnityEngine.Random.Range(0, obstacles.Count)];
            float x = UnityEngine.Random.Range(-12f, 12f);
            float z = UnityEngine.Random.Range(-12f, 12f);
            Vector3 pos = new Vector3(spawnReference.position.x + x, 0f, spawnReference.position.z + z);
            Quaternion rot = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
            GameObject instance = Instantiate(prefab, pos, rot, spawnReference);
            instance.tag = "Obstacle";
        }
    }

    

    public void GoalReached() {        
        sheepsInGoal++;   

        activeSheep = activeSheep.Where(s => s != null && s.gameObject.activeSelf).ToList();

        if (NoMoreSheepsLeft()) StartFP();
    }

    private bool NoMoreSheepsLeft() {
        return activeSheep.Count == 0;
    }

    
}
