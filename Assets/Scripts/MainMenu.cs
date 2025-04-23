using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class MainMenu : MonoBehaviour
{
    public VideoPlayer videoPlayer; 

    void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoEnd;
            videoPlayer.Play();
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("MainScene"); 
    }

    public void OpenOptions()
    {
   
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

  
    void OnVideoEnd(VideoPlayer vp)
    {
      
    }
}