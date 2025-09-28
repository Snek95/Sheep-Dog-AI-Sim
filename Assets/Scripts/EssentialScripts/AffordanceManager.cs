using System;
using UnityEngine;

public class AffordanceManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.OnStateChanged += GM_OnStateChanged;
        gameObject.SetActive(false);
    }

    private void GM_OnStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsPlayingTP())
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

}
