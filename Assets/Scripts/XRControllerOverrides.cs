using UnityEngine;

public class XRControllerOverrides : MonoBehaviour
{
    [SerializeField] GameObject LocoMotionController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        LocoMotionController.SetActive(false);
    }

    private void GameManager_OnStateChanged(object sender, System.EventArgs e) {

        if (GameManager.Instance.IsMainMenu()) {

            LocoMotionController.SetActive(false);
        }

        if (GameManager.Instance.IsPlayingFP()) {

            LocoMotionController.SetActive(true);
        }

        if (GameManager.Instance.IsPlayingTP()) {

            LocoMotionController.SetActive(false);
        }

        if (GameManager.Instance.IsGameOver()) {

            LocoMotionController.SetActive(false);
        }

        if (GameManager.Instance.IsPaused()) {

            LocoMotionController.SetActive(false);
        }

        if (GameManager.Instance.IsPlaying()) {

            LocoMotionController.SetActive(true);
        }


    }



  
}
