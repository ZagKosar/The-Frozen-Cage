using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager
{
    private static EventManager s_instance;
    public static EventManager Instance
    {
        get
        {
            s_instance ??= new EventManager();
            return s_instance;
        }
    }

    private Dictionary<Type, List<Delegate>> eventSubscribers = new Dictionary<Type, List<Delegate>>();

    public void Subscribe<T>(Action<T> action)
    {
        var type = typeof(T);

        if (!eventSubscribers.TryGetValue(type,out var subscribers))
        {
            subscribers = new List<Delegate>();
            eventSubscribers[type] = subscribers;
        }

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

        foreach (var subscriber in subscribers)
        {
            ((Action<T>)subscriber)?.Invoke(data);        
        }
    }
}
