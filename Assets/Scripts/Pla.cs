using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float interactRange = 2f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private LayerMask npcLayer;

    private void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            TryInteractWithNPC();
        }
    }

    private void TryInteractWithNPC()
    {
        Collider2D[] npcs = Physics2D.OverlapCircleAll(
            transform.position, 
            interactRange, 
            npcLayer
        );

        foreach (Collider2D npc in npcs)
        {
            NPC_AI npcAI = npc.GetComponent<NPC_AI>();
            if (npcAI != null)
            {
                npcAI.TriggerDialogue();
                break; // Interact with only one NPC
            }
        }
    }

    // Visualize interaction range in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}