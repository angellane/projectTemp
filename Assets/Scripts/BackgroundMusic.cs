using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Music Tracks")]
    public AudioClip mainSceneTrack;
    public AudioClip storageSceneTrack;

    private AudioSource audioSource;
    private const float TARGET_VOLUME = 0.05f; // alter number for volume change

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;

            audioSource = GetComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.volume = TARGET_VOLUME; // permanent volume
            audioSource.clip = mainSceneTrack;
            audioSource.Play();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // saves music timestamp on transition
        float currentTime = audioSource.time;

        if (scene.name == "MainScene")
        {
            if (audioSource.clip != mainSceneTrack)
            {
                audioSource.clip = mainSceneTrack;
                audioSource.time = currentTime % mainSceneTrack.length;
                audioSource.Play();
            }
        }
        else if (scene.name == "StorageScene")
        {
            if (audioSource.clip != storageSceneTrack)
            {
                audioSource.clip = storageSceneTrack;
                audioSource.time = currentTime % storageSceneTrack.length;
                audioSource.Play();
            }
        }
    }
}