using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMoveStore : MonoBehaviour
{
    public int sceneBuildIndex;
    public Vector2 newSpawnPosition; // Set this in the Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Save spawn position so we can return to it later
            PlayerPrefs.SetFloat("SpawnX", newSpawnPosition.x);
            PlayerPrefs.SetFloat("SpawnY", newSpawnPosition.y);
            PlayerPrefs.Save();

            Debug.Log("Saved spawn position: " + newSpawnPosition);
            SceneManager.LoadScene(sceneBuildIndex);
        }
    }
}
