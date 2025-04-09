using UnityEngine;

public class PlayerPrefReset : MonoBehaviour
{
    private void Awake()
    {
        // Debug: Confirm the script execution
        Debug.Log("PlayerPrefsResetter is executing.");

        // Reset specific player preferences related to the player's position
        PlayerPrefs.DeleteKey("SpawnX");
        PlayerPrefs.DeleteKey("SpawnY");
        PlayerPrefs.Save();

        // Debug: Confirm the PlayerPrefs reset
        Debug.Log("PlayerPrefs for spawn position have been reset.");
    }
}
