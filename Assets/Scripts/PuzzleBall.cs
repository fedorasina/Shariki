using UnityEngine;

public class PuzzleBall : MonoBehaviour
{
    public PuzzleSystem puzzleSystem;

    void OnTriggerEnter(Collider other)
    {
        for (int i = 0; i < puzzleSystem.puzzleColliders.Length; i++)
        {
            if (other.gameObject == puzzleSystem.puzzleColliders[i])
            {
                puzzleSystem.RegisterPlayerInput(i);
                Destroy(gameObject);
                return;
            }
        }
    }
}