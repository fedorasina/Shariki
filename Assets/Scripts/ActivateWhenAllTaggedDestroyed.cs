using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActivateWhenAllTaggedDestroyed : MonoBehaviour
{
    [Header("Settings")]
    public string targetTag = "Enemy";
    public bool disableOnStart = true;
    public float checkInterval = 0.5f;

    private void Start()
    {
        if (disableOnStart)
        {
            gameObject.SetActive(false);
        }

        // Запускаем проверку через менеджер вместо корутины
        DestructionManager.Instance.StartTracking(this, targetTag);
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        enabled = false; // Отключаем скрипт после активации
    }
}

// Отдельный менеджер для отслеживания объектов
public class DestructionManager : MonoBehaviour
{
    private static DestructionManager _instance;
    public static DestructionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var obj = new GameObject("DestructionManager");
                _instance = obj.AddComponent<DestructionManager>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }

    private Dictionary<string, List<ActivateWhenAllTaggedDestroyed>> _listeners = new Dictionary<string, List<ActivateWhenAllTaggedDestroyed>>();
    private Dictionary<string, List<GameObject>> _trackedObjects = new Dictionary<string, List<GameObject>>();

    public void StartTracking(ActivateWhenAllTaggedDestroyed listener, string tag)
    {
        if (!_listeners.ContainsKey(tag))
        {
            _listeners[tag] = new List<ActivateWhenAllTaggedDestroyed>();
            _trackedObjects[tag] = new List<GameObject>();
        }

        _listeners[tag].Add(listener);

        // Находим все объекты с этим тегом
        var objects = GameObject.FindGameObjectsWithTag(tag);
        _trackedObjects[tag].AddRange(objects);

        // Если объектов нет - сразу активируем
        if (_trackedObjects[tag].Count == 0)
        {
            listener.Activate();
            return;
        }

        // Добавляем трекеры ко всем объектам
        foreach (var obj in objects)
        {
            if (obj.GetComponent<ObjectDestroyTracker>() == null)
            {
                var tracker = obj.AddComponent<ObjectDestroyTracker>();
                tracker.Initialize(tag, OnObjectDestroyed);
            }
        }

        // Запускаем проверку
        StartCoroutine(PeriodicCheck(tag, listener.checkInterval));
    }

    private IEnumerator PeriodicCheck(string tag, float interval)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            
            // Удаляем null-ссылки (уничтоженные объекты)
            _trackedObjects[tag].RemoveAll(obj => obj == null);

            if (_trackedObjects[tag].Count == 0)
            {
                NotifyListeners(tag);
                yield break;
            }
        }
    }

    private void OnObjectDestroyed(string tag, GameObject destroyedObject)
    {
        if (_trackedObjects.ContainsKey(tag))
        {
            _trackedObjects[tag].Remove(destroyedObject);
            
            if (_trackedObjects[tag].Count == 0)
            {
                NotifyListeners(tag);
            }
        }
    }

    private void NotifyListeners(string tag)
    {
        if (_listeners.ContainsKey(tag))
        {
            foreach (var listener in _listeners[tag])
            {
                if (listener != null)
                {
                    listener.Activate();
                }
            }
            _listeners.Remove(tag);
        }
    }
}

// Трекер для отдельных объектов
public class ObjectDestroyTracker : MonoBehaviour
{
    private string _objectTag;
    private System.Action<string, GameObject> _onDestroyCallback;

    public void Initialize(string tag, System.Action<string, GameObject> callback)
    {
        _objectTag = tag;
        _onDestroyCallback = callback;
    }

    private void OnDestroy()
    {
        _onDestroyCallback?.Invoke(_objectTag, gameObject);
    }
}