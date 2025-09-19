using UnityEngine;

public class XRControllerOverrides : MonoBehaviour
{
    [SerializeField] GameObject LocoMotionController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManger.Instance.OnStateChanged += GameManager_OnStateChanged;
    }

    private void GameManager_OnStateChanged(object sender, System.EventArgs e) {

        if (GameManger.Instance.IsMainMenu()) {

            LocoMotionController.SetActive(false);
        }

        if (GameManger.Instance.IsPaused()) {

            LocoMotionController.SetActive(false);
        }

        if (GameManger.Instance.IsPlayingFP()) {

            LocoMotionController.SetActive(true);
        }

        if (GameManger.Instance.IsPlayingTP()) {

            LocoMotionController.SetActive(false);
        }

        if (GameManger.Instance.IsGameOver()) {

            LocoMotionController.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
