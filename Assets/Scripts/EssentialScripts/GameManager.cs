using System;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    public int SheepCount = 10;
    public int obstacleCount = 10;
    public bool useRLSheep = false;  
  
    private Transform activeDogRef;
    [SerializeField] private Transform startingPosition;
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject playerCharacter;

    public event EventHandler OnStateChanged;
    private bool PlayingFP = true;

    public enum GameState {
        MainMenu,
        Starting,
        Playing,
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
        
        SetState(GameState.MainMenu);
    }

    

    // ----------------------
    // State Handling
    // ----------------------
    public void SetState(GameState newState) {
        if (currentState != newState) {
            currentState = newState;
            OnStateChanged?.Invoke(this, EventArgs.Empty);
            Debug.Log(currentState.ToString());
        }
    }

    public bool IsMainMenu() => currentState == GameState.MainMenu;
    public bool IsPlayingTP() => currentState == GameState.PlayingTP;
    public bool IsPlayingFP() => currentState == GameState.PlayingFP;
    public bool IsPaused() => currentState == GameState.Paused;
    public bool IsGameOver() => currentState == GameState.GameOver;
    public bool IsPlaying() => currentState == GameState.Playing;
    public bool IsStarting() => currentState == GameState.Starting;

    public void OnPressPlay() {
        if (PlayingFP) {
            SetState(GameState.PlayingFP);
            SetState(GameState.Starting);
            SetState(GameState.Playing);
            mainMenuUI.SetActive(false);
        } else {
            SetState(GameState.PlayingTP);
            SetState(GameState.Starting);
            SetState(GameState.Playing);
            mainMenuUI.SetActive(false);
        }
    }

    public void OnPause(bool isPaused) {
        if (isPaused) {
            SetState(GameState.Paused);
        } else {
            SetState(GameState.Playing);
        }
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

    public void ReturnToMenu() {
        SetState(GameState.MainMenu);
        mainMenuUI.SetActive(true);
        playerCharacter.transform.position = startingPosition.position;
        playerCharacter.transform.rotation = startingPosition.rotation;
        
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
