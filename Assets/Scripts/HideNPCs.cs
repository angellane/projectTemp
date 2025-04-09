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

        if (scene.buildIndex == 0) // Check if the loaded scene is scene 0
        {
            foreach (var npc in npcs)
            {
                SetNPCVisibility(npc, true); // Make NPCs visible
            }
        }
        else
        {
            foreach (var npc in npcs)
            {
                SetNPCVisibility(npc, false); // Make NPCs invisible
            }
        }
    }

    void SetNPCVisibility(GameObject npc, bool isVisible)
    {
        // Get the Renderer component of the NPC
        Renderer npcRenderer = npc.GetComponent<Renderer>();
        if (npcRenderer != null)
        {
            npcRenderer.enabled = isVisible; // Enable or disable the renderer
        }
    }

    void Start()
    {
        // Initialization code if needed
    }
}
