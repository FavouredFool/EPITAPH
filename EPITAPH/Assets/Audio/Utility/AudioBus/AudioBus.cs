using System.Collections.Generic;
using UnityEngine;
using System;


public static class AudioBus
{
    private static Dictionary<Type, List<object>> subscribers = new();
    public static void Subscribe<T>(AudioEventSubscriber<T> sub) where T : AudioEvent
    {
        if (!subscribers.ContainsKey(typeof(T)))
            subscribers.Add(typeof(T), new());
        subscribers[typeof(T)].Add(sub);

    }

    public static void Unsubscribe<T>(AudioEventSubscriber<T> sub) where T : AudioEvent // Method for a Subscriber to unsubscribe from the Audio Controller
    {
        if (!subscribers.ContainsKey(typeof(T)))
            return;
        subscribers[typeof(T)].Remove(sub);
    }

    public static void Fire<T>(T e) where T : AudioEvent
    {
        if (subscribers.TryGetValue(typeof(T), out var subs))
            foreach (var sub in subs)
                ((AudioEventSubscriber<T>)sub).OnEventHappened(e);
    }
}
