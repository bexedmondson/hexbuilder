using System;
using System.Collections.Generic;

public interface IEvent { }

public class EventDispatcher : IInjectable
{
	public delegate void EventHandler<T> (T arg1) where T : IEvent;
		
	private readonly Dictionary<Type, Delegate> listeners = new();

	public EventDispatcher()
	{
		InjectionManager.Register(this);
	}

	public void Add<T>(EventHandler<T> callback) where T : IEvent
	{
		Type t = typeof(T);
		if (!listeners.TryAdd(t, callback))
			listeners[t] = (EventHandler<T>)listeners[t] + callback;
	}
	
	public virtual void Remove<T>(EventHandler<T> callback) where T : IEvent
	{
		Type t = typeof(T);
		if (!listeners.TryGetValue(t, out var callbacks))
			return;
		
		listeners[t] = (EventHandler<T>)callbacks - callback;
		if (listeners[t] == null)
			listeners.Remove(t);
	}

	public virtual void Dispatch<T> (T eventObject) where T : IEvent
	{
		Type t = typeof(T);
		if (listeners.TryGetValue(t, out var callbacks))
		{
			((EventHandler<T>)callbacks)(eventObject);
		}
	}

	public virtual void Clear()
	{
		listeners.Clear();
	}
}