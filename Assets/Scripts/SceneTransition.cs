using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionTrigger : MonoBehaviour
{
    [SerializeField] private string sceneName = "StorageScene"; // Set this in Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Make sure your player has the "Player" tag
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}