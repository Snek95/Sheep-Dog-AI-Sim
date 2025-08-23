using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SheepUI : MonoBehaviour
{
    public List<RLSheepBehaviour> sheepList;

    [SerializeField] private TextMeshProUGUI _episode;
    [SerializeField] private TextMeshProUGUI _meanReward;
    [SerializeField] private TextMeshProUGUI _randomSheepReward;
    // Update is called once per frame
    void Update()
    {
        _episode.text = "Episode: " + (sheepList.Count > 0 ? sheepList[0].CurrentEpisode : "") + " Step: " + (sheepList.Count > 0 ? sheepList[0].StepCount : "");
        float meanCumulativeReward = 0f;
        if (sheepList.Count > 0)
        {
            float totalReward = 0f;
            foreach (var sheep in sheepList)
            {
                totalReward += sheep.CumulativeReward;
            }
            meanCumulativeReward = totalReward / sheepList.Count;
        }
        _meanReward.text = "Mean Reward: " + meanCumulativeReward;
        _randomSheepReward.text = "Random Sheep Reward: " + (sheepList.Count > 0 ? sheepList[0].CumulativeReward : "");
    }
}
