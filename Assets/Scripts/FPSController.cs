using UnityEngine;

/**
 * @brief First Person Shooter character controller
 * 
 * This class handles movement and camera control for a first-person character
 */
public class FPSController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float mouseSensitivity = 2f;
    public Transform cameraTransform;

    private Rigidbody rb;
    private Vector3 movement;
    private float rotationX = 0f;

    /**
     * @brief Initializes the controller
     * 
     * Gets the Rigidbody component and locks the cursor to the center of the screen
     */
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    /**
     * @brief Handles input and camera rotation
     * 
     * Processes mouse and keyboard input for camera rotation and movement direction
     */
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        
        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.Rotate(Vector3.up * mouseX);
        
        movement.x = Input.GetAxis("Horizontal");
        movement.z = Input.GetAxis("Vertical");
    }

    /**
     * @brief Applies physics-based movement
     * 
     * Translates input direction into movement based on the character's orientation
     * and applies it to the Rigidbody
     */
    void FixedUpdate()
    {
        Vector3 moveDirection = transform.right * movement.x + transform.forward * movement.z;
        rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);
    }
}
