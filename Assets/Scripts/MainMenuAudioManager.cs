using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public AudioClip menuMusic; // Assign in Inspector
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = menuMusic;
        audioSource.loop = true;
        audioSource.Play();
    }

    // Call this to change music later (e.g., after clicking "Play")
    public void ChangeMusic(AudioClip newClip)
    {
        audioSource.Stop();
        audioSource.clip = newClip;
        audioSource.Play();
    }
}