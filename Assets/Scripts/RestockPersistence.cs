using UnityEngine;

public class RestockPersistence : MonoBehaviour
{
    private static RestockPersistence _instance; // Singleton instance

    void Awake()
    {
        // Ensure only one instance of RestockPersistence exists
        if (_instance != null && _instance != this)
        {
            Debug.Log("Duplicate RestockPersistence object destroyed: " + gameObject.name);
            Destroy(gameObject); // Destroy duplicate
            return;
        }

        // Set this as the singleton instance
        _instance = this;

        // Persist this object across scene changes
        DontDestroyOnLoad(gameObject);

        Debug.Log("RestockPersistence instance created: " + gameObject.name);
    }
}