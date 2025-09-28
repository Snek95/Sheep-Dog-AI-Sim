using System;
using TMPro;
using UnityEngine;

public class ProgressDisplay : MonoBehaviour
{
    [SerializeField] private Transform playerEyes;
    public Dog dog;
    public PlayerDog playerDog;


    private TextMeshPro textMesh;

    private void Start()
    {
        textMesh = GetComponent<TextMeshPro>();
        GameManager.Instance.OnStateChanged += GM_OnStateChanged;
        gameObject.SetActive(false);
    }

    private void GM_OnStateChanged(object sender, EventArgs e)
    {
        if ( GameManager.Instance.IsPlayingFP() || GameManager.Instance.IsPlayingTP())
        {
            gameObject.SetActive(true);
        } else
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(new Vector3(playerEyes.position.x, transform.position.y, playerEyes.position.z));
        transform.Rotate(0, 180, 0);
        if (dog != null && dog.isOpponentDog)
        {
            textMesh.text = $"{dog.sheepsInGoal}/{GameManager.Instance.SheepCount}";
        }
        else if (playerDog != null && !dog.gameObject.activeSelf)
        {
            textMesh.text = $"{playerDog.sheepsInGoal}/{GameManager.Instance.SheepCount}";
        }
        else
        {
            Debug.Log("ProgressDisplay: No dog or playerDog assigned.");
        }

    }
}
