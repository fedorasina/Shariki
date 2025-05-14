using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace Platformers
{
    /// @brief Manages the coin collection system and general game state in the platformer game.
    /// @details This singleton class handles coin collection, checkpoints, UI updates, and level progression.
    /// It persists between scenes and manages the player's progress throughout the game.
    public class CoinGameManager : MonoBehaviour
    {
        [Header("UI References")]
        /// @brief Name of the TextMeshPro UI element that displays coin count
        [SerializeField] private string coinTextName = "CoinText";
        /// @brief Name of the TextMeshPro UI element that displays temporary messages
        [SerializeField] private string messageTextName = "MessageText";
        /// @brief Duration in seconds for which temporary messages are displayed
        public float messageDisplayTime = 2f;

        [Header("Game Settings")]
        /// @brief Initial spawn point for the player
        public Vector3 playerSpawnPoint;
        /// @brief Whether the checkpoint system is enabled
        public bool useCheckpoints = true;
        /// @brief Whether all coins must be collected to exit the level
        public bool requireAllCoinsToExit = true;

        private TextMeshProUGUI coinText;
        private TextMeshProUGUI messageText;
        private int coins = 0;
        private int totalCoinsInLevel = 0;
        private Vector3 lastCheckpoint;
        private float messageTimer;

        private static CoinGameManager instance;
        /// @brief Singleton instance of the CoinGameManager
        /// @return The single instance of CoinGameManager
        public static CoinGameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindAnyObjectByType<CoinGameManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("CoinGameManager");
                        instance = go.AddComponent<CoinGameManager>();
                    }
                }
                return instance;
            }
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialize spawn point
            lastCheckpoint = playerSpawnPoint;

            // Subscribe to the scene loaded event
            SceneManager.sceneLoaded += OnSceneLoaded;

            // Initialize the coin count
            CountCoinsInLevel();
            FindUIElements();
            UpdateUI();
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void FindUIElements()
        {
            // Find UI elements by name
            GameObject coinTextObj = GameObject.Find(coinTextName);
            GameObject messageTextObj = GameObject.Find(messageTextName);

            if (coinTextObj != null)
            {
                coinText = coinTextObj.GetComponent<TextMeshProUGUI>();
            }
            else
            {
                Debug.LogWarning($"Could not find UI element named {coinTextName}");
            }

            if (messageTextObj != null)
            {
                messageText = messageTextObj.GetComponent<TextMeshProUGUI>();
                if (messageText != null)
                {
                    messageText.gameObject.SetActive(false);
                }
            }
            else
            {
                Debug.LogWarning($"Could not find UI element named {messageTextName}");
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Reset counters when a new scene is loaded
            coins = 0;
            CountCoinsInLevel();
            
            // Find new UI elements in the loaded scene
            FindUIElements();
            
            UpdateUI();
            Debug.Log("Scene loaded");
        }

        private void Start()
        {
            CountCoinsInLevel();
            FindUIElements();
            UpdateUI();
        }

        private void CountCoinsInLevel()
        {
            Coin[] coinsInLevel = FindObjectsByType<Coin>(FindObjectsSortMode.None);
            totalCoinsInLevel = coinsInLevel.Length;
            Debug.Log($"Found {totalCoinsInLevel} coins in level");
        }

        private void Update()
        {
            // Handle temporary messages
            if (messageTimer > 0)
            {
                messageTimer -= Time.deltaTime;
                if (messageTimer <= 0 && messageText != null)
                {
                    messageText.gameObject.SetActive(false);
                }
            }
        }

        /// @brief Adds coins to the player's collection and updates UI
        /// @param amount Number of coins to add
        public void AddCoins(int amount)
        {
            coins += amount;
            UpdateUI();
            ShowMessage($"+{amount} Coins! ({coins}/{totalCoinsInLevel})");
        }

        /// @brief Checks if all required coins have been collected
        /// @return True if enough coins are collected to exit the level
        public bool AreAllCoinsCollected()
        {
            return !requireAllCoinsToExit || coins >= totalCoinsInLevel;
        }

        /// @brief Sets a new checkpoint position
        /// @param position World position for the new checkpoint
        public void SetCheckpoint(Vector3 position)
        {
            if (useCheckpoints)
            {
                lastCheckpoint = position;
                ShowMessage("Checkpoint reached!");
            }
        }

        /// @brief Respawns the player at the last checkpoint
        /// @details Handles proper physics state management during respawn
        public void RespawnPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Rigidbody rb = player.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Disable physics temporarily
                    rb.isKinematic = true;
                    
                    // Teleport to checkpoint
                    player.transform.position = lastCheckpoint;
                    
                    // Reset velocity and rotation
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    
                    // Re-enable physics
                    rb.isKinematic = false;
                }
                else
                {
                    // Fallback if no Rigidbody found
                    player.transform.position = lastCheckpoint;
                    Debug.LogWarning("Player doesn't have a Rigidbody component!", player);
                }
            }
        }

        /// @brief Displays a temporary message in the UI
        /// @param message The message to display
        /// @param duration Optional duration override. If negative, uses default duration
        public void ShowMessage(string message, float duration = -1)
        {
            if (messageText != null)
            {
                messageText.text = message;
                messageText.gameObject.SetActive(true);
                messageTimer = duration < 0 ? messageDisplayTime : duration;
            }
            else
            {
                Debug.LogWarning("Message text is not assigned!");
            }
        }

        private void UpdateUI()
        {
            if (coinText != null)
            {
                coinText.text = requireAllCoinsToExit 
                    ? $"Coins: {coins}/{totalCoinsInLevel}"
                    : $"Coins: {coins}";
            }
            else
            {
                Debug.LogWarning("Coin text is not assigned!");
            }
        }

        /// @brief Restarts the current level
        public void RestartLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        /// @brief Loads the next level if available
        public void LoadNextLevel()
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                Debug.Log("No more levels to load!");
                // You might want to load a "Game Complete" scene or show a message
            }
        }
    }
}
