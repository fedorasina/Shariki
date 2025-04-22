using UnityEngine;

public class RotateOnXZWithKeys : MonoBehaviour
{
    public float rotationSpeed = 100f; // Скорость вращения

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
