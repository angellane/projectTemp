using UnityEngine;

public class SpawnManagerStore : MonoBehaviour
{
    public Vector2 defaultSpawnPosition = new Vector2(0, 0); // Set in Inspector

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // Debug: Check PlayerPrefs values before setting spawn
            Debug.Log("Checking PlayerPrefs before setting spawn...");
            Debug.Log("Stored Spawn Position: " + PlayerPrefs.GetFloat("SpawnX") + ", " + PlayerPrefs.GetFloat("SpawnY"));

            // First game start: Reset position
            if (!PlayerPrefs.HasKey("GameStarted"))
            {
                Debug.Log("First time playing, setting default spawn.");
                PlayerPrefs.SetFloat("SpawnX", defaultSpawnPosition.x);
                PlayerPrefs.SetFloat("SpawnY", defaultSpawnPosition.y);
                PlayerPrefs.SetInt("GameStarted", 1);
                PlayerPrefs.Save();
            }

            // Load last saved position
            float x = PlayerPrefs.GetFloat("SpawnX", defaultSpawnPosition.x);
            float y = PlayerPrefs.GetFloat("SpawnY", defaultSpawnPosition.y);
            Vector2 spawnPosition = new Vector2(x, y);

            Debug.Log("Moving player to: " + spawnPosition);
            player.transform.position = spawnPosition;
        }
    }
}
