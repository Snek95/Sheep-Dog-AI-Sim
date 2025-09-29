using UnityEngine;


public class MenuManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.OnStateChanged += GM_OnStateChanged;
    }

    private void GM_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsPlayingTP() || GameManager.Instance.IsPlayingFP())
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}
