using UnityEngine;

public class CheckoutPersistence : MonoBehaviour
{
    private static CheckoutPersistence _instance; // Singleton instance

    void Awake()
    {
        // If an instance already exists and it's not this one, destroy this object
        if (_instance != null && _instance != this)
        {
            Debug.Log("Duplicate CheckoutPersistence object destroyed: " + gameObject.name);
            Destroy(gameObject); // Destroy duplicate
            return;
        }

        // Set this as the singleton instance
        _instance = this;

        // Persist this object across scene changes
        DontDestroyOnLoad(gameObject);

        Debug.Log("CheckoutPersistence instance created: " + gameObject.name);
    }
}