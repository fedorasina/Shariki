using UnityEngine;
using UnityEngine.Events;

namespace Platformers
{
    /// @brief Interactive lever that controls moving platforms
    /// @details Implements a lever mechanism that can be activated by players to control
    /// one or more moving platforms. Supports one-time use, initial state configuration,
    /// and custom events on activation/deactivation.
    public class Lever : MonoBehaviour
    {
        [Header("Lever Settings")]
        /// @brief Transform representing the physical lever handle
        public Transform leverHandle;
        /// @brief Angle the lever rotates when activated
        public float activationAngle = 45f;
        /// @brief Key used to interact with the lever
        public KeyCode activationKey = KeyCode.E;
        /// @brief Whether the lever can only be used once
        public bool oneTimeUse = false;
        /// @brief Whether the lever starts in activated state
        public bool startActivated = false;

        [Header("Platform Control")]
        /// @brief Array of platforms controlled by this lever
        public MovingPlatform[] controlledPlatforms;

        [Header("Events")]
        /// @brief Event triggered when lever is activated
        public UnityEvent onActivate;
        /// @brief Event triggered when lever is deactivated
        public UnityEvent onDeactivate;

        private bool canActivate = false;
        private bool isActivated;

        /// @brief Initializes lever state and handle position
        private void Start()
        {
            isActivated = startActivated;
            if (leverHandle != null && startActivated)
            {
                leverHandle.localRotation = Quaternion.Euler(activationAngle, 0, 0);
            }
        }

        /// @brief Handles player input for lever activation
        private void Update()
        {
            if (canActivate && Input.GetKeyDown(activationKey))
            {
                if (!oneTimeUse || (oneTimeUse && !isActivated))
                {
                    ToggleLever();
                }
            }
        }

        /// @brief Toggles lever state and updates controlled platforms
        /// @details Rotates lever handle, updates platform states, and triggers events
        private void ToggleLever()
        {
            isActivated = !isActivated;

            // Rotate lever handle
            if (leverHandle != null)
            {
                leverHandle.localRotation = Quaternion.Euler(isActivated ? activationAngle : 0, 0, 0);
            }

            // Activate/deactivate platforms
            foreach (MovingPlatform platform in controlledPlatforms)
            {
                if (platform != null)
                {
                    if (isActivated)
                    {
                        platform.ActivatePlatform();
                    }
                    else
                    {
                        platform.DeactivatePlatform();
                    }
                }
            }

            // Trigger events
            if (isActivated)
            {
                onActivate?.Invoke();
            }
            else
            {
                onDeactivate?.Invoke();
            }
        }

        /// @brief Handles player entering lever interaction zone
        /// @param other The collider that entered the trigger zone
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                canActivate = true;
            }
        }

        /// @brief Handles player leaving lever interaction zone
        /// @param other The collider that exited the trigger zone
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                canActivate = false;
            }
        }

        /// @brief Draws debug visualization of connections to controlled platforms
        private void OnDrawGizmos()
        {
            // Draw lines to controlled platforms
            if (controlledPlatforms != null)
            {
                Gizmos.color = Color.yellow;
                foreach (MovingPlatform platform in controlledPlatforms)
                {
                    if (platform != null)
                    {
                        Gizmos.DrawLine(transform.position, platform.transform.position);
                    }
                }
            }
        }
    }
}
