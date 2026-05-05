using UnityEngine;

public class ModelInteraction : MonoBehaviour
{
    // How fast the model rotates and moves with the mouse
    public float Speed = 5;
    public Camera mainCamera;
    private float cameraZDistance;

    void Start()
    {
        // Grab the main camera and store how far the model is from it on the Z axis
        mainCamera = Camera.main;
        cameraZDistance = mainCamera.WorldToScreenPoint(transform.position).z;
    }

    void Update()
    {
        // Left click to rotate the model based on mouse movement
        if (Input.GetMouseButton(0))
        {
            transform.eulerAngles += Speed * new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
        }

        // Right click to move the model to follow the mouse position in world space
        if (Input.GetMouseButton(1))
        {
            // Keep the original Z distance from the camera when converting to world position
            Vector3 screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraZDistance);
            Vector3 newWorldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
            transform.position = newWorldPosition;
        }
    }
}