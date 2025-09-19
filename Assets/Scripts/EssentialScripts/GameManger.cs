using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameManger : MonoBehaviour //Eine globale Instanz auf die alle Scripte zugreifen können, in der Regel auch als Singleton Pattern bekannt0. Hier werden in der Regel allgemeine Einstellungen, wie z.B. Anzahl der Schafe, Schwierigkeitsgrad etc. gespeichert
{
    public static GameManger Instance { get; private set; }

    public int difficulty = 3;
    public bool overwriteDifficulty = false;

    public int SheepCount = 10;
    public int obstacleCount = 10;
    public bool useRLSheep = false;

    private float timer = 0f;
    [SerializeField] private float logInterval = 5f;

    private float episodeCount;
    private float meanReward;

    Dog[] dogs;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        dogs = FindObjectsByType<Dog>(FindObjectsSortMode.None);
    }

    private void Update() {

        timer += Time.deltaTime;

        if (timer > logInterval) {
            PrintMeanReward();
            timer = 0f;
        }

        GoalManager.Instance.SetMode(difficulty);
    }

    private void PrintMeanReward() {

        dogs = FindObjectsByType<Dog>(FindObjectsSortMode.None);

        float avgReward = dogs.Average(d => d.CumulativeReward);
        Debug.Log($"[GameManager] Durchschnittlicher Reward ({dogs.Length} Agents): {avgReward:F2} Episode: {episodeCount:F2}" );

    }

    public void IncreaseDifficulty() {

        if (!overwriteDifficulty) { 

        if (episodeCount > 50) {
            GoalManager.Instance.SetMode(2);
        }
            if (episodeCount > 100) {
                GoalManager.Instance.SetMode(1);

            } else { GoalManager.Instance.SetMode(difficulty); }
        }
    }
    public void AddEpisode() {
        if (dogs == null || dogs.Length == 0) return;

        episodeCount += 1f / dogs.Length;
    }

}
