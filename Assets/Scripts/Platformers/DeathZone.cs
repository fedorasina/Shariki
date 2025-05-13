using UnityEngine;

namespace Platformers 
{
    /// @brief Represents a hazardous area that causes player death/respawn
    /// @details Creates a trigger zone that respawns the player at their last checkpoint when entered.
    /// Requires a Collider component and depends on CoinGameManager for respawn functionality.
    public class DeathZone : MonoBehaviour 
    {
        /// @brief Initializes the death zone's collider settings
        /// @details Ensures the collider is set as a trigger
        private void Start()
        {
            // Ensure the collider is a trigger if it's not already
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = true;
            }
            else
            {
                Debug.LogError("DeathZone requires a Collider component.", this);
            }
        }

        /// @brief Handles player entry into the death zone
        /// @param other The collider that entered the trigger zone
        /// @details Triggers player respawn through CoinGameManager when player enters
        private void OnTriggerEnter(Collider other) 
        {
            if (other.CompareTag("Player")) 
            {
                if (CoinGameManager.Instance != null) 
                {
                    CoinGameManager.Instance.RespawnPlayer();
                    // Optionally show a message
                    // CoinGameManager.Instance.ShowMessage("You fell!", 2f);
                }
                else 
                {
                    Debug.LogError("CoinGameManager not found. Cannot respawn player.");
                    // Fallback: Reload scene if CoinGameManager is missing (though GM should be persistent)
                    // UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                }
            }
        }
    }
} 