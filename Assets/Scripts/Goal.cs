using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    public Dog dog;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sheep") && dog != null)
        {
            other.GetComponent<SheepBehaviour>().RemoveSheep();
            dog.GoalReached();
            
        }
    }
}
