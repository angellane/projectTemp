using UnityEngine;
using System.Collections.Generic;

public class NPCSpawner : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _npcPrefabs; // List of NPC prefabs to choose from

    [SerializeField]
    private float _minimumSpawnTime;

    [SerializeField]
    private float _maximumSpawnTime;

    [SerializeField]
    private int _maxNPCs = 10; // Maximum number of NPCs that can be spawned

    private float _timeUntilSpawn;
    private List<GameObject> _spawnedNPCs = new List<GameObject>(); // Track spawned NPCs

    void Awake()
    {
        SetTimeUntilSpawn();
    }

    void Update()
    {
        // Clean up destroyed NPCs from the list
        CleanUpDestroyedNPCs();

        // Only spawn if the maximum number of NPCs hasn't been reached
        if (_spawnedNPCs.Count < _maxNPCs)
        {
            _timeUntilSpawn -= Time.deltaTime;

            if (_timeUntilSpawn <= 0)
            {
                SpawnNPC();
                SetTimeUntilSpawn();
            }
        }
    }

    private void SetTimeUntilSpawn()
    {
        _timeUntilSpawn = Random.Range(_minimumSpawnTime, _maximumSpawnTime);
    }

    private void SpawnNPC()
    {
        if (_npcPrefabs == null || _npcPrefabs.Count == 0)
        {
            Debug.LogError("No NPC prefabs assigned to the spawner!");
            return;
        }

        // Randomly select a prefab from the list
        GameObject randomPrefab = _npcPrefabs[Random.Range(0, _npcPrefabs.Count)];

        // Instantiate the selected prefab
        GameObject npc = Instantiate(randomPrefab, transform.position, Quaternion.identity);

        // Pass the maxNPCs value to the NPC
        NPCPersistence npcPersistence = npc.GetComponent<NPCPersistence>();
        if (npcPersistence != null)
        {
            npcPersistence.SetMaxNPCs(_maxNPCs);
        }

        // Add the spawned NPC to the list
        _spawnedNPCs.Add(npc);
    }

    // Clean up destroyed NPCs from the list
    private void CleanUpDestroyedNPCs()
    {
        // Iterate through the list in reverse to avoid issues when removing items
        for (int i = _spawnedNPCs.Count - 1; i >= 0; i--)
        {
            if (_spawnedNPCs[i] == null) // Check if the NPC has been destroyed
            {
                _spawnedNPCs.RemoveAt(i); // Remove it from the list
            }
        }
    }
}