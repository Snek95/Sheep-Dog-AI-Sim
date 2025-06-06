using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] private Dog _dog;
    [SerializeField] private TextMeshProUGUI _episode;
    [SerializeField] private TextMeshProUGUI _reward;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _episode.text = "Episode: " + _dog.CurrentEpisode + " Step: " + _dog.StepCount;
        _reward.text= "Reward: " + _dog.CumulativeReward.ToString();

        _reward.color = _dog.CumulativeReward < 0 ? Color.red : Color.green;
    }
}
