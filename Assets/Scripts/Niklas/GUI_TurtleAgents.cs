using UnityEngine;

public class GUI_TurtleAgents : MonoBehaviour
{

    [SerializeField] private TurtleAgent turtleAgent;

    private GUIStyle defaultStyle = new GUIStyle();
    private GUIStyle positiveStyle = new GUIStyle();
    private GUIStyle negativeStyle = new GUIStyle();

    private void OnGUI() {

        string debugEpisode = "Episode: " + turtleAgent.currentEpisode + " - Step: " + turtleAgent.StepCount;
        string debugReward = "Reward: " + turtleAgent.cumulativeReward.ToString();

        GUIStyle rewardStyle = turtleAgent.cumulativeReward < 0 ? negativeStyle : positiveStyle;

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
