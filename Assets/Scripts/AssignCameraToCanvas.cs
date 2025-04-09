using UnityEngine;

public class AssignCameraToCanvas : MonoBehaviour
{
    public Canvas canvas; // Assign your Canvas in the Inspector
    public Camera targetCamera; // Assign your Camera in the Inspector (optional)

    void Start()
    {
        // If no target camera is assigned, use the main camera
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        // Ensure the canvas and camera are valid
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