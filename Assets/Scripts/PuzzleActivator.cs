using UnityEngine;

public class PuzzleActivator : MonoBehaviour
{
    public PuzzleSystem puzzleSystem;
    public float activationDistance = 3f;
    public KeyCode activationKey = KeyCode.F;

    private Camera playerCamera;

    void Start()
    {
        playerCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(activationKey))
        {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, activationDistance))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    Debug.Log("[Puzzle] Activator pressed - activating puzzle");
                    puzzleSystem.ActivatePuzzle();
                }
            }
        }
    }
}