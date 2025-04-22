using UnityEngine;

public class CheckoutPersistence : MonoBehaviour
{
    private static CheckoutPersistence _instance;

    void Awake()
    {
      
        if (_instance != null && _instance != this)
        {
            Debug.Log("Duplicate CheckoutPersistence object destroyed: " + gameObject.name);
            Destroy(gameObject);
            return;
        }

     
        _instance = this;

    
        DontDestroyOnLoad(gameObject);

        Debug.Log("CheckoutPersistence instance created: " + gameObject.name);
    }
}