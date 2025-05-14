using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * @brief Loads a new scene when triggered
 * 
 * This component loads a specified scene when a ball enters its trigger collider
 */
public class LoadSceneOnTrigger : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "SceneName"; // Scene name to load
    [SerializeField] private string ballTag = "Player";       // Ball tag to detect

    /**
     * @brief Handles collision events with the trigger
     * 
     * @param other The collider that entered the trigger
     * 
     * Checks if the entering object has the specified ball tag and loads the target scene if it does
     */
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(ballTag))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
