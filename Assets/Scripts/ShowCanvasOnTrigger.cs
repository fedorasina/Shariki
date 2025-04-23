using UnityEngine;
using TMPro;

/**
 * @brief Controls the visibility of TextMeshPro text based on player position and view angle
 * 
 * This component shows/hides a TextMeshPro text element when the player enters a trigger
 * and is looking at the object within a specified angle threshold
 */
public class ShowTMPTextOnTrigger : MonoBehaviour
{
    [SerializeField] private TMP_Text tmpText;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float viewAngleThreshold = 45f;
    [SerializeField] private Camera playerCamera;

    private CanvasGroup textCanvasGroup;
    private bool isPlayerInside = false;
    private bool isTextVisible = false;

    /**
     * @brief Initializes the component
     * 
     * Gets or adds a CanvasGroup component to the text object for fading
     * and finds the main camera if none is assigned
     */
    private void Start()
    {
        if (tmpText != null)
        {
            textCanvasGroup = tmpText.GetComponent<CanvasGroup>();
            if (textCanvasGroup == null)
            {
                textCanvasGroup = tmpText.gameObject.AddComponent<CanvasGroup>();
            }
            textCanvasGroup.alpha = 0f;
        }
        else
        {
            Debug.LogWarning("TMP_Text not assigned in the inspector!");
        }

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    /**
     * @brief Handles player entering the trigger area
     * 
     * @param other The collider that entered the trigger
     * 
     * Sets the player inside flag when a collider with the player tag enters
     */
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInside = true;
        }
    }

    /**
     * @brief Handles player exiting the trigger area
     * 
     * @param other The collider that exited the trigger
     * 
     * Resets flags and fades out the text when the player leaves
     */
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInside = false;
            isTextVisible = false;
            StopAllCoroutines();
            StartCoroutine(FadeText(0f));
        }
    }

    /**
     * @brief Updates the text visibility based on player's view angle
     * 
     * Checks if the player is looking at the object and shows/hides
     * the text accordingly using fade animations
     */
    private void Update()
    {
        if (isPlayerInside && textCanvasGroup != null)
        {
            bool looking = IsLookingAtObject();

            if (looking && !isTextVisible)
            {
                isTextVisible = true;
                StopAllCoroutines();
                StartCoroutine(FadeText(1f));
            }
            else if (!looking && isTextVisible)
            {
                isTextVisible = false;
                StopAllCoroutines();
                StartCoroutine(FadeText(0f));
            }
        }
    }

    /**
     * @brief Determines if the player is looking at this object
     * 
     * @return True if the angle between the camera forward vector and
     * the vector to this object is less than the threshold
     */
    private bool IsLookingAtObject()
    {
        Vector3 toObject = transform.position - playerCamera.transform.position;
        float angle = Vector3.Angle(playerCamera.transform.forward, toObject);

        return angle <= viewAngleThreshold;
    }

    /**
     * @brief Coroutine that gradually fades the text to a target alpha
     * 
     * @param targetAlpha The target alpha value (0 for invisible, 1 for fully visible)
     * 
     * @return IEnumerator for coroutine functionality
     */
    private System.Collections.IEnumerator FadeText(float targetAlpha)
    {
        float startAlpha = textCanvasGroup.alpha;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            textCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            yield return null;
        }

        textCanvasGroup.alpha = targetAlpha;
    }
}
