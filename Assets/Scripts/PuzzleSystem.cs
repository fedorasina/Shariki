using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleSystem : MonoBehaviour
{
    [Header("Main Settings")]
    public GameObject[] puzzleColliders; // Все целевые кубы
    public GameObject cubePrefab; // Префаб мигающего куба
    public GameObject rewardPrefab; // Награда
    public Transform rewardSpawnPoint; // Точка спавна награды

    [Header("Sequence Settings")]
    [Tooltip("Минимальное количество кубов в последовательности")]
    public int minSequenceLength = 2;
    [Tooltip("Максимальное количество кубов в последовательности")]
    public int maxSequenceLength = 5;
    [Tooltip("Текущее количество кубов в последовательности")]
    public int currentSequenceLength = 2;

    [Header("Timing Settings")]
    public float displayTime = 1f;
    public float delayBetweenCubes = 0.5f;
    public float cubeHeightOffset = 1f;

    [Header("Debug")]
    public bool enableLogs = true;

    private List<int> currentSequence = new List<int>();
    private List<int> playerSequence = new List<int>();
    private bool isDisplayingSequence = false;
    private bool isWaitingForInput = false;
    private bool isActive = false; // Активна ли сейчас головоломка

    public void ActivatePuzzle()
    {
        if (isDisplayingSequence || isWaitingForInput || isActive) return;

        if (enableLogs) Debug.Log("[Puzzle] Puzzle activated");
        isActive = true;
        StartNewSequence();
    }

    void StartNewSequence()
    {
        currentSequence.Clear();
        playerSequence.Clear();

        // Создаём список доступных индексов
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < puzzleColliders.Length; i++)
        {
            availableIndices.Add(i);
        }

        // Генерация последовательности без повторений
        for (int i = 0; i < currentSequenceLength; i++)
        {
            if (availableIndices.Count == 0)
            {
                Debug.LogWarning("Not enough unique cubes! Using repeats.");
                availableIndices = new List<int>();
                for (int j = 0; j < puzzleColliders.Length; j++) availableIndices.Add(j);
            }

            int randomIndex = Random.Range(0, availableIndices.Count);
            int selectedCube = availableIndices[randomIndex];
            currentSequence.Add(selectedCube);
            availableIndices.RemoveAt(randomIndex);

            if (enableLogs) Debug.Log($"[Puzzle] Added cube {selectedCube} to sequence");
        }

        StartCoroutine(DisplaySequence());
    }

    IEnumerator DisplaySequence()
    {
        isDisplayingSequence = true;
        if (enableLogs) Debug.Log($"[Puzzle] Displaying sequence of {currentSequence.Count} cubes");

        foreach (int index in currentSequence)
        {
            Vector3 spawnPosition = puzzleColliders[index].transform.position + Vector3.up * cubeHeightOffset;
            GameObject cube = Instantiate(cubePrefab, spawnPosition, Quaternion.identity);
            
            if (enableLogs) Debug.Log($"[Puzzle] Showing cube at position {index}");

            yield return new WaitForSeconds(displayTime);
            Destroy(cube);
            yield return new WaitForSeconds(delayBetweenCubes);
        }

        isDisplayingSequence = false;
        isWaitingForInput = true;
        if (enableLogs) Debug.Log("[Puzzle] Waiting for player input");
    }

    public void RegisterPlayerInput(int colliderIndex)
    {
        if (!isWaitingForInput || isDisplayingSequence || !isActive) return;

        if (enableLogs) Debug.Log($"[Puzzle] Player hit cube {colliderIndex}");

        playerSequence.Add(colliderIndex);

        // Проверка правильности ввода
        if (playerSequence[playerSequence.Count - 1] != currentSequence[playerSequence.Count - 1])
        {
            if (enableLogs) Debug.Log("[Puzzle] Wrong sequence! Puzzle deactivated");
            StartCoroutine(SequenceFailed());
            return;
        }

        // Если последовательность завершена
        if (playerSequence.Count == currentSequence.Count)
        {
            if (enableLogs) Debug.Log("[Puzzle] Correct sequence!");
            StartCoroutine(SequenceCompleted());
        }
    }

    IEnumerator SequenceCompleted()
    {
        isWaitingForInput = false;

        // Если достигли максимальной длины последовательности
        if (currentSequenceLength >= maxSequenceLength)
        {
            if (rewardPrefab != null && rewardSpawnPoint != null)
            {
                Instantiate(rewardPrefab, rewardSpawnPoint.position, Quaternion.identity);
                if (enableLogs) Debug.Log("[Puzzle] Reward spawned!");
            }
            currentSequenceLength = minSequenceLength;
            isActive = false; // Деактивируем после победы
        }
        else
        {
            currentSequenceLength++;
            yield return new WaitForSeconds(1f);
            StartNewSequence();
        }
    }

    IEnumerator SequenceFailed()
    {
        isWaitingForInput = false;
        isActive = false; // Полностью деактивируем при неудаче
        currentSequenceLength = minSequenceLength;
        yield return new WaitForSeconds(0.5f);
        if (enableLogs) Debug.Log("[Puzzle] Puzzle needs reactivation");
    }
}