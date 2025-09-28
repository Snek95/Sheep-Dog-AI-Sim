using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    public Dog dog;
    public PlayerDog playerDog;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sheep"))
        {
            if (dog != null && dog.isOpponentDog)
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
            } else if (playerDog != null && !dog.gameObject.activeSelf)
            {
                if (other.TryGetComponent<RLSheepBehaviour>(out RLSheepBehaviour sheep))
                {
                    playerDog.GoalReached();
                    sheep.controller.SheepReachedGoal(sheep);
                }
                else if (other.TryGetComponent<SheepBehaviour>(out SheepBehaviour boidSheep))
                {
                    playerDog.GoalReached();
                    boidSheep.RemoveSheep();
                }
            }
            

                
        }
    }
}
