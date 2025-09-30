using System;
using TMPro;
using Unity.Barracuda;
using UnityEngine;

public class ProgressDisplay : MonoBehaviour
{
    [SerializeField] private Transform playerEyes;
    public Dog dog;
    public PlayerDog playerDog;

    public ParticleSystem winEffect1;
    public ParticleSystem winEffect2;
    public AudioSource soundEffect;


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
            if (dog.sheepsInGoal >= GameManager.Instance.SheepCount)
            {
                winEffect1.Play();
                winEffect2.Play();
                soundEffect.Play();
                dog.sheepsInGoal = 0;
                Debug.Log("Firework! opponent wins");
                gameObject.SetActive(false);
            }
        }
        else if (playerDog != null && !dog.gameObject.activeSelf)
        {
            textMesh.text = $"{playerDog.sheepsInGoal}/{GameManager.Instance.SheepCount}";
            if (playerDog.sheepsInGoal >= GameManager.Instance.SheepCount)
            {
                winEffect1.Play();
                winEffect2.Play();
                soundEffect.Play();
                playerDog.sheepsInGoal = 0;
                Debug.Log("Firework! Player wins");
                gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.Log("ProgressDisplay: No dog or playerDog assigned.");
        }
    }
}
