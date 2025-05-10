using UnityEngine;

public class DestroyOnTagCollision : MonoBehaviour
{
    [Tooltip("Тег объекта, при столкновении с которым этот объект будет уничтожен")]
    [SerializeField] private string destroyerTag = "Destroyer"; // Тег, который вызывает уничтожение

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, имеет ли столкнувшийся объект нужный тег
        if (other.CompareTag(destroyerTag))
        {
            // Уничтожаем этот объект
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Аналогичная проверка для физических столкновений
        if (collision.gameObject.CompareTag(destroyerTag))
        {
            Destroy(gameObject);
        }
    }
}