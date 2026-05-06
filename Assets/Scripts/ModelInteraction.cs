// Name: Paul Lewis Marcos
// Date: February 20, 2026
// Assignment: CS 152 Project - Programming Paradigms
// Description: Handles all 3D model interaction in the scene, including mouse-driven
//              rotation and panning, keyboard zoom (I/O), and a full transform reset (R).

using UnityEngine;

public class ModelInteraction : MonoBehaviour
{
    public float rotateSpeed = 5f;
    public float zoomSpeed = 5f;
    public Camera mainCamera;
    private float cameraZDistance;

    // Set to false externally (e.g. by OpacityController) to lock rotation
    public bool canRotate = true;

    private Vector3 initialModelPosition;
    private Quaternion initialModelRotation;
    private Vector3 initialCameraPosition;
    private Quaternion initialCameraRotation;

    void Start()
    {
        mainCamera = Camera.main;
        cameraZDistance = mainCamera.WorldToScreenPoint(transform.position).z;

        initialModelPosition = transform.position;
        initialModelRotation = transform.rotation;
        initialCameraPosition = mainCamera.transform.position;
        initialCameraRotation = mainCamera.transform.rotation;
    }

    void Update()
    {
        // Left click: full 360-degree rotation on all axes without gimbal lock
        if (canRotate && Input.GetMouseButton(0))
        {
            transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * rotateSpeed, Space.World);
            transform.Rotate(Vector3.right, -Input.GetAxis("Mouse Y") * rotateSpeed, Space.World);
        }

        // Right click: pan the model to follow the mouse in world space
        if (Input.GetMouseButton(1))
        {
            Vector3 screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraZDistance);
            transform.position = mainCamera.ScreenToWorldPoint(screenPosition);
        }

        // 9 to zoom in, 0 to zoom out
        if (Input.GetKey(KeyCode.Alpha9))
        {
            mainCamera.transform.position += mainCamera.transform.forward * zoomSpeed * Time.deltaTime;
            cameraZDistance = mainCamera.WorldToScreenPoint(transform.position).z;
        }
        if (Input.GetKey(KeyCode.Alpha0))
        {
            mainCamera.transform.position -= mainCamera.transform.forward * zoomSpeed * Time.deltaTime;
            cameraZDistance = mainCamera.WorldToScreenPoint(transform.position).z;
        }

        // R to reset model and camera to their starting state
        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.position = initialModelPosition;
            transform.rotation = initialModelRotation;
            mainCamera.transform.position = initialCameraPosition;
            mainCamera.transform.rotation = initialCameraRotation;
            cameraZDistance = mainCamera.WorldToScreenPoint(transform.position).z;
        }
    }
}