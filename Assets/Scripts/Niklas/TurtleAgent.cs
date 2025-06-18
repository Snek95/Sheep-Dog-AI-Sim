using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.InputSystem;
using System.Collections;

public class TurtleAgent : Agent
{

    [SerializeField] private Transform goal;
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float rotationSpeed = 180f;

    [SerializeField] private Renderer groundRenderer;

    private new Renderer renderer;

    [HideInInspector] public int currentEpisode = 0;
    [HideInInspector] public float cumulativeReward = 0f;

    private Color defaultGroundColor;
    private Coroutine flashGroundCoroutine;

    public override void Heuristic(in ActionBuffers actionsOut) {

        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = 0; //Do nothing

        if (Keyboard.current.upArrowKey.isPressed) {
            discreteActionsOut[0] = 1;
        
        }

        else if (Keyboard.current.leftArrowKey.isPressed) {
            discreteActionsOut[0] = 2;
        }

        else if (Keyboard.current.rightArrowKey.isPressed) {
            discreteActionsOut[0] = 3;
        }

       
    }

    public override void Initialize() { //Diese Methode wird ausgeführt wenn der Agent erzeugt wird, quasi das Äquivalent zu Awake()
        Debug.Log("Initialize()");

        renderer = GetComponent<Renderer>();

        currentEpisode = 0;
        cumulativeReward = 0f;

        if (groundRenderer != null) {
            defaultGroundColor = groundRenderer.material.color;
        }
    }

    public override void OnEpisodeBegin() {//Diese Methode wird ausgeführt beim Start jeder Episode. In jeder Episode versucht der Agent das Goal zu erreichen, quasi Start()
        Debug.Log("OnEpisodeBegin()");

        if (groundRenderer != null && cumulativeReward != 0f) {
            Color flashColor = (cumulativeReward > 0f) ? Color.green : Color.red; //Wenn Reward größer Null dann Grün, sonst rot

            if (flashGroundCoroutine != null) {
                StopCoroutine(flashGroundCoroutine);
            }

            flashGroundCoroutine = StartCoroutine(FlashGround(flashColor, 3.0f));
        }


        currentEpisode++;
        cumulativeReward = 0f;
        renderer.material.color = Color.blue;

        SpawnObjects();
    }

    private IEnumerator FlashGround(Color targetColor, float duration) {
        float elapsedTime = 0f;
        groundRenderer.material.color = targetColor;

        while (elapsedTime < duration) {
        elapsedTime += Time.deltaTime;
        groundRenderer.material.color = Color.Lerp(targetColor, defaultGroundColor, elapsedTime / duration);
        yield return null;
        }
    }

    private void SpawnObjects() {

        transform.localRotation = Quaternion.identity;
        transform.localPosition = new Vector3(0f,0.15f,0f);

        //Blickrichtung randomisieren

        float randomAngle = Random.Range(0f, 360f);
        Vector3 randomDirection = Quaternion.Euler(0f, randomAngle, 0f) * Vector3.forward;

        //Distanz randomisieren

        float randomDistance = Random.Range(1f, 2.5f);

        //Ziel Position bestimmen
        Vector3 goalPosition = transform.localPosition + randomDirection * randomDistance;

        //Zielposition anwenden
        goal.localPosition = new Vector3(goalPosition.x, 0.3f, goalPosition.z);
    }

    public override void CollectObservations(VectorSensor sensor) {//Damit der Agent aus seiner Umgebung lernen kann muss er observieren, werte werden normalized weil NNs diese sonst nicht gut verwenden können

        //Ziel Position
        float goalPosX_normalized = goal.localPosition.x / 5f;
        float goalPosZ_normalized = goal.localPosition.z / 5f;

        //Turtle Position
        float turtlePosX_normalized = transform.localPosition.x / 5f;
        float turtlePosZ_normalized = transform.localPosition.z / 5f;

        //Turtles Richtung auf der Y Achse
        float turtleRotation_normalized = (transform.localRotation.eulerAngles.y / 360f) * 2f - 1f;

        sensor.AddObservation(goalPosX_normalized);
        sensor.AddObservation(goalPosZ_normalized);
        sensor.AddObservation(turtlePosX_normalized);
        sensor.AddObservation(turtlePosZ_normalized);
        sensor.AddObservation(turtleRotation_normalized);


    }

    public override void OnActionReceived(ActionBuffers actions) {//Nachdem der Agent alle Informationen erhalten hat, soll nun eine Aktion ausgeführt werden
        
        MoveAgent(actions.DiscreteActions);

        AddReward(-2f / MaxStep);

        cumulativeReward = GetCumulativeReward();
    }

    private void MoveAgent(ActionSegment<int> act) {
        var action = act[0];

        switch (action) {
            case 1:
                transform.position += transform.forward * moveSpeed * Time.deltaTime;
                break;
            case 2:
                transform.Rotate(0f, -rotationSpeed * Time.deltaTime, 0f);
                break;
            case 3:
                transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
                break;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Goal")) {
            GoalReached();
        }
    }

    private void OnCollisionEnter(Collision collision) {
        
        if (collision.gameObject.CompareTag("Wall")) {
            AddReward(-0.05f);
        }

        if (renderer != null) {
            renderer.material.color = Color.red;
        }
    }

    private void OnCollisionStay(Collision collision) {

        if (collision.gameObject.CompareTag("Wall")) {
            AddReward(-0.01f * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionExit(Collision collision) {

        if (collision.gameObject.CompareTag("Wall")) {

            if (renderer != null) {
                renderer.material.color = Color.blue;
            }
        }
    }

    private void GoalReached() {
        AddReward(1.0f);
        cumulativeReward = GetCumulativeReward();

        EndEpisode();
    }

}

