using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class MainMenu : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Assign in Inspector if using VideoPlayer

    void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoEnd; // Optional: If you want an outro
            videoPlayer.Play();
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("YourGameScene"); // Replace with your game scene name
    }

    public void OpenOptions()
    {
        // Add options menu logic (e.g., open a submenu)
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // Optional: If video should trigger scene change after playing
    void OnVideoEnd(VideoPlayer vp)
    {
        //SceneManager.LoadScene("YourGameScene");
    }
}