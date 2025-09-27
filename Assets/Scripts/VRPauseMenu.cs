using UnityEngine;
using UnityEngine.InputSystem;

public class VRPauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private InputActionProperty pauseAction;

    private bool isPaused = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.OnStateChanged += GM_OnStateChanged;
    }

    private void GM_OnStateChanged(object sender, System.EventArgs e) {
        if (GameManager.Instance.IsPaused()) {
            isPaused = true;
        } else {
            isPaused = false;
        }
    }

    private void OnEnable() {

        pauseAction.action.performed += TogglePause;
        pauseAction.action.Enable();
    }

    private void OnDisable() {
        pauseAction.action.performed -= TogglePause;
        pauseAction.action.Disable();
    }

    private void TogglePause(InputAction.CallbackContext obj) {

        Debug.Log("PauseButton");
        isPaused = !isPaused;

        if (pauseMenuUI != null) {
            pauseMenuUI.SetActive(isPaused);
        }
        GameManager.Instance.OnPause(isPaused);

        if (isPaused) {
            Time.timeScale = 0f;
        }

        else {
            Time.timeScale = 1f;
        }
    }

   
}
