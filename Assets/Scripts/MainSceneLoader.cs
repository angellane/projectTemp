using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class AsyncSceneLoader : MonoBehaviour
{
    public GameObject loadingScreen; // Assign the LoadingScreen panel
    public Slider progressBar;      // Assign the ProgressBar slider
    public string sceneName = "MainScene"; // Set your target scene name

    public void LoadSceneAsync()
    {
        StartCoroutine(LoadSceneCoroutine());
    }

    IEnumerator LoadSceneCoroutine()
    {
        // Show loading screen
        loadingScreen.SetActive(true);

        // Reset progress bar
        progressBar.value = 0;

        // Start loading the scene asynchronously
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        // Don't allow the scene to activate immediately
        operation.allowSceneActivation = false;

        // Track loading progress
        while (!operation.isDone)
        {
            // Progress is reported as 0-0.9 (for 0%-90%)
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.value = progress;

            // When loading is almost complete (90%+), allow activation
            if (progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null; // Wait a frame
        }
    }
}