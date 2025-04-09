using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMove : MonoBehaviour
{
    public int sceneBuildIndex;
    public Vector2 newSpawnPosition; // Set in Inspector for returning position

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Save new spawn position when returning to main scene
            PlayerPrefs.SetFloat("SpawnX", newSpawnPosition.x);
            PlayerPrefs.SetFloat("SpawnY", newSpawnPosition.y);
            PlayerPrefs.Save();

            Debug.Log("Saved return spawn position: " + newSpawnPosition);

            SceneManager.LoadScene(sceneBuildIndex);
        }
    }
}
