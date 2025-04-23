using UnityEngine;

/**
 * @brief Manages switching between different cameras and control systems
 * 
 * This component allows toggling between a main camera/controller and 
 * an alternate camera/controller when the player enters a trigger zone
 * and presses a designated key
 */
public class CameraSwitchTrigger : MonoBehaviour
{
    [Header("References")]
    public GameObject playerObject; // Player object (mesh/model)
    public GameObject mainCamera; // Main camera
    public GameObject alternateCamera; // Alternate camera
    public MonoBehaviour mainPlayerController; // Main player controller
    public MonoBehaviour alternateController; // Alternate controller

    [Header("Settings")]
    public KeyCode switchKey = KeyCode.F; // Switch key
    public bool hidePlayer = true; // Whether to hide the player when switching

    private bool isInsideTrigger = false;
    private bool isUsingAlternate = false; // Current state flag

    /**
     * @brief Checks for key input to toggle camera/controller when player is in trigger zone
     * 
     * Called every frame to detect when the player presses the switch key
     */
    private void Update()
    {
        if (isInsideTrigger && Input.GetKeyDown(switchKey))
        {
            ToggleCameraAndController();
        }
    }

    /**
     * @brief Toggles between main and alternate camera/controller setups
     * 
     * Activates/deactivates the appropriate camera and enables/disables
     * the corresponding controller components. Can also hide/show the player model.
     */
    private void ToggleCameraAndController()
    {
        isUsingAlternate = !isUsingAlternate;

        // Switch cameras
        mainCamera.SetActive(!isUsingAlternate);
        alternateCamera.SetActive(isUsingAlternate);

        // Switch controllers
        mainPlayerController.enabled = !isUsingAlternate;
        
        if (alternateController != null)
        {
            alternateController.enabled = isUsingAlternate;
        }

        // Hide/show player if needed
        if (hidePlayer && playerObject != null)
        {
            playerObject.SetActive(!isUsingAlternate);
        }
    }

    /**
     * @brief Handles player entering the trigger zone
     * 
     * @param other The collider that entered the trigger
     * 
     * Sets the inside trigger flag when a player enters the zone
     */
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInsideTrigger = true;
            // Can add UI hint here
        }
    }

    /**
     * @brief Handles player exiting the trigger zone
     * 
     * @param other The collider that exited the trigger
     * 
     * Resets the inside trigger flag and automatically switches
     * back to the main camera/controller setup if using the alternate
     */
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInsideTrigger = false;
            // Can remove UI hint here
            
            // Automatically return to main control when exiting
            if (isUsingAlternate)
            {
                ResetToMain();
            }
        }
    }

    /**
     * @brief Resets to the main camera and controller setup
     * 
     * Forces a return to the main camera and controller regardless
     * of the current state, and shows the player model if it was hidden
     */
    private void ResetToMain()
    {
        isUsingAlternate = false;
        
        mainCamera.SetActive(true);
        alternateCamera.SetActive(false);
        
        mainPlayerController.enabled = true;
        
        if (alternateController != null)
        {
            alternateController.enabled = false;
        }
        
        if (hidePlayer && playerObject != null)
        {
            playerObject.SetActive(true);
        }
    }
}