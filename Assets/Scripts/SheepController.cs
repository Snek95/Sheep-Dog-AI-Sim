using System.Collections.Generic;
using UnityEngine;

public class SheepController : MonoBehaviour
{
    public GameObject sheepPrefab;

    public int spawnCount = 10;

    [Range(0.1f, 20.0f)]
    public float spawnDistance = 2.0f;

    [Header("(Xmin, Xmax, Zmin, Zmax)")]
    public Vector4 area = new Vector4(-15, 15, -15, 15);


    [Tooltip("Multiplikator für die Gesamtgeschwindigkeit der Schafe. (Skaliert die Geschwindigkeit aller Bewegungsrichtungen. Kann zu Problemen führen wenn gesamt Alignment oder Inertia zu hoch werden, dann werden Herden nicht mehr langsamer)")]
    [Range(0.0f, 20.0f)]
    public float speedMultiplier = 1.0f;

    [Tooltip("Minimale Geschwindigkeit, mit der sich ein Schaf bewegen kann.")]
    [Range(0.0f, 30.0f)]
    public float minSpeed = 0.5f;

    [Tooltip("Maximale Geschwindigkeit, mit der sich ein Schaf bewegen kann.")]
    [Range(0.0f, 50.0f)]
    public float maxSpeed = 10.0f;

    [Tooltip("Wie schnell sich ein Schaf in Bewegungsrichtung dreht. (Ähnlicher Effekt wie die InertiaStrength. Verhindert appruptes Richtungsänderung)")]
    [Range(0.1f, 20.0f)]
    public float rotationCoeff = 4.0f;

    [Tooltip("Radius, in dem andere Schafe als Nachbarn erkannt werden.")]
    [Range(0.1f, 10.0f)]
    public float neighborDist = 2.0f;

    [Tooltip("Stärke, mit der Schafe den Rand des Spielfelds meiden.")]
    [Range(0.0f, 50.0f)]
    public float boundaryAvoidanceStrength = 20.0f;

    [Tooltip("Kurve für die Randvermeidung (Dadurch schon leichte Vermeidung bevor das Schaf gegen den Zaun läuft.).")]
    public AnimationCurve boundaryAvoidanceCurve;

    [Tooltip("Stärke, mit der Schafe Hindernisse meiden.")]
    [Range(0.0f, 50.0f)]
    public float barrierAvoidanceStrength = 20.0f;

    [Tooltip("Kurve für die Hindernissvermeidung (Dadurch schon leichte Vermeidung bevor das Schaf gegen ein Hindernis läuft.).")]
    public AnimationCurve barrierAvoidanceCurve;

    [Tooltip("Stärke, mit der Schafe zur Herde gezogen werden (Kohäsion).")]
    [Range(0.0f, 10.0f)]
    public float attractionStrength = 1.0f;

    [Tooltip("Kurve für die Anziehung zu anderen Schafen.")]
    public AnimationCurve attractionCurve;

    [Tooltip("Stärke, mit der Schafe Abstand zu anderen halten (Separation).")]
    [Range(0.0f, 10.0f)]
    public float separationStrength = 1f;

    [Tooltip("Kurve für die Separationskraft.")]
    public AnimationCurve separationCurve;

    [Tooltip("Stärke, mit der Schafe sich an der Bewegungsrichtung der Nachbarn ausrichten (Alignment). Sorgt dafür dass Schafe in die gleiche Richtung schauen und sich dadurch die Flucht vor dem Hund durch die Herde verbreitet. (Probleme siehe SpeedMultiplier)")]
    [Range(0.0f, 1.0f)]
    public float alignmentStrength = 1.0f;

    [Tooltip("Kurve für die Ausrichtung an Nachbarn.")]
    public AnimationCurve alignmentCurve;

    [Tooltip("Wie stark das Schaf seine bisherige Bewegungsrichtung beibehält.(Probleme siehe SpeedMultiplier)")]
    [Range(0.0f, 1.0f)]
    public float inertiaStrength = 1.0f;

    [Tooltip("Stärke der zufälligen Bewegungskomponente.")]
    [Range(0.0f, 10.0f)]
    public float randomMovementStrength = 1.0f;

    [Tooltip("Beschreibt wie schnell sich der Vektor für die Zufallskomponente ändert.")]
    [Range(0.0f, 10.0f)]
    public float randomMovementFrequency = 1.0f;

    [Tooltip("Layer, auf dem nach Nachbarschafen gesucht wird.")]
    public LayerMask searchLayer;

    [Tooltip("Referenz auf den Hund, vor dem die Schafe fliehen.")]
    public Transform dog;

    [Tooltip("Kurve für die Fluchtreaktion vor dem Hund.")]
    public AnimationCurve dogFearCurve;

    
    public void Spawn()
    {
        for (var i = 0; i < spawnCount; i++)
        {
            Spawn(transform.position + new Vector3(Random.Range(-spawnDistance, spawnDistance), 0, Random.Range(-spawnDistance, spawnDistance)));
        }
        
    }

    public GameObject Spawn(Vector3 position)
    {
        float randomY = Random.Range(0f, 360f);
        Quaternion randomRotation = Quaternion.Euler(0, randomY, 0);
        var boid = Instantiate(sheepPrefab, position, randomRotation,transform) as GameObject;
        boid.GetComponent<SheepBehaviour>().controller = this;
        return boid;
    }
    public void DestroyAllChildren()
    {
        for (int i = transform.childCount; i > 0; i--)
        {
            Transform child = transform.GetChild(i-1);
            GameObject.Destroy(child.gameObject);
        }
    }
}
