using UnityEngine;

public class RestockPersistence : MonoBehaviour
{
    private static RestockPersistence _instance;

    void Awake()
    {

        if (_instance != null && _instance != this)
        {
            Debug.Log("Duplicate RestockPersistence object destroyed: " + gameObject.name);
            Destroy(gameObject);
            return;
        }


        _instance = this;

  
        DontDestroyOnLoad(gameObject);

        Debug.Log("RestockPersistence instance created: " + gameObject.name);
    }
}