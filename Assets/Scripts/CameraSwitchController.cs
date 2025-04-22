using UnityEngine;

public class CameraSwitchTrigger : MonoBehaviour
{
    [Header("References")]
    public GameObject playerObject; // Объект игрока (меш/модель)
    public GameObject mainCamera; // Основная камера
    public GameObject alternateCamera; // Альтернативная камера
    public MonoBehaviour mainPlayerController; // Основной контроллер игрока
    public MonoBehaviour alternateController; // Альтернативный контроллер

    [Header("Settings")]
    public KeyCode switchKey = KeyCode.F; // Клавиша переключения
    public bool hidePlayer = true; // Скрывать ли игрока при переключении

    private bool isInsideTrigger = false;
    private bool isUsingAlternate = false; // Флаг текущего состояния

    private void Update()
    {
        if (isInsideTrigger && Input.GetKeyDown(switchKey))
        {
            ToggleCameraAndController();
        }
    }

    private void ToggleCameraAndController()
    {
        isUsingAlternate = !isUsingAlternate;

        // Переключаем камеры
        mainCamera.SetActive(!isUsingAlternate);
        alternateCamera.SetActive(isUsingAlternate);

        // Переключаем контроллеры
        mainPlayerController.enabled = !isUsingAlternate;
        
        if (alternateController != null)
        {
            alternateController.enabled = isUsingAlternate;
        }

        // Скрываем/показываем игрока если нужно
        if (hidePlayer && playerObject != null)
        {
            playerObject.SetActive(!isUsingAlternate);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInsideTrigger = true;
            // Можно добавить UI подсказку здесь
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInsideTrigger = false;
            // Можно убрать UI подсказку здесь
            
            // Автоматически возвращаем к основному управлению при выходе
            if (isUsingAlternate)
            {
                ResetToMain();
            }
        }
    }

    // Возврат к основному управлению
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