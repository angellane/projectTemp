using UnityEngine;
using UnityEngine.SceneManagement;

public class HideNPCs : MonoBehaviour
{
    private GameObject[] npcs;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        npcs = GameObject.FindGameObjectsWithTag("NPC");

        if (scene.buildIndex == 1)
        {
            foreach (var npc in npcs)
            {
                SetNPCVisibility(npc, true);
            }
        }
        else
        {
            foreach (var npc in npcs)
            {
                SetNPCVisibility(npc, false);
            }
        }
    }

    void SetNPCVisibility(GameObject npc, bool isVisible)
    {

        Renderer npcRenderer = npc.GetComponent<Renderer>();
        if (npcRenderer != null)
        {
            npcRenderer.enabled = isVisible; 
        }
    }

    void Start()
    {

    }
}
