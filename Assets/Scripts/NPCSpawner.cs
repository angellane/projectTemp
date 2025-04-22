using UnityEngine;
using System.Collections.Generic;

public class NPCSpawner : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _npcPrefabs;

    [SerializeField]
    private float _minimumSpawnTime;

    [SerializeField]
    private float _maximumSpawnTime;

    [SerializeField]
    private int _maxNPCs = 10;

    private float _timeUntilSpawn;
    private List<GameObject> _spawnedNPCs = new List<GameObject>(); 

    void Awake()
    {
        SetTimeUntilSpawn();
    }

    void Update()
    {
   
        CleanUpDestroyedNPCs();

    
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

    
        GameObject randomPrefab = _npcPrefabs[Random.Range(0, _npcPrefabs.Count)];

   
        GameObject npc = Instantiate(randomPrefab, transform.position, Quaternion.identity);

        NPCPersistence npcPersistence = npc.GetComponent<NPCPersistence>();
        if (npcPersistence != null)
        {
            npcPersistence.SetMaxNPCs(_maxNPCs);
        }

   
        _spawnedNPCs.Add(npc);
    }

 
    private void CleanUpDestroyedNPCs()
    {
 
        for (int i = _spawnedNPCs.Count - 1; i >= 0; i--)
        {
            if (_spawnedNPCs[i] == null)
            {
                _spawnedNPCs.RemoveAt(i);
            }
        }
    }
}