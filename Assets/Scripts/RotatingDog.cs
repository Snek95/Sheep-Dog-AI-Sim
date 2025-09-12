using UnityEngine;

public class RotatingDog : MonoBehaviour {
    // Drehgeschwindigkeit in Grad pro Sekunde
    public Vector3 rotationSpeed = new Vector3(0, 100, 0);

    void Update() {
        // Rotiert das Objekt basierend auf der eingestellten Geschwindigkeit
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}