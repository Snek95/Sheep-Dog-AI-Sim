using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;

public class Dog : Agent {
    [SerializeField] private Transform goal;
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float roationSpeed = 0.001f;

    
    private Vector3 last_known_goal_pos = Vector3.zero;
    private Dictionary<int, Vector3> last_known_sheep_positions = new Dictionary<int, Vector3>();

    private bool useRLSheep = false;
    [SerializeField] private SheepController sheepController;
    [SerializeField] private RLSheepController rlSheepController;
    [SerializeField] private Transform spawnReference;
    [SerializeField] private List<GameObject> obstacles;
    private int obstacleAmount;

    public float optimalMinDist = 2f;
    public float optimalMaxDist = 5f;
    public float timeScale = 1f;

    private int maxSheep = 10;
    

    private Vector3 ogStablePos = new Vector3(-11.79f, 2.93f, -2.42f);

    [HideInInspector] public int CurrentEpisode = 0;
    [HideInInspector] public float CumulativeReward = 0f;

    private Rigidbody rb;
    private int sheepsInGoal = 0;

    private RayPerceptionSensorComponent3D raySensorComponent;

    private List<Transform> activeSheep = new List<Transform>();
    private Dictionary<Transform, float> sheepPrevDist = new Dictionary<Transform, float>();

    public override void Initialize() {
        rb = GetComponent<Rigidbody>();
        Time.timeScale = timeScale;
        raySensorComponent = GetComponent<RayPerceptionSensorComponent3D>();
    }

    public override void OnEpisodeBegin() {

        obstacleAmount = GameManger.Instance.obstacleCount;
        maxSheep = GameManger.Instance.SheepCount;
        useRLSheep = GameManger.Instance.useRLSheep;
        // Lösche die gespeicherten Positionen zu Beginn jeder Episode
        last_known_sheep_positions.Clear();
        last_known_goal_pos = Vector3.zero;

        GameManger.Instance.AddEpisode();
        GameManger.Instance.IncreaseDifficulty();
        CurrentEpisode++;
        CumulativeReward = 0f;
        sheepsInGoal = 0;
        sheepPrevDist.Clear();
        activeSheep.Clear();

        SpawnObjects();
        GoalManager.Instance.UpdateGoalSides();
    }

    public override void CollectObservations(VectorSensor sensor) {
        if (raySensorComponent.RaySensor != null) {
            var rayOutput = raySensorComponent.RaySensor.RayPerceptionOutput;

            if (rayOutput != null && rayOutput.RayOutputs != null) {
                var currentSheepSeen = new Dictionary<int, Vector3>();

                foreach (var rayInfo in rayOutput.RayOutputs) {
                    if (rayInfo.HitGameObject) {
                        if (rayInfo.HitGameObject.CompareTag("Sheep")) {
                            var sheepId = rayInfo.HitGameObject.GetInstanceID();
                            currentSheepSeen[sheepId] = rayInfo.HitGameObject.transform.position;
                        } else if (rayInfo.HitGameObject.CompareTag("Goal")) {
                            last_known_goal_pos = rayInfo.HitGameObject.transform.position;
                        }
                    }
                }

                foreach (var entry in currentSheepSeen) {
                    last_known_sheep_positions[entry.Key] = entry.Value;
                }
            }
        }

        float fieldSize = 37f; // maximale Spielfeldgröße zur Normalisierung

        // Goal Position relativ zum Hund, skaliert
        Vector3 relativeGoal = (last_known_goal_pos - transform.position) / fieldSize;
        sensor.AddObservation(relativeGoal);

        // Nur aktive Schafe berücksichtigen
        var activeSheepPositions = last_known_sheep_positions.Values
            .Where(pos => pos != null)
            .ToList();

        // Nach Distanz zum Hund sortieren und maximal maxSheep aufnehmen
        var sortedSheep = activeSheepPositions
            .OrderBy(pos => Vector3.Distance(transform.position, pos))
            .Take(maxSheep)
            .ToList();

        int count = 0;
        foreach (var sheepPos in sortedSheep) {
            Vector3 relativeSheep = (sheepPos - transform.position) / fieldSize;
            sensor.AddObservation(relativeSheep);
            count++;
        }

        // Fehlende Plätze mit 0 auffüllen, damit Beobachtungsvektor konstant bleibt
        for (; count < maxSheep; count++) {
            sensor.AddObservation(Vector3.zero);
        }
    }

    


    public override void OnActionReceived(ActionBuffers actions) {

        
        MoveAgent(actions.DiscreteActions);

        // kleine Strafe pro Schritt, animiert Bewegung
        AddReward(-0.05f / MaxStep);

        float stepReward = 0f;
        Vector3 dogPos = transform.position;

        foreach (Transform sheepChild in activeSheep.ToList()) {
            if (sheepChild == null || !sheepChild.gameObject.activeSelf) {
                activeSheep.Remove(sheepChild);
                continue;
            }

            Vector3 sheepPos = sheepChild.position;

            // 1. Abstandskontrolle (stärkere Bestrafung)
            float dist = Vector3.Distance(dogPos, sheepPos);
            if (dist >= optimalMinDist && dist <= optimalMaxDist)
                stepReward += 0.004f;
            else
                stepReward -= 0.005f;

            // 2. Einflussvektor
            Vector3 toGoal = (goal.position - sheepPos).normalized;
            Vector3 toDog = (dogPos - sheepPos).normalized;
            float alignment = Vector3.Dot(toGoal, toDog);
            stepReward += alignment * 0.0002f;

            // 3. Fortschritt Richtung Ziel
            float newDist = Vector3.Distance(sheepPos, goal.position);
            if (sheepPrevDist.TryGetValue(sheepChild, out float prevDist)) {
                if (newDist < prevDist) stepReward += 0.006f;
                else stepReward -= 0.002f;
            }
            sheepPrevDist[sheepChild] = newDist;
        }

        // Herdenkohäsion (weniger Strafe, mehr Belohnung)
        HerdCohesionReward(activeSheep);

        // Gruppenerfolg
        stepReward += sheepsInGoal * 0.01f;

        AddReward(stepReward);

        // Endbedingung
        if (NoMoreSheepsLeft()) {
            AddReward(150.0f); // deutlich höhere Belohnung
            EndEpisode();
        }

        CumulativeReward = GetCumulativeReward();
    }

    private Vector3 CalcHerdCenter(List<Transform> sheep) {
        Vector3 center = Vector3.zero;
        int count = 0;
        foreach (var s in sheep) {
            if (s != null && s.gameObject.activeSelf) {
                center += s.position;
                count++;
            }
        }
        return count > 0 ? center / count : center;
    }

    private void HerdCohesionReward(List<Transform> sheep) {
        Vector3 center = CalcHerdCenter(sheep);
        float cohesionReward = 0f;

        foreach (var s in sheep) {
            if (s == null || !s.gameObject.activeSelf) continue;

            float distanceToCenter = Vector3.Distance(center, s.position);
            if (distanceToCenter < 4f) cohesionReward += 0.004f; // Belohnung leicht erhöht
            else if (distanceToCenter > 6f) cohesionReward -= 0.0005f; // Strafe reduziert
        }

        AddReward(cohesionReward);
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

        rb.linearDamping = 15f;
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

        for (int i = 0; i < obstacleAmount; i++) {
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

    private void MoveAgent(ActionSegment<int> act) {
        var action = act[0];
        float maxSpeed = 2.5f;

        switch (action) {
            case 0:
                AddReward(-0.01f);
                break;
            case 1:
                rb.AddForce(transform.forward * moveSpeed, ForceMode.VelocityChange);
                if (rb.linearVelocity.magnitude > maxSpeed)
                    rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
                break;
            case 2:
                transform.Rotate(0f, -roationSpeed * Time.deltaTime, 0f);
                AddReward(-0.01f);
                break;
            case 3:
                transform.Rotate(0f, roationSpeed * Time.deltaTime, 0f);
                AddReward(-0.01f);
                break;
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut) {
        var discreteActionsOut = actionsOut.DiscreteActions;

        if (Keyboard.current.dKey.isPressed) discreteActionsOut[0] = 3;
        else if (Keyboard.current.wKey.isPressed) discreteActionsOut[0] = 1;
        else if (Keyboard.current.aKey.isPressed) discreteActionsOut[0] = 2;
    }

    public void GoalReached() {
        AddReward(50.0f); // deutlich erhöht
        sheepsInGoal++;
        CumulativeReward = GetCumulativeReward();

        activeSheep = activeSheep.Where(s => s != null && s.gameObject.activeSelf).ToList();

        if (NoMoreSheepsLeft()) EndEpisode();
    }

    private bool NoMoreSheepsLeft() {
        return activeSheep.Count == 0;
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Obstacle"))
            AddReward(-1f);
    }

    private void OnCollisionStay(Collision collision) {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Obstacle"))
            AddReward(-0.01f);
    }
}
