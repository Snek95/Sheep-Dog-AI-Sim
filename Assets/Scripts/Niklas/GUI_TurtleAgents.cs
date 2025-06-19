using UnityEngine;

public class GUI_TurtleAgents : MonoBehaviour
{

    [SerializeField] private WolfAgent wolfAgent;

    private GUIStyle defaultStyle = new GUIStyle();
    private GUIStyle positiveStyle = new GUIStyle();
    private GUIStyle negativeStyle = new GUIStyle();

    private void OnGUI() {

        string debugEpisode = "Episode: " + wolfAgent.currentEpisode + " - Step: " + wolfAgent.StepCount;
        string debugReward = "Reward: " + wolfAgent.cumulativeReward.ToString();

        GUIStyle rewardStyle = wolfAgent.cumulativeReward < 0 ? negativeStyle : positiveStyle;

        GUI.Label(new Rect(20, 20, 500, 30), debugEpisode, defaultStyle);
        GUI.Label(new Rect(20, 60, 500, 30), debugReward, rewardStyle);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        defaultStyle.fontSize = 20;
        defaultStyle.normal.textColor = Color.yellow;

        positiveStyle.fontSize = 20;
        positiveStyle.normal.textColor = Color.green;

        negativeStyle.fontSize = 20;
        negativeStyle.normal.textColor = Color.red;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
