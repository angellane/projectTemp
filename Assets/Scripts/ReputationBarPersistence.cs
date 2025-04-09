using UnityEngine;

public class ReputationBarPersistence : MonoBehaviour
{
    private static ReputationBarPersistence _instance; // Singleton instance for ReputationBar

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.Log("Duplicate ReputationBarPersistence object destroyed: " + gameObject.name);
            Destroy(gameObject); // Destroy duplicate
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log("ReputationBarPersistence instance created: " + gameObject.name);
    }
}