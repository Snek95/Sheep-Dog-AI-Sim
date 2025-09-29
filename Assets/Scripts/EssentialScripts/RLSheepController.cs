using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class RLSheepController : MonoBehaviour
{
    /// <summary>
    /// Max Academy steps before this platform resets
    /// </summary>
    [Tooltip("Max Environment Steps")] public int MaxEnvironmentSteps = 15000;
    public int sheepAmount = 20;
    [Range(0.1f, 20.0f)]
    public float spawnDistance = 5f;

    public float maxSheepMoveSpeed = 5f;
    public float sheepRotationSpeed = 5f;

    public GameObject RLSheepPrefab;
    public SheepUI sheepUI;
    public GameObject dog;
    public float dogFearRadius = 5f;
    public float neighborDist = 7f;

    public float meanDistanceRewardMultiplier = 0.1f;
    public float sheepDistanceToAverageMultiplier = 0.1f;
    public AnimationCurve sheepDistanceToAverageRewardCurve;

    public List<RLSheepBehaviour> sheepList = new List<RLSheepBehaviour>();
    private SimpleMultiAgentGroup m_SheepGroup;

    [HideInInspector] public Vector3 avgPosition = Vector3.zero;

    private int m_ResetTimer;


    void Start()
    {
        m_SheepGroup = new SimpleMultiAgentGroup();
        if (GameManager.Instance.useRLSheep == false)
        {
            gameObject.SetActive(false);
            return;
        }
    }

    private void FixedUpdate()
    {
        m_ResetTimer += 1;
        if (m_ResetTimer >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0)
        {
            //ResetScene();
        }
        /*
        // Calculate average position of all active sheep
        int activeSheepCount = 0;
        avgPosition = Vector3.zero;
        foreach (var sheep in sheepList)
        {
            if (sheep.gameObject.activeSelf)
            {
                avgPosition += sheep.transform.position;
                activeSheepCount++;
            }
        }
        if (activeSheepCount > 0)
        {
            avgPosition /= activeSheepCount;
        }

        // Calculate mean distance between all pairs of active sheep
        float totalDistance = 0f;
        int pairCount = 0;
        for (int i = 0; i < sheepList.Count; i++)
        {
            if (!sheepList[i].gameObject.activeSelf) continue;
            for (int j = i + 1; j < sheepList.Count; j++)
            {
                if (!sheepList[j].gameObject.activeSelf) continue;
                totalDistance += Vector3.Distance(sheepList[i].transform.position, sheepList[j].transform.position);
                pairCount++;
            }
        }
        float meanDistance = pairCount > 0 ? totalDistance / pairCount : 0f;
        //m_SheepGroup.AddGroupReward(-meanDistance * meanDistanceRewardMultiplier * 0.01f);
        m_SheepGroup.AddGroupReward(sheepDistanceToAverageRewardCurve.Evaluate(meanDistance) * sheepDistanceToAverageMultiplier);*/
    }


    public void SheepReachedGoal(RLSheepBehaviour sheep)
    {
        // Handle the logic when a sheep reaches its goal
        //Debug.Log("Sheep reached goal: " + sheep.name);
        m_SheepGroup.UnregisterAgent(sheep);
        sheep.gameObject.SetActive(false);
        Destroy(sheep.gameObject);
    }

    public void SpawnSheep(int amount)
    {
        Debug.Log("Spawning " + amount + " RL Sheep for " + transform.parent.name);
        for (int i = 0; i < amount; i++)
        {
            var sheep = Instantiate(RLSheepPrefab, transform);
            RLSheepBehaviour sheepBehaviour = sheep.GetComponent<RLSheepBehaviour>();
            sheepBehaviour.controller = this;
            sheepList.Add(sheepBehaviour);
            if (sheepUI != null)
            {
                sheepUI.sheepList.Add(sheepBehaviour);
            }
        }
    }

    public void DestroyAllSheep()
    {
        foreach (var sheep in sheepList)
        {
            if (sheep != null)
            {
                Destroy(sheep.gameObject);
            }
        }
        sheepList.Clear();
        if (sheepUI != null)
        {
            sheepUI.sheepList.Clear();
        }
    }

    public void ResetScene()
    {
        m_SheepGroup.GroupEpisodeInterrupted();
        m_ResetTimer = 0;
        foreach (var sheep in sheepList)
        {
            sheep.gameObject.SetActive(true);
            m_SheepGroup.RegisterAgent(sheep);
            sheep.transform.position = transform.position + new Vector3(Random.Range(-spawnDistance, spawnDistance), 0f, Random.Range(-spawnDistance, spawnDistance));
            sheep.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            Rigidbody rb = sheep.GetComponent<Rigidbody>();
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

    }

    public bool NoSheepLeft()
    {
        foreach (var sheep in sheepList)
        {
            if (sheep.gameObject.activeSelf) return false;
        }
        return true;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(avgPosition, 1f);
        Gizmos.color = Color.purple;
        Gizmos.DrawWireSphere(dog.transform.position, dogFearRadius);
    }
}
