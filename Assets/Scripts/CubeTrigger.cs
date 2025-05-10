using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ObjectPool
{
    public GameObject prefab;
    public int initialSize = 10;
    private Queue<GameObject> pooledObjects = new Queue<GameObject>();

    public void Initialize(Transform parent = null)
    {
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Object.Instantiate(prefab, parent);
            obj.SetActive(false);
            pooledObjects.Enqueue(obj);
        }
    }

    public GameObject GetObject(Vector3 position, Quaternion rotation)
    {
        if (pooledObjects.Count == 0)
        {
            GameObject newObj = Object.Instantiate(prefab);
            newObj.SetActive(false);
            pooledObjects.Enqueue(newObj);
        }

        GameObject obj = pooledObjects.Dequeue();
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);
        return obj;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pooledObjects.Enqueue(obj);
    }
}

public class CubeTrigger : MonoBehaviour
{
    [Header("Trigger Settings")]
    [SerializeField] private string ballTag = "Ball";
    [SerializeField] private float activationTime = 5f;

    [Header("Object Pool")]
    [SerializeField] private ObjectPool objectPool;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    private List<GameObject> activatedObjects = new List<GameObject>();
    private List<Vector3> originalPositions = new List<Vector3>();
    private List<Quaternion> originalRotations = new List<Quaternion>();
    private bool isActivated = false;
    private float timer = 0f;

    private void Start()
    {
        if (objectPool != null)
        {
            objectPool.Initialize(transform.parent);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActivated && other.CompareTag(ballTag))
        {
            ActivateObjects();
        }
    }

    private void ActivateObjects()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points assigned!");
            return;
        }

        isActivated = true;
        activatedObjects.Clear();
        originalPositions.Clear();
        originalRotations.Clear();

        foreach (Transform spawnPoint in spawnPoints)
        {
            GameObject obj = objectPool.GetObject(spawnPoint.position, spawnPoint.rotation);
            activatedObjects.Add(obj);
            originalPositions.Add(spawnPoint.position);
            originalRotations.Add(spawnPoint.rotation);
        }

        timer = activationTime;
    }

    private void Update()
    {
        if (!isActivated) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            CheckObjectsAndDestroy();
        }
    }

    private void CheckObjectsAndDestroy()
    {
        activatedObjects.RemoveAll(obj => obj == null);

        if (activatedObjects.Count == 0)
        {
            Destroy(gameObject);
        }
        else
        {
            foreach (GameObject obj in activatedObjects)
            {
                if (obj != null)
                {
                    objectPool.ReturnObject(obj);
                }
            }

            ResetState();
        }
    }

    private void ResetState()
    {
        isActivated = false;
        activatedObjects.Clear();
        originalPositions.Clear();
        originalRotations.Clear();
    }
}