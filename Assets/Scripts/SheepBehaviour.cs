using System.Collections.Generic;
using UnityEngine;

public class SheepBehaviour : MonoBehaviour
{
    public SheepController controller;

    public Vector3 currentVelocity = Vector3.zero;

    public List<GameObject> activeBarriers = new List<GameObject>();

    float randomMovementOffset;

    void Start()
    {
        randomMovementOffset = Random.value;
        controller = transform.parent.gameObject.GetComponent<SheepController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentPosition = transform.position;

        //---Influences---
        Vector3 separation = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 attraction = Vector3.zero;
        Vector3 inertia = currentVelocity * controller.inertiaStrength;
        Vector3 randomMovement = Vector3.zero;

        Vector3 dog = Vector3.zero;
        Vector3 boundary = Vector3.zero;
        Vector3 barriers = Vector3.zero;

        //---Sheep interactions---
        var neighbors = Physics.OverlapSphere(transform.position, controller.neighborDist, controller.searchLayer);

        foreach (var neighbor in neighbors)
        {
            if (neighbor.gameObject == gameObject) continue;

            Vector3 neighborPosition = neighbor.transform.position;
            Vector3 neighborDirection = neighbor.transform.forward;

            float distance = Vector3.Distance(currentPosition, neighborPosition);

            // Separation
            float neighborSeparationStrength = controller.separationCurve.Evaluate(distance / controller.neighborDist);
            separation += (currentPosition - neighborPosition).normalized * neighborSeparationStrength;

            // Alignment
            float neighborDirectionStrength = controller.alignmentCurve.Evaluate(distance / controller.neighborDist);
            alignment += neighbor.GetComponent<SheepBehaviour>().currentVelocity * neighborDirectionStrength;

            // Cohesion
            float neighborAttractionStrength = controller.attractionCurve.Evaluate(distance / controller.neighborDist);
            attraction += (neighborPosition - currentPosition).normalized * neighborAttractionStrength;
        }

        if (neighbors.Length > 0)
        {
            alignment = alignment / neighbors.Length * controller.alignmentStrength;
            attraction = attraction / neighbors.Length * controller.attractionStrength;
        }
        separation *= controller.separationStrength;

        //---Dog interactions---
        float dogDistance = Vector3.Distance(currentPosition, controller.dog.position);
        float fearStrength = controller.dogFearCurve.Evaluate(dogDistance);
        dog = (currentPosition - controller.dog.position).normalized * fearStrength;

        // Boundary avoidance
        float xMinAvoid = controller.boundaryAvoidanceCurve.Evaluate(-(controller.transform.position.x + controller.area.x - currentPosition.x)) * controller.boundaryAvoidanceStrength;
        float xMaxAvoid = controller.boundaryAvoidanceCurve.Evaluate(controller.transform.position.x + controller.area.y - currentPosition.x) * controller.boundaryAvoidanceStrength;
        float zMinAvoid = controller.boundaryAvoidanceCurve.Evaluate(-(controller.transform.position.z + controller.area.z - currentPosition.z)) * controller.boundaryAvoidanceStrength;
        float zMaxAvoid = controller.boundaryAvoidanceCurve.Evaluate(controller.transform.position.z + controller.area.w - currentPosition.z) * controller.boundaryAvoidanceStrength;
        boundary = new Vector3(-xMaxAvoid + xMinAvoid, 0, -zMaxAvoid + zMinAvoid);

        // BarrierAvoidance
        foreach (var barrier in activeBarriers)
        {
            Vector3 closestPoint = barrier.GetComponentInParent<Collider>().ClosestPointOnBounds(currentPosition);
            Debug.DrawLine(currentPosition, closestPoint, Color.red);
            float distance = Vector3.Distance(currentPosition, closestPoint);
            float strength = controller.barrierAvoidanceCurve.Evaluate(distance) * controller.barrierAvoidanceStrength;
            Vector3 direction = (currentPosition - closestPoint).normalized;
            barriers += direction * strength;
            Debug.Log($"Barrier: {barrier.name}, Distance: {distance}, Strength: {strength}");
        }

        // Random movement
        float xNoise = Mathf.PerlinNoise(Time.time * controller.randomMovementFrequency, randomMovementOffset * 10.0f)-0.46f;
        float zNoise = Mathf.PerlinNoise(Time.time * controller.randomMovementFrequency, randomMovementOffset * 20.0f)-0.46f;
        randomMovement = new Vector3(xNoise, 0, zNoise) * controller.randomMovementStrength;
        Debug.Log($"Random Movement: {xNoise < 0}");

        // Draw debug lines for visualization
        Debug.DrawLine(currentPosition, currentPosition + separation, Color.red);
        Debug.DrawLine(currentPosition, currentPosition + alignment, Color.green);
        Debug.DrawLine(currentPosition, currentPosition + attraction, Color.blue);
        Debug.DrawLine(currentPosition, currentPosition + inertia, Color.yellow);
        Debug.DrawLine(currentPosition, currentPosition + dog, Color.magenta);
        Debug.DrawLine(currentPosition, currentPosition + boundary, Color.cyan);
        Debug.DrawLine(currentPosition, currentPosition + randomMovement, Color.white);
        Debug.DrawLine(currentPosition, currentPosition + barriers, Color.black);

        // Calculate the new direction
        Vector3 newVelocity = (inertia + separation + alignment + attraction + dog + boundary + randomMovement + barriers) * controller.speedMultiplier;
        newVelocity = new Vector3 (newVelocity.x,0, newVelocity.z);
        newVelocity = ClampMagnitude(newVelocity, controller.maxSpeed, controller.minSpeed);
        currentVelocity = newVelocity;

        // Apply rotation
        Quaternion targetRotation = Quaternion.LookRotation(newVelocity);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, controller.rotationCoeff * Time.deltaTime);

        // Move the sheep
        transform.position += transform.forward * newVelocity.magnitude * Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, controller.neighborDist);
    }
    
    public static Vector3 ClampMagnitude(Vector3 v, float max, float min)
    {
        double sm = v.sqrMagnitude;
        if(sm > (double)max * (double)max) return v.normalized * max;
        else if(sm < (double)min * (double)min) return v.normalized * min;
        return v;
    }

    public void RemoveSheep()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
