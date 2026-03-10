using System;
using System.Collections.Generic;
using UnityEngine;

public static class SignalBus
{
	private static readonly Dictionary<Type, Delegate> _events = new();

	public static void Subscribe<T>(Action<T> listener)
	{
		if (_events.TryGetValue(typeof(T), out var existing))
		{
			_events[typeof(T)] = Delegate.Combine(existing, listener);
		}
		else
		{
			_events[typeof(T)] = listener;
		}
	}

	public static void Unsubscribe<T>(Action<T> listener)
	{
		if (_events.TryGetValue(typeof(T), out var existing))
		{
			var combined = Delegate.Remove(existing, listener);
			if (combined == null)
				_events.Remove(typeof(T));
			else
				_events[typeof(T)] = combined;
		}
	}

	public static void Fire<T>(T payload)
	{
		if (_events.TryGetValue(typeof(T), out var existing))
		{
			(existing as Action<T>)?.Invoke(payload);
		}
	}
}
