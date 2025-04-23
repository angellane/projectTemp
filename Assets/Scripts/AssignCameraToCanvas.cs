using UnityEngine;

public class AssignCameraToCanvas : MonoBehaviour
{
    public Canvas canvas; // Assign your Canvas in the Inspector
    public Camera targetCamera;

    void Start()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (canvas != null && targetCamera != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = targetCamera;
            Debug.Log("Camera assigned to Canvas successfully!");
        }
        else
        {
            Debug.LogError("Canvas or Camera is not assigned!");
        }
    }
}