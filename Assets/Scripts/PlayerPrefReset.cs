using UnityEngine;

public class PlayerPrefReset : MonoBehaviour
{
    private void Awake()
    {

        Debug.Log("PlayerPrefsResetter is executing.");


        PlayerPrefs.DeleteKey("SpawnX");
        PlayerPrefs.DeleteKey("SpawnY");
        PlayerPrefs.Save();


        Debug.Log("PlayerPrefs for spawn position have been reset.");
    }
}
