using UnityEngine;

public class NPCPersistence : MonoBehaviour
{
    private int _maxNPCs; // Maximum number of NPCs allowed

    public void SetMaxNPCs(int max)
    {
        _maxNPCs = max;
    }

    void Start()
    {
        // Find all NPCs in the scene using the new method
        NPCPersistence[] allNPCs = FindObjectsByType<NPCPersistence>(FindObjectsSortMode.None);

        // Check if the number of NPCs exceeds the maximum limit
        if (allNPCs.Length > _maxNPCs)
        {
            Debug.Log($"Too many NPCs in the scene. Destroying excess NPC: {gameObject.name}");
            Destroy(gameObject); // Destroy this NPC
            return;
        }

        // Persist this NPC across scene changes
        DontDestroyOnLoad(gameObject);

        Debug.Log($"NPC persisted: {gameObject.name}. Total NPCs: {allNPCs.Length}");
    }
}