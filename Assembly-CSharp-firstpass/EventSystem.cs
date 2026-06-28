using System;
using System.Collections.Generic;
using UnityEngine;

public class EventSystem
{
	public EventSystem()
	{
		this.log = new LoggerFIO("Events");
	}

	public static void Trigger(GameObject go, int hash, object data = null)
	{
		KObject kobject = KObjectManager.Instance.CreateObject(go);
		kobject.GetEventSystem().Trigger(hash, data);
	}

	public void OnCleanUp()
	{
		List<GameObject> list = new List<GameObject>(this.subscribedEvents.Keys);
		foreach (GameObject gameObject in list)
		{
			if (gameObject != null)
			{
				List<int> list2 = new List<int>(this.subscribedEvents[gameObject].Keys);
				foreach (int num in list2)
				{
					List<EventSystem.EventHandler> list3 = new List<EventSystem.EventHandler>(this.subscribedEvents[gameObject][num]);
					foreach (EventSystem.EventHandler eventHandler in list3)
					{
						this.Unsubscribe(gameObject, num, eventHandler);
					}
				}
			}
		}
	}

	public void UnregisterEvent(GameObject target, int eventName, EventSystem.EventHandler handler)
	{
		if (this.subscribedEvents.ContainsKey(target) && this.subscribedEvents[target].ContainsKey(eventName))
		{
			this.subscribedEvents[target][eventName].Remove(handler);
		}
	}

	public void RegisterEvent(GameObject target, int eventName, EventSystem.EventHandler handler)
	{
		if (!this.subscribedEvents.ContainsKey(target))
		{
			this.subscribedEvents.Add(target, new Dictionary<int, List<EventSystem.EventHandler>>());
		}
		if (!this.subscribedEvents[target].ContainsKey(eventName))
		{
			this.subscribedEvents[target].Add(eventName, new List<EventSystem.EventHandler>());
		}
		if (!this.subscribedEvents[target][eventName].Contains(handler))
		{
			this.subscribedEvents[target][eventName].Add(handler);
		}
	}

	public void Subscribe(int hash, EventSystem.EventHandler handler)
	{
		EventSystem.ListenerList listenerList;
		this.eventListeners.TryGetValue(hash, out listenerList);
		if (listenerList == null)
		{
			listenerList = new EventSystem.ListenerList();
			this.eventListeners[hash] = listenerList;
		}
		listenerList.listeners.Add(new EventSystem.ListenerList.Entry(handler));
	}

	public void Unsubscribe(int hash, EventSystem.EventHandler handler)
	{
		EventSystem.ListenerList listenerList;
		if (this.eventListeners.TryGetValue(hash, out listenerList))
		{
			if (listenerList.currentlyTriggering == 0)
			{
				for (int i = 0; i < listenerList.listeners.Count; i++)
				{
					if (listenerList.listeners[i].handler == handler)
					{
						listenerList.listeners.RemoveAt(i);
						break;
					}
				}
			}
			else
			{
				for (int j = 0; j < listenerList.listeners.Count; j++)
				{
					EventSystem.ListenerList.Entry entry = listenerList.listeners[j];
					if (!entry.pendingRemoval && entry.handler == handler)
					{
						entry.pendingRemoval = true;
						listenerList.listeners[j] = entry;
						listenerList.dirty = true;
						break;
					}
				}
			}
		}
	}

	public void Subscribe(GameObject target, int eventName, EventSystem.EventHandler handler)
	{
		this.RegisterEvent(target, eventName, handler);
		KObject kobject = KObjectManager.Instance.CreateObject(target);
		kobject.GetEventSystem().Subscribe(eventName, handler);
	}

	public void Unsubscribe(GameObject target, int eventName, EventSystem.EventHandler handler)
	{
		this.UnregisterEvent(target, eventName, handler);
		if (target == null)
		{
			return;
		}
		KObject kobject = KObjectManager.Instance.CreateObject(target);
		kobject.GetEventSystem().Unsubscribe(eventName, handler);
	}

	public void Unsubscribe(string[] eventNames, EventSystem.EventHandler handler)
	{
		foreach (string text in eventNames)
		{
			int num = Hash.SDBMLower(text);
			this.Unsubscribe(num, handler);
		}
	}

	public void Trigger(int hash, object data = null)
	{
		if (App.IsExiting)
		{
			return;
		}
		EventSystem.ListenerList listenerList;
		this.eventListeners.TryGetValue(hash, out listenerList);
		if (listenerList != null)
		{
			listenerList.currentlyTriggering++;
			int count = listenerList.listeners.Count;
			for (int i = 0; i < count; i++)
			{
				if (!listenerList.listeners[i].pendingRemoval)
				{
					listenerList.listeners[i].handler(data);
				}
			}
			listenerList.currentlyTriggering--;
			if (listenerList.currentlyTriggering == 0 && listenerList.dirty)
			{
				listenerList.dirty = false;
				listenerList.listeners.RemoveAll((EventSystem.ListenerList.Entry x) => x.pendingRemoval);
			}
		}
	}

	public global::Logger GetLog()
	{
		return this.log;
	}

	private Dictionary<int, EventSystem.ListenerList> eventListeners = new Dictionary<int, EventSystem.ListenerList>();

	private Dictionary<GameObject, Dictionary<int, List<EventSystem.EventHandler>>> subscribedEvents = new Dictionary<GameObject, Dictionary<int, List<EventSystem.EventHandler>>>();

	private LoggerFIO log;

	private class ListenerList
	{
		public bool dirty = true;

		public int currentlyTriggering;

		public List<EventSystem.ListenerList.Entry> listeners = new List<EventSystem.ListenerList.Entry>();

		public struct Entry
		{
			public Entry(EventSystem.EventHandler handler)
			{
				this.pendingRemoval = false;
				this.handler = handler;
			}

			public EventSystem.EventHandler handler;

			public bool pendingRemoval;
		}
	}

	public delegate void EventHandler(object data);
}
