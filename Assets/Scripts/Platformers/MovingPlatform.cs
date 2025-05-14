using UnityEngine;

namespace Platformers
{
    /// @brief Implements a platform that moves between two points
    /// @details Creates a platform that can move between a start and target point,
    /// with options for continuous movement and player attachment.
    /// Provides visual debugging tools and supports both standard meshes and Unity plane objects.
    public class MovingPlatform : MonoBehaviour
    {
        [Header("Movement Settings")]
        /// @brief Target position the platform moves to
        public Transform targetPoint;
        /// @brief Movement speed in units per second
        public float speed = 2f;
        /// @brief Whether the platform starts moving immediately
        public bool moveOnStart = false;
        /// @brief Whether the platform should continuously move back and forth
        public bool pingPong = true;

        [Header("Platform Settings")]
        /// @brief Whether players should stick to the platform while moving
        public bool attachPlayer = true;

        private Vector3 startPoint;
        private Vector3 currentTarget;
        private bool isActivated = false;
        private Transform attachedPlayer = null;

        /// @brief Initializes platform position and movement state
        private void Start()
        {
            startPoint = transform.position;
            if (targetPoint != null)
            {
                currentTarget = targetPoint.position;
            }
            
            if (moveOnStart)
            {
                ActivatePlatform();
            }
        }

        /// @brief Handles platform movement and target switching
        private void Update()
        {
            if (isActivated && targetPoint != null)
            {
                // Move platform
                transform.position = Vector3.MoveTowards(transform.position, currentTarget, speed * Time.deltaTime);

                // Check if reached target
                if (Vector3.Distance(transform.position, currentTarget) < 0.01f)
                {
                    if (pingPong)
                    {
                        // Switch target between start and end points
                        currentTarget = (currentTarget == targetPoint.position) ? startPoint : targetPoint.position;
                    }
                    else
                    {
                        isActivated = false;
                    }
                }
            }
        }

        /// @brief Handles player attachment to platform
        /// @param other The collider that entered the platform's trigger
        private void OnTriggerEnter(Collider other)
        {
            if (attachPlayer && other.CompareTag("Player"))
            {
                attachedPlayer = other.transform;
                other.transform.SetParent(transform);
            }
        }

        /// @brief Handles player detachment from platform
        /// @param other The collider that exited the platform's trigger
        private void OnTriggerExit(Collider other)
        {
            if (attachPlayer && other.CompareTag("Player"))
            {
                if (other.transform == attachedPlayer)
                {
                    attachedPlayer.SetParent(null);
                    attachedPlayer = null;
                }
            }
        }

        /// @brief Activates platform movement towards target point
        public void ActivatePlatform()
        {
            if (targetPoint != null)
            {
                isActivated = true;
                currentTarget = targetPoint.position;
            }
        }

        /// @brief Deactivates platform movement and returns to start if pingPong is true
        public void DeactivatePlatform()
        {
            isActivated = false;
            if (pingPong)
            {
                currentTarget = startPoint;
            }
        }

        /// @brief Draws debug visualization of the platform's path and target position
        /// @details Shows movement path, direction indicators, and platform size at target
        private void OnDrawGizmos()
        {
            if (targetPoint != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, targetPoint.position);

                Vector3 size;
                bool isPlane = GetComponent<MeshFilter>()?.sharedMesh?.name == "Plane";

                if (isPlane)
                {
                    float scaleX = transform.localScale.x * 10f;
                    float scaleZ = transform.localScale.z * 10f;
                    size = new Vector3(scaleX, 0.01f, scaleZ);
                }
                else
                {
                    size = transform.localScale;
                }

                Matrix4x4 rotationMatrix = Matrix4x4.TRS(
                    targetPoint.position,
                    transform.rotation,
                    Vector3.one
                );
                
                Gizmos.matrix = rotationMatrix;
                Gizmos.DrawWireCube(Vector3.zero, size);
                
                Gizmos.matrix = Matrix4x4.identity;

                Vector3 direction = (targetPoint.position - transform.position).normalized;
                Vector3 arrowPos = Vector3.Lerp(transform.position, targetPoint.position, 0.5f);
                float arrowSize = 0.5f;
                
                Vector3 right = Vector3.Cross(Vector3.up, direction).normalized;
                Vector3 arrowLeft = direction - right * 0.5f;
                Vector3 arrowRight = direction + right * 0.5f;
                
                Gizmos.DrawRay(arrowPos, arrowLeft * arrowSize);
                Gizmos.DrawRay(arrowPos, arrowRight * arrowSize);
            }
        }
    }
}
