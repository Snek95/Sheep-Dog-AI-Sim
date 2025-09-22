using System;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

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

    private Transform activeDogRef;

    public event EventHandler OnStateChanged;
    private bool PlayingFP = true;

    public enum GameState {
        MainMenu,
        PlayingFP,
        PlayingTP,
        Paused,
        GameOver
    }

    private GameState currentState;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        dogs = FindObjectsByType<Dog>(FindObjectsSortMode.None);
        SetState(GameState.MainMenu);
    }

    private void Update() {
        timer += Time.deltaTime;

        if (timer > logInterval) {
            //PrintMeanReward();
            timer = 0f;
        }

        GoalManager.Instance.SetMode(difficulty);
    }

    // ----------------------
    // State Handling
    // ----------------------
    public void SetState(GameState newState) {
        if (currentState != newState) {
            currentState = newState;
            OnStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool IsMainMenu() => currentState == GameState.MainMenu;
    public bool IsPlayingTP() => currentState == GameState.PlayingTP;
    public bool IsPlayingFP() => currentState == GameState.PlayingFP;
    public bool IsPaused() => currentState == GameState.Paused;
    public bool IsGameOver() => currentState == GameState.GameOver;

    public void OnPressPlay() {
        if (PlayingFP) {
            SetState(GameState.PlayingFP);
        } else {
            SetState(GameState.PlayingTP);
        }
    }

    public void OnPause() {
        SetState(GameState.Paused);
    }

    public void OnGameOver() {
        SetState(GameState.GameOver);
    }

    public void PlayMode(int mode) {
        if (mode == 0) {
            PlayingFP = true;
        } else if (mode == 1) {
            PlayingFP = false;
        }
    }

    

    /*private void PrintMeanReward() {
        dogs = FindObjectsByType<Dog>(FindObjectsSortMode.None);
        float avgReward = dogs.Average(d => d.CumulativeReward);
        Debug.Log($"[GameManager] Durchschnittlicher Reward ({dogs.Length} Agents): {avgReward:F2} Episode: {episodeCount:F2}");
    }*/

    public void IncreaseDifficulty() {
        if (!overwriteDifficulty) {
            if (episodeCount > 50) {
                GoalManager.Instance.SetMode(2);
            }
            if (episodeCount > 100) {
                GoalManager.Instance.SetMode(1);
            } else {
                GoalManager.Instance.SetMode(difficulty);
            }
        }
    }

    public void AddEpisode() {
        if (dogs == null || dogs.Length == 0) return;
        episodeCount += 1f / dogs.Length;
    }

    public void ToggleRLSheep(bool rlsheepToggle) {
        useRLSheep = rlsheepToggle;
        Debug.Log("Rl Sheep on:  " + useRLSheep);
    }

    public void SetSheepCount(float sheeps) {
        SheepCount = Mathf.RoundToInt(sheeps);
        Debug.Log("Sheeps:  " + sheeps);
    }

    public void SetDogRef(Transform dogRef) {
        activeDogRef = dogRef;
    }

    public Transform GetDogRef() {
        return activeDogRef;
    }
}
