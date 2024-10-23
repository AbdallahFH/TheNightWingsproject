using UnityEngine;

public class CameraController : MonoBehaviour
{  
    public Transform playerTransform; // Reference to the player character's transform
    public float cameraDistance = 5f;
    public float cameraHeight = 2f;
    public float cameraRotationSpeed = 5f;

    [Header("Mouse Settings")]
    public float mouseSensitivity = 100f;
    private float mouseX;
    private float mouseY;
    private float cameraYaw = 0f;
    private float cameraPitch = 0f;

    private Vector3 offset; // To maintain smooth camera follow

    void Start()
    {
        // Initialize the offset based on the initial position of the camera relative to the player
        offset = new Vector3(0, cameraHeight, -cameraDistance);

        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        HandleCameraRotation();
        MoveCamera();
    }

    void HandleCameraRotation()
    {
        // Get mouse input for camera rotation
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Update the yaw (horizontal rotation) based on mouse input
        cameraYaw += mouseX;
        cameraPitch += mouseY;
        // Apply rotation around the Y axis to the camera
        transform.rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0f);
    }

    void MoveCamera()
    {
        // Smoothly update the camera's position behind the player
        Vector3 targetPosition = playerTransform.position + transform.rotation * offset;
        transform.position = targetPosition;
    }
}