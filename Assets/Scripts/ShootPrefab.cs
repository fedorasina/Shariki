using UnityEngine;
using System.Collections.Generic;

public class ShootPrefab : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab; // Префаб снаряда
    [SerializeField] private Transform spawnPoint; // Точка спавна снаряда
    [SerializeField] private float shootForce = 20f; // Сила выстрела
    [SerializeField] private float fireRate = 0.5f; // Скорострельность (секунды между выстрелами)
    [SerializeField] private int maxProjectiles = 100; // Максимальное количество снарядов
    [SerializeField] private Camera mainCamera; // Главная камера
    [SerializeField] private float aimUpdateRate = 0.1f; // Частота обновления прицеливания (секунды)

    private float nextFireTime; // Время следующего выстрела
    private Queue<GameObject> projectilePool = new Queue<GameObject>(); // Очередь для отслеживания снарядов
    private float nextAimUpdateTime; // Время следующего обновления прицеливания

    void Update()
    {
        // Обновление прицеливания с заданной частотой
        if (Time.time >= nextAimUpdateTime)
        {
            AimAtScreenCenter();
            nextAimUpdateTime = Time.time + aimUpdateRate;
        }

        // Проверка нажатия левой кнопки мыши и времени между выстрелами
        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void AimAtScreenCenter()
    {
        if (mainCamera == null) return;

        // Центр экрана в экранных координатах
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);

        // Создаем луч из камеры через центр экрана
        Ray ray = mainCamera.ScreenPointToRay(screenCenter);
        Vector3 targetPoint;

        // Проверяем, пересекает ли луч что-либо в сцене
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            // Если луч попал в объект, используем точку попадания
            targetPoint = hit.point;
        }
        else
        {
            // Если луч не попал, используем точку на расстоянии 100 единиц от камеры
            targetPoint = ray.origin + ray.direction * 100f;
        }

        // Поворачиваем только оружие в сторону целевой точки
        Vector3 direction = (targetPoint - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);
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

        // Уничтожение снаряда через 5 секунд
        Destroy(projectile, 5f);
    }
}