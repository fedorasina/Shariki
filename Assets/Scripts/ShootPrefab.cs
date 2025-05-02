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
    [SerializeField] private LayerMask raycastLayerMask; // Маска слоев для Raycast
    [SerializeField] private float rotationSpeed = 10f; // Скорость поворота оружия (градусов/сек)

    private float nextFireTime; // Время следующего выстрела
    private Queue<GameObject> projectilePool = new Queue<GameObject>(); // Очередь для отслеживания снарядов
    private Vector3 screenCenter; // Кэшированный центр экрана

    void Start()
    {
        // Кэшируем центр экрана при старте
        screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
    }

    void Update()
    {
        // Обновление прицеливания каждый кадр с оптимизацией
        AimAtScreenCenter();

        // Проверка нажатия левой кнопки мыши и времени между выстрелами
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void AimAtScreenCenter()
    {
        if (mainCamera == null) return;

        // Создаем луч из камеры через центр экрана
        Ray ray = mainCamera.ScreenPointToRay(screenCenter);
        Vector3 targetPoint;

        // Проверяем, пересекает ли луч что-либо в сцене, используя маску слоев
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, raycastLayerMask))
        {
            // Если луч попал в объект, используем точку попадания
            targetPoint = hit.point;
        }
        else
        {
            // Если луч не попал, используем точку на расстоянии 100 единиц
            targetPoint = ray.origin + ray.direction * 100f;
        }

        // Вычисляем направление и плавно поворачиваем оружие
        Vector3 direction = (targetPoint - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
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