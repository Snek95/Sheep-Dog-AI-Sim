using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    public Dog dog;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sheep") && dog != null)
        {
            if (other.TryGetComponent<RLSheepBehaviour>(out RLSheepBehaviour sheep))
            {
                dog.GoalReached();
                sheep.controller.SheepReachedGoal(sheep);
            }
            else if (other.TryGetComponent<SheepBehaviour>(out SheepBehaviour boidSheep))
            {
                dog.GoalReached();
                boidSheep.RemoveSheep();
            }            
        }
    }
}
