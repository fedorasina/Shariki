using UnityEngine;
using TMPro;

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
            Debug.LogWarning("TMP_Text не назначен в инспекторе!");
        }

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInside = true;
        }
    }

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

    private bool IsLookingAtObject()
    {
        Vector3 toObject = transform.position - playerCamera.transform.position;
        float angle = Vector3.Angle(playerCamera.transform.forward, toObject);

        return angle <= viewAngleThreshold;
    }

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
