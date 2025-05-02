using UnityEngine;
using System.Collections.Generic;

public class ShootPrefab : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab; // Префаб снаряда
    [SerializeField] private Transform spawnPoint; // Точка спавна снаряда
    [SerializeField] private float shootForce = 20f; // Сила выстрела
    [SerializeField] private float fireRate = 0.5f; // Скорострельность (секунды между выстрелами)
    [SerializeField] private int maxProjectiles = 100; // Максимальное количество снарядов

    private float nextFireTime; // Время следующего выстрела
    private Queue<GameObject> projectilePool = new Queue<GameObject>(); // Очередь для отслеживания снарядов

    void Update()
    {
        // Проверка нажатия левой кнопки мыши и времени между выстрелами
        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        // Проверка превышения лимита снарядов
        if (projectilePool.Count >= maxProjectiles)
        {
            // Удаляем самый старый снаряд
            GameObject oldestProjectile = projectilePool.Dequeue();
            if (oldestProjectile != null)
            {
                Destroy(oldestProjectile);
            }
        }

        // Создание нового снаряда
        GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
        projectilePool.Enqueue(projectile); // Добавляем снаряд в очередь

        // Получение компонента Rigidbody для добавления силы
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Применение силы к снаряду в направлении вперед
            rb.AddForce(spawnPoint.forward * shootForce, ForceMode.Impulse);
        }

        // Уничтожение снаряда через 5 секунд (опционально)
        Destroy(projectile, 5f);
    }
}