using UnityEngine;

public class NPCPersistence : MonoBehaviour
{
    private int _maxNPCs;

    public void SetMaxNPCs(int max)
    {
        _maxNPCs = max;
    }

    void Start()
    {

        NPCPersistence[] allNPCs = FindObjectsByType<NPCPersistence>(FindObjectsSortMode.None);

        if (allNPCs.Length > _maxNPCs)
        {
            Debug.Log($"Too many NPCs in the scene. Destroying excess NPC: {gameObject.name}");
            Destroy(gameObject);
            return;
        }


        DontDestroyOnLoad(gameObject);

        Debug.Log($"NPC persisted: {gameObject.name}. Total NPCs: {allNPCs.Length}");
    }
}