using UnityEngine;
using UnityEngine.Events;

public class TriggerBarrier : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<SheepBehaviour>(out var sheep))
        {
            sheep.activeBarriers.Add(this.transform.parent.Find("Center").gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<SheepBehaviour>(out var sheep))
        {
            sheep.activeBarriers.Remove(this.transform.parent.Find("Center").gameObject);
        }
    }
}
