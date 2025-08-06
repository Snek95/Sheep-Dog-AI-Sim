using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Rendering;
using UnityEditor.Animations;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;

public class WolfAgent : Agent
{
    [SerializeField] private SheepController sheepController;
    [SerializeField] private Transform dog;

    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float rotationSpeed = 180f;

    [SerializeField] private Renderer groundRenderer;

    private new Renderer renderer;
    public float arenaRadius = 20;

    [HideInInspector] public int currentEpisode = 0;
    [HideInInspector] public float cumulativeReward = 0f;

    private Color defaultGroundColor;
    private Color defaultWolfColor;
    private Coroutine flashGroundCoroutine;
    private Rigidbody rb;

    private float closestSheepDistance = Mathf.Infinity;
    private Transform closestSheep;


    private List<Transform> sheepList;

    public override void Heuristic(in ActionBuffers actionsOut) { //Manuelle Übernahme des Agenten

        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = 0; //Do nothing

        if (Keyboard.current.wKey.isPressed) {
            discreteActionsOut[0] = 1;

        } else if (Keyboard.current.aKey.isPressed) {
            discreteActionsOut[0] = 2;
        } else if (Keyboard.current.dKey.isPressed) {
            discreteActionsOut[0] = 3;
        }


    }


    private void Update() {
        if (Keyboard.current.spaceKey.isPressed) {
            EndEpisode();
        }
    }

    public override void Initialize() { //Diese Methode wird ausgeführt wenn der Agent erzeugt wird, quasi das Äquivalent zu Awake()
        Debug.Log("Initialize()");

        SpawnObjects();
        
        renderer = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
        defaultWolfColor = renderer.material.color;

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
        renderer.material.color = defaultWolfColor;

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

        closestSheepDistance = Mathf.Infinity;

        if (sheepController.transform.childCount > 0) {
            sheepController.DestroyAllChildren();
        }

        sheepController.Spawn();

        foreach (Transform sheep in sheepList) {

            float sheepDistance = Vector3.Distance(transform.localPosition, sheep.localPosition);

            if (sheepDistance < closestSheepDistance) {
                closestSheepDistance = sheepDistance;
                closestSheep = sheep;
            }
        }


        transform.localPosition = new Vector3(0, 0.61f, 0);
        transform.localRotation = Quaternion.identity;
                      
       
    }

    /*public override void CollectObservations(VectorSensor sensor) {

        // Richtung und Entfernung zum Schaf
        Vector3 toSheep = sheepController.localPosition - transform.localPosition;
        Vector3 localToSheep = transform.InverseTransformDirection(toSheep);
        float distanceToSheep = toSheep.magnitude / 10f; // ggf. Arena-Radius normalisieren
        float sheepRotation_normalized = (sheepController.localRotation.eulerAngles.y / 360f) * 2f - 1f;

        sensor.AddObservation(localToSheep.x); // Links/Rechts
        sensor.AddObservation(localToSheep.z); // Vorwärts/Rückwärts
        sensor.AddObservation(distanceToSheep);
        sensor.AddObservation(sheepRotation_normalized);

        // Richtung und Entfernung zum Hund
        Vector3 toDog = dog.localPosition - transform.localPosition;
        Vector3 localToDog = transform.InverseTransformDirection(toDog);
        float distanceToDog = toDog.magnitude / 10f;
        float dogRotation_normalized = (dog.localRotation.eulerAngles.y / 360f) * 2f - 1f;

        sensor.AddObservation(localToDog.x);
        sensor.AddObservation(localToDog.z);
        sensor.AddObservation(distanceToDog);
        sensor.AddObservation(dogRotation_normalized);

        float wolfRotation_normalized = (transform.localRotation.eulerAngles.y / 360f) * 2f - 1f;
        sensor.AddObservation(wolfRotation_normalized);

    }*/

    public override void OnActionReceived(ActionBuffers actions) {

        float move = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f); // Vor/zurück
        float turn = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f); // Links/rechts

        rb.AddForce(transform.forward * move * moveSpeed, ForceMode.VelocityChange);
        transform.Rotate(0f, turn * rotationSpeed * Time.deltaTime, 0f);

        AddReward(-1f / MaxStep);

        float currentDistanceToSheep = Vector3.Distance(transform.localPosition, closestSheep.localPosition);
        float distanceDelta = closestSheepDistance - currentDistanceToSheep;

        AddReward(distanceDelta * 0.5f);

        closestSheepDistance = currentDistanceToSheep;

        float distance = Vector3.Distance(transform.localPosition, closestSheep.localPosition);
        if (distance < 1.0f)
            AddReward(0.02f); // Bonus bei Nähe

        Vector3 toSheep = (closestSheep.localPosition - transform.localPosition).normalized;
        float lookDot = Vector3.Dot(transform.forward, toSheep); // 1 = direkt schauen, 0 = orthogonal
        AddReward(lookDot * 0.005f); // kleiner Bonus, wenn er „hinblickt“

        cumulativeReward = GetCumulativeReward();
    }

    private void MoveAgent(ActionSegment<int> act) {
        var action = act[0];

        switch (action) {
            case 0:
                AddReward(-0.001f);
                break;

            case 1: //Move forward
                rb.AddForce(transform.forward * moveSpeed, ForceMode.VelocityChange);
                if (rb.linearVelocity.magnitude > moveSpeed) {
                    rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
                }
                break;

            case 2: //Rotate left
                transform.Rotate(0f, -rotationSpeed * Time.deltaTime, 0f);
                AddReward(-0.001f);
                break;
            case 3: //Rotate right
                transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
                AddReward(-0.004f);
                break;

        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Sheep")) {
            GoalReached();
        }
    }

    private void GoalReached() {
        AddReward(1.0f);
        cumulativeReward = GetCumulativeReward();

        EndEpisode();
    }

    private void OnCollisionEnter(Collision collision) {

        if (collision.gameObject.CompareTag("Wall")) {
            AddReward(-0.2f);
            renderer.material.color = Color.red;
        }

       
        if (collision.gameObject.CompareTag("Dog")) {
            AddReward(-2);
            EndEpisode();
        }

        if (collision.gameObject.CompareTag("Sheep")) {
            GoalReached();
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
                renderer.material.color = defaultWolfColor;
            }
        }
    }

}
