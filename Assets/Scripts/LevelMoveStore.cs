using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMoveStore : MonoBehaviour
{
    public int sceneBuildIndex;
    public Vector2 newSpawnPosition;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
     
            PlayerPrefs.SetFloat("SpawnX", newSpawnPosition.x);
            PlayerPrefs.SetFloat("SpawnY", newSpawnPosition.y);
            PlayerPrefs.Save();

            Debug.Log("Saved spawn position: " + newSpawnPosition);
            SceneManager.LoadScene(sceneBuildIndex);
        }
    }
}
