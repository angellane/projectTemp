using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            float x = PlayerPrefs.GetFloat("SpawnX", 0);
            float y = PlayerPrefs.GetFloat("SpawnY", 0);
            Vector2 spawnPosition = new Vector2(x, y);

            Debug.Log("Moving player to stored spawn position: " + spawnPosition);
            player.transform.position = spawnPosition;
        }
    }
}
