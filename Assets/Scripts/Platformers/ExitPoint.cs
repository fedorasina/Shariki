using UnityEngine;
using UnityEngine.SceneManagement;

namespace Platformers
{
    /// @brief Represents a level exit point that transitions to the next level
    /// @details Manages level completion, coin collection requirements, and scene transitions.
    /// Provides visual debugging tools and customizable exit behavior.
    [RequireComponent(typeof(BoxCollider))]
    public class ExitPoint : MonoBehaviour
    {
        [Header("Exit Settings")]
        /// @brief Name of the next scene to load. Leave empty for final level
        public string nextLevelName; // Имя следующей сцены. Оставьте пустым для последнего уровня.
        /// @brief Whether this is the final level of the game
        public bool isLastLevel = false;
        /// @brief Message shown when level is completed
        public string completionMessage = "Level Complete!";
        /// @brief Message shown when player hasn't collected enough coins
        public string notEnoughCoinsMessage = "Collect all coins first!";

        private bool canExit = false;
        private bool showingMessage = false;
        private float messageDelay = 1f;
        private float lastMessageTime;

        /// @brief Initializes the exit point's collider settings
        private void Awake()
        {
            // Убедимся, что коллайдер является триггером
            BoxCollider boxCollider = GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                boxCollider.isTrigger = true;
            }
            else
            {
                Debug.LogError("ExitPoint requires a BoxCollider component.", this);
            }
        }

        /// @brief Handles player entry into the exit zone
        /// @param other The collider that entered the trigger zone
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                canExit = true;
                CheckAndHandleExit();
            }
        }

        /// @brief Handles player leaving the exit zone
        /// @param other The collider that exited the trigger zone
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                canExit = false;
                showingMessage = false;
            }
        }

        /// @brief Checks if player can exit and handles the exit logic
        /// @details Verifies coin collection requirements and shows appropriate messages
        private void CheckAndHandleExit()
        {
            if (!canExit) return;

            if (CoinGameManager.Instance != null)
            {
                if (CoinGameManager.Instance.AreAllCoinsCollected())
                {
                    HandleExit();
                }
                else if (!showingMessage && Time.time - lastMessageTime > messageDelay)
                {
                    showingMessage = true;
                    lastMessageTime = Time.time;
                    CoinGameManager.Instance.ShowMessage(notEnoughCoinsMessage);
                }
            }
            else
            {
                Debug.LogError("CoinGameManager not found in scene. Cannot handle exit logic.");
            }
        }

        /// @brief Processes the actual level exit and scene transition
        /// @details Handles both final level completion and next level loading
        private void HandleExit()
        {
            if (CoinGameManager.Instance != null)
            {
                if (isLastLevel || string.IsNullOrEmpty(nextLevelName))
                {
                    CoinGameManager.Instance.ShowMessage(completionMessage, 5f);
                    Debug.Log("Player reached the final exit of the game/demo.");
                }
                else
                {
                    if (SceneUtility.GetBuildIndexByScenePath("Assets/Scenes/Platformers/" + nextLevelName + ".unity") >= 0 || 
                        SceneUtility.GetBuildIndexByScenePath(nextLevelName) >= 0)
                    {
                        SceneManager.LoadScene(nextLevelName);
                    }
                    else
                    {
                        Debug.LogError($"Scene '{nextLevelName}' not found in Build Settings or path is incorrect.");
                        CoinGameManager.Instance.ShowMessage($"Error: Scene '{nextLevelName}' not found.", 5f);
                    }
                }
            }
        }

        /// @brief Draws debug visualization of the exit point in the Unity editor
        /// @details Shows exit location, direction, and decorative elements
        private void OnDrawGizmos()
        {
            // Visualization of the exit point in the editor
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
            
            // Show direction to the next level with an arrow
            Gizmos.DrawRay(transform.position, Vector3.up * 2f);
            
            // Draw a small crown above the exit point
            float crownSize = 0.3f;
            Vector3 crownBase = transform.position + Vector3.up * 2f;
            Gizmos.DrawLine(crownBase + Vector3.left * crownSize, crownBase + Vector3.up * crownSize);
            Gizmos.DrawLine(crownBase + Vector3.up * crownSize, crownBase + Vector3.right * crownSize);
            Gizmos.DrawLine(crownBase + Vector3.right * crownSize, crownBase);
            Gizmos.DrawLine(crownBase, crownBase + Vector3.left * crownSize);
        }
    }
} 