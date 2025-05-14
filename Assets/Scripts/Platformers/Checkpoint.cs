using UnityEngine;

namespace Platformers
{
    /// @brief Represents a checkpoint in the game where players can respawn
    /// @details This component creates a checkpoint area that saves the player's position when triggered.
    /// It includes visual debugging tools and customizable activation behavior.
    [RequireComponent(typeof(BoxCollider))]
    public class Checkpoint : MonoBehaviour
    {
        [Header("Checkpoint Settings")]
        /// @brief Whether the checkpoint can be activated
        public bool isActive = true;
        /// @brief If true, the checkpoint becomes inactive after first use
        public bool deactivateAfterUse = false;
        /// @brief Minimum time between checkpoint activations in seconds
        public float activationDelay = 0.5f; // Prevent multiple rapid activations
        
        [Header("Visual Settings")]
        /// @brief Color used to visualize active checkpoint in the editor
        public Color activeColor = new Color(0.3f, 1f, 0.3f, 0.5f); // Semi-transparent green
        /// @brief Color used to visualize inactive checkpoint in the editor
        public Color inactiveColor = new Color(0.7f, 0.7f, 0.7f, 0.5f); // Semi-transparent gray
        /// @brief Physical size of the checkpoint trigger area
        public Vector3 checkpointSize = new Vector3(3f, 0.1f, 3f); // Size of the checkpoint area

        private bool canActivate = true;
        private float lastActivationTime;

        /// @brief Initializes the checkpoint's collider
        /// @details Ensures the checkpoint has a properly configured trigger collider
        private void Start()
        {
            // Ensure we have a trigger collider
            BoxCollider boxCollider = GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                boxCollider.isTrigger = true;
                // Set the collider size to match our visual size
                boxCollider.size = checkpointSize;
            }
            else
            {
                Debug.LogError("Checkpoint requires a BoxCollider component!", this);
            }
        }

        /// @brief Handles player entry into the checkpoint area
        /// @param other The collider that entered the trigger zone
        private void OnTriggerEnter(Collider other)
        {
            if (!isActive || !canActivate) return;

            if (other.CompareTag("Player"))
            {
                if (Time.time - lastActivationTime >= activationDelay)
                {
                    SetCheckpoint();
                    lastActivationTime = Time.time;

                    if (deactivateAfterUse)
                    {
                        isActive = false;
                    }
                }
            }
        }

        /// @brief Saves the checkpoint position and resets player state
        /// @details Updates the game manager with the new checkpoint position and handles player physics
        private void SetCheckpoint()
        {
            if (CoinGameManager.Instance != null)
            {
                // Get the position slightly above the checkpoint to prevent ground clipping
                Vector3 spawnPosition = transform.position + Vector3.up * 0.5f;
                
                CoinGameManager.Instance.SetCheckpoint(spawnPosition);

                // If the player has a Rigidbody, we might want to reset their velocity when respawning
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    Rigidbody rb = player.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.linearVelocity = Vector3.zero;
                    }
                }
            }
            else
            {
                Debug.LogWarning("CoinGameManager not found! Checkpoint won't be saved.", this);
            }
        }

        /// @brief Draws debug visualization of the checkpoint in the Unity editor
        /// @details Visualizes the checkpoint area, activation state, and direction indicators
        private void OnDrawGizmos()
        {
            // Draw the checkpoint area
            Gizmos.color = isActive ? activeColor : inactiveColor;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(Vector3.zero, checkpointSize);

            // Draw arrows pointing upward to make it more visible
            Vector3 center = transform.position;
            float arrowSize = 1f;
            Vector3 arrowOffset = Vector3.up * (checkpointSize.y + 0.5f);

            Gizmos.color = isActive ? Color.green : Color.gray;
            
            // Draw main arrow
            Gizmos.DrawRay(center, arrowOffset);
            
            // Draw arrow head
            Vector3 arrowTop = center + arrowOffset;
            Vector3 right = transform.right * (arrowSize * 0.3f);
            Vector3 forward = transform.forward * (arrowSize * 0.3f);
            
            Gizmos.DrawLine(arrowTop, arrowTop - arrowOffset * 0.3f + right);
            Gizmos.DrawLine(arrowTop, arrowTop - arrowOffset * 0.3f - right);
            Gizmos.DrawLine(arrowTop, arrowTop - arrowOffset * 0.3f + forward);
            Gizmos.DrawLine(arrowTop, arrowTop - arrowOffset * 0.3f - forward);

            // Draw text label
            #if UNITY_EDITOR
            UnityEditor.Handles.Label(arrowTop + Vector3.up * 0.5f, 
                isActive ? "Active Checkpoint" : "Inactive Checkpoint");
            #endif
        }
    }
}
