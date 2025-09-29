using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class TimedSoundPlayer : MonoBehaviour {
    [Tooltip("Liste der Sounds, die abgespielt werden können")]
    public List<AudioClip> soundClips;

    [Tooltip("Minimales Intervall zwischen den Sounds (in Sekunden)")]
    public float minInterval = 3f;

    [Tooltip("Maximales Intervall zwischen den Sounds (in Sekunden)")]
    public float maxInterval = 5f;

    private AudioSource audioSource;
    private float timer;

    void Start() {
        audioSource = GetComponent<AudioSource>();
        ResetTimer();
    }

    void Update() {
        timer -= Time.deltaTime;

        if (timer <= 0f && soundClips.Count > 0) {
            PlayRandomSound();
            ResetTimer();
        }
    }

    private void PlayRandomSound() {
        int index = Random.Range(0, soundClips.Count);
        audioSource.PlayOneShot(soundClips[index]);
    }

    private void ResetTimer() {
        timer = Random.Range(minInterval, maxInterval);
    }
}