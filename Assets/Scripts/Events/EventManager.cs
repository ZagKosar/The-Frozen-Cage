using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventManager
{
    private static EventManager s_instance;
    /// <summary>
    /// Получение созданного объекта
    /// </summary>
    public static EventManager Instance
    {
        get
        {
            s_instance ??= new EventManager();
            return s_instance;
        }
    }

    private Dictionary<Type, List<Delegate>> eventSubscribers = new Dictionary<Type, List<Delegate>>();
    /// <summary>
    /// Subscribe - подписка что бы ждать Invoke()
    /// </summary>
    /// <typeparam name="T">Вид события</typeparam>
    /// <param name="action"> Действие которое срабатывает при получении события(звонка) </param>

    private bool _isSceneListening = false;

    public void Subscribe<T>(Action<T> action)
    {
        EnsureSceneListener();

        var type = typeof(T);

        if (!eventSubscribers.TryGetValue(type,out var subscribers))
        {
            subscribers = new List<Delegate>();
            eventSubscribers[type] = subscribers;
        }

        if (subscribers.Contains(action))
            return;

        subscribers.Add(action);
    }

    public void Unsubscribe<T>(Action<T> action)
    {
        var type = typeof(T);

        if (!eventSubscribers.TryGetValue(type, out var subscribers))
            return;

        subscribers.Remove(action);
    }

    public void Invoke<T>(T data)
    {
        var type = typeof(T);

        if (!eventSubscribers.TryGetValue(type, out var subscribers))
            return;

        var copy = new List<Delegate>(subscribers);

        foreach (var subscriber in subscribers)
        {
            try
            {
                ((Action<T>)subscriber)?.Invoke(data);
            }
            catch (Exception e)
            {
                Debug.LogError($"[EventManager] Error invoking {type.Name}: {e}");
            }
        }
    }

    public void ClearAll()
    {
        eventSubscribers.Clear();
    }

    /// <summary>
    /// Удаляет подписки, чьи целевые объекты были уничтожены
    /// </summary>
    public void CleanupDestroyedSubscribers()
    {
        foreach (var kvp in eventSubscribers)
        {
            kvp.Value.RemoveAll(d =>
            {
                if (d.Target is MonoBehaviour mb)
                    return mb == null; // Unity null check — объект уничтожен
                return false;
            });
        }
    }

    private void EnsureSceneListener()
    {
        if (_isSceneListening)
            return;

        _isSceneListening = true;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnSceneUnloaded(Scene scene)
    {
        CleanupDestroyedSubscribers();
    }
}
