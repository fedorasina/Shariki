using UnityEngine;

/**
 * @brief Enables keyboard-controlled rotation of an object
 * 
 * This component allows rotating an object around its X, Y, and Z axes using keyboard input
 */
public class RotateOnXZWithKeys : MonoBehaviour
{
    public float rotationSpeed = 100f; // Rotation speed

    /**
     * @brief Processes keyboard input and applies rotation
     * 
     * - W/S keys: Rotate around X axis
     * - A/D keys: Rotate around Z axis
     * - Q/E keys: Rotate around Y axis
     * 
     * All rotations are applied in world space
     */
    private void Update()
    {
        float rotationX = Input.GetAxis("Vertical") * rotationSpeed * Time.deltaTime;    // W/S
        float rotationZ = -Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime; // A/D

        float rotationY = 0f;
        if (Input.GetKey(KeyCode.Q))
        {
            rotationY = -rotationSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            rotationY = rotationSpeed * Time.deltaTime;
        }

        transform.Rotate(rotationX, rotationY, rotationZ, Space.World);
    }
}
