using UnityEngine;

namespace Platformers
{
    /// @brief Represents a collectible coin in the game
    /// @details Implements coin behavior including rotation, bobbing motion, and collection mechanics.
    /// Provides visual debugging tools and customizable collection behavior.
    public class Coin : MonoBehaviour
    {
        [Header("Coin Settings")]
        /// @brief Value of the coin when collected
        public int value = 1;
        /// @brief Speed at which the coin rotates in degrees per second
        public float rotationSpeed = 100f;
        /// @brief Speed of the up and down bobbing motion
        public float bobSpeed = 1f;
        /// @brief Maximum height of the bobbing motion
        public float bobHeight = 0.5f;
        /// @brief Whether to destroy the coin object when collected
        public bool destroyOnCollect = true;

        private Vector3 startPosition;
        private float bobTime;

        /// @brief Initializes the coin's position and collider settings
        private void Start()
        {
            startPosition = transform.position;
            bobTime = Random.Range(0f, 2f * Mathf.PI); // Random start phase

            // Ensure the object has a collider and it's set as a trigger
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = true;
            }
            else
            {
                Debug.LogWarning("Coin object should have a Collider component set as trigger!", this);
            }
        }

        /// @brief Updates the coin's rotation and position for animation
        /// @details Handles continuous rotation and sinusoidal bobbing motion
        private void Update()
        {
            // Rotate the coin
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

            // Bob up and down
            bobTime += bobSpeed * Time.deltaTime;
            float newY = startPosition.y + Mathf.Sin(bobTime) * bobHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }

        /// @brief Handles coin collection when player enters trigger
        /// @param other The collider that entered the trigger zone
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // Try to find CoinGameManager to add score
                CoinGameManager CoinGameManager = CoinGameManager.Instance;
                if (CoinGameManager != null)
                {
                    CoinGameManager.AddCoins(value);
                }
                else
                {
                    Debug.LogWarning("CoinGameManager not found! Coin collection won't be tracked.", this);
                }

                if (destroyOnCollect)
                {
                    // You might want to add particle effects or sound here before destroying
                    Destroy(gameObject);
                }
                else
                {
                    // If not destroying, maybe disable renderer and collider
                    gameObject.SetActive(false);
                }
            }
        }

        /// @brief Draws debug visualization of the coin in the Unity editor
        /// @details Shows the bobbing range and coin value
        private void OnDrawGizmosSelected()
        {
            // Draw the bob range in the editor
            Gizmos.color = Color.yellow;
            Vector3 bottom = transform.position - Vector3.up * bobHeight;
            Vector3 top = transform.position + Vector3.up * bobHeight;
            Gizmos.DrawLine(bottom, top);
            Gizmos.DrawWireSphere(bottom, 0.1f);
            Gizmos.DrawWireSphere(top, 0.1f);

            // Draw coin value
            #if UNITY_EDITOR
            UnityEditor.Handles.Label(transform.position + Vector3.up * (bobHeight + 0.3f), $"Value: {value}");
            #endif
        }
    }
}
