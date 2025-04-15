using System.Collections;
using TMPro;
using UnityEngine;

public class NPC_AI : MonoBehaviour
{
    private enum State { Roaming, Talking }

    [Header("Movement")]
    [SerializeField] private NPCPathfinding pathfinding;
    [SerializeField] private float roamChangeInterval = 2f;
    [SerializeField] private float roamRange = 5f;

    [Header("Dialogue")]
    [SerializeField] private float displayTime = 3f;
    [SerializeField] private string[] dialogueOptions = {
        "Thank you!",
        "I Love This Drink!",
        "This is why she took the kids"
    };

    [Header("Text Settings")]
    [SerializeField] private float textVerticalOffset = 0.5f; // Height above NPC (0.5 works for 0.6 tall NPC)
    [SerializeField] private float textScale = 0.5f; // Overall size multiplier
    [SerializeField] private int fontSize = 10; // Base font size
    [SerializeField] private Color textColor = Color.black;

    private TextMeshPro speechText;
    private State state;

    private void Start()
    {
        CreateSpeechText();
        state = State.Roaming;
        StartCoroutine(RoamingRoutine());
    }

    private void CreateSpeechText()
    {
        // Destroy old text if it exists
        TextMeshPro existingText = GetComponentInChildren<TextMeshPro>();
        if (existingText != null)
            Destroy(existingText.gameObject);

        // Create new text object with proper scaling
        GameObject textObj = new GameObject("SpeechText");
        textObj.transform.SetParent(transform);
        textObj.transform.localPosition = new Vector3(0, textVerticalOffset, 0);
        textObj.transform.localScale = Vector3.one * textScale;

        // Configure TextMeshPro
        speechText = textObj.AddComponent<TextMeshPro>();
        speechText.text = "";
        speechText.fontSize = fontSize;
        speechText.alignment = TextAlignmentOptions.Center;
        speechText.color = textColor;
        speechText.fontStyle = FontStyles.Bold;
        speechText.enableWordWrapping = false;
        
        // Critical rendering settings
        var renderer = speechText.GetComponent<MeshRenderer>();
        renderer.sortingOrder = 9999;
        renderer.sortingLayerName = "UI";

        textObj.SetActive(false);
    }

    private void LateUpdate()
    {
        if (speechText != null && speechText.gameObject.activeSelf)
        {
            // Face camera while staying upright
            speechText.transform.rotation = Quaternion.LookRotation(
                Camera.main.transform.forward,
                Vector3.up
            );
        }
    }

    IEnumerator RoamingRoutine()
    {
        while (state == State.Roaming)
        {
            if (pathfinding != null)
            {
                Vector2 roamPos = (Vector2)transform.position + Random.insideUnitCircle * roamRange;
                pathfinding.MoveTo(roamPos);
            }
            yield return new WaitForSeconds(roamChangeInterval);
        }
    }

    public void TriggerDialogue()
    {
        if (state != State.Talking)
            StartCoroutine(TalkRoutine());
    }

    IEnumerator TalkRoutine()
    {
        state = State.Talking;
        
        if (pathfinding != null)
            pathfinding.StopMoving();

        if (speechText != null && dialogueOptions.Length > 0)
        {
            string randomLine = dialogueOptions[Random.Range(0, dialogueOptions.Length)];
            speechText.text = randomLine;
            speechText.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(displayTime);

        if (speechText != null)
            speechText.gameObject.SetActive(false);

        state = State.Roaming;
        
        if (pathfinding != null)
            pathfinding.ResumeMoving();
    }
}