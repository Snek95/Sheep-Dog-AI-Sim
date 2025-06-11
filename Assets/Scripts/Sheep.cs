using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SheepController : MonoBehaviour
{
    
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float fenceAvoidanceRadius = 2f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float rotationSpeed = 180f; // Grad pro Sekunde

    private Rigidbody rb;
    public Transform dog;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 fleeDirection = Vector3.zero;
        int influenceCount = 0;

        // Vom Hund weg
        if (dog != null)
        {
            Vector3 toDog = transform.position - dog.position;
            if (toDog.magnitude < detectionRadius)
            {
                fleeDirection += toDog.normalized;
                influenceCount++;
            }
        }

        // Von Zäunen weg
        Collider[] nearby = Physics.OverlapSphere(transform.position, fenceAvoidanceRadius);
        foreach (var col in nearby)
        {
            if (col.CompareTag("Fance"))
            {
                Vector3 toFence = transform.position - col.ClosestPoint(transform.position);
                if (toFence.magnitude > 0.01f)
                {
                    fleeDirection += toFence.normalized;
                    influenceCount++;
                }
            }
        }

        if (influenceCount > 0)
        {
            Vector3 desiredDir = fleeDirection.normalized;

            // Drehung: Richtung (forward vs. desiredDir)
            float angleToTarget = Vector3.SignedAngle(transform.forward, desiredDir, Vector3.up);

            // Drehen in Richtung
            float maxTurn = rotationSpeed * Time.fixedDeltaTime;
            float clampedAngle = Mathf.Clamp(angleToTarget, -maxTurn, maxTurn);
            transform.Rotate(0, clampedAngle, 0);

            // Vorwärtsbewegung entlang der Blickrichtung
            rb.AddForce(transform.forward * moveSpeed, ForceMode.Acceleration);

            // Max. Geschwindigkeit begrenzen
            if (rb.linearVelocity.magnitude > moveSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
            }
        }
        else
        {
            // keine Bedrohung -> Schaf bremst
            rb.linearVelocity *= 0.95f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, fenceAvoidanceRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2f);
    }
}
