using System;
using System.Collections.Generic;
using UnityEngine;

public class EventSystem
{
	public EventSystem()
	{
		this.log = new LoggerFIO("Events", 35);
	}

	public static void Trigger(GameObject go, int hash, object data = null)
	{
		KObject orCreateObject = KObjectManager.Instance.GetOrCreateObject(go);
		orCreateObject.GetEventSystem().Trigger(hash, data);
	}

	public void OnCleanUp()
	{
		for (int i = this.subscribedEvents.Count - 1; i >= 0; i--)
		{
			EventSystem.SubscribedEntry subscribedEntry = this.subscribedEvents[i];
			if (subscribedEntry.go != null)
			{
				this.Unsubscribe(subscribedEntry.go, subscribedEntry.hash, subscribedEntry.handler);
			}
		}
		for (int j = 0; j < this.entries.Count; j++)
		{
			EventSystem.Entry entry = this.entries[j];
			entry.handler = null;
			this.entries[j] = entry;
		}
		this.entries.Clear();
		this.subscribedEvents.Clear();
	}

	public void UnregisterEvent(GameObject target, int eventName, Action<object> handler)
	{
		for (int i = 0; i < this.subscribedEvents.Count; i++)
		{
			if (this.subscribedEvents[i].hash == eventName && this.subscribedEvents[i].handler == handler && this.subscribedEvents[i].go == target)
			{
				this.subscribedEvents.RemoveAt(i);
				break;
			}
		}
	}

	public void RegisterEvent(GameObject target, int eventName, Action<object> handler)
	{
		this.subscribedEvents.Add(new EventSystem.SubscribedEntry(target, eventName, handler));
	}

	public int Subscribe(int hash, Action<object> handler)
	{
		this.entries.Add(new EventSystem.Entry(hash, handler, ++this.nextId));
		return this.nextId;
	}

	public void Unsubscribe(int hash, Action<object> handler)
	{
		int i = 0;
		while (i < this.entries.Count)
		{
			if (this.entries[i].hash == hash && this.entries[i].handler == handler)
			{
				if (this.currentlyTriggering == 0)
				{
					this.entries.RemoveAt(i);
					break;
				}
				this.dirty = true;
				EventSystem.Entry entry = this.entries[i];
				entry.handler = null;
				this.entries[i] = entry;
				break;
			}
			else
			{
				i++;
			}
		}
	}

	public void Unsubscribe(int id)
	{
		int i = 0;
		while (i < this.entries.Count)
		{
			if (this.entries[i].id == id)
			{
				if (this.currentlyTriggering == 0)
				{
					this.entries.RemoveAt(i);
					break;
				}
				this.dirty = true;
				EventSystem.Entry entry = this.entries[i];
				entry.handler = null;
				this.entries[i] = entry;
				break;
			}
			else
			{
				i++;
			}
		}
	}

	public void Subscribe(GameObject target, int eventName, Action<object> handler)
	{
		this.RegisterEvent(target, eventName, handler);
		KObject orCreateObject = KObjectManager.Instance.GetOrCreateObject(target);
		orCreateObject.GetEventSystem().Subscribe(eventName, handler);
	}

	public void Unsubscribe(GameObject target, int eventName, Action<object> handler)
	{
		this.UnregisterEvent(target, eventName, handler);
		if (target == null)
		{
			return;
		}
		KObject orCreateObject = KObjectManager.Instance.GetOrCreateObject(target);
		orCreateObject.GetEventSystem().Unsubscribe(eventName, handler);
	}

	public void Unsubscribe(string[] eventNames, Action<object> handler)
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
		this.currentlyTriggering++;
		int count = this.entries.Count;
		for (int i = 0; i < count; i++)
		{
			if (this.entries[i].hash == hash && this.entries[i].handler != null)
			{
				if (EventSystem.ENABLE_DETAILED_EVENT_PROFILE_INFO)
				{
				}
				this.entries[i].handler(data);
				if (EventSystem.ENABLE_DETAILED_EVENT_PROFILE_INFO)
				{
				}
			}
		}
		this.currentlyTriggering--;
		if (this.dirty && this.currentlyTriggering == 0)
		{
			this.dirty = false;
			this.entries.RemoveAll((EventSystem.Entry x) => x.handler == null);
		}
	}

	private static bool ENABLE_DETAILED_EVENT_PROFILE_INFO;

	private int nextId;

	private int currentlyTriggering;

	private bool dirty;

	private List<EventSystem.SubscribedEntry> subscribedEvents = new List<EventSystem.SubscribedEntry>();

	private List<EventSystem.Entry> entries = new List<EventSystem.Entry>();

	private LoggerFIO log;

	private struct Entry
	{
		public Entry(int hash, Action<object> handler, int id)
		{
			this.handler = handler;
			this.hash = hash;
			this.id = id;
		}

		public Action<object> handler;

		public int hash;

		public int id;
	}

	private struct SubscribedEntry
	{
		public SubscribedEntry(GameObject go, int hash, Action<object> handler)
		{
			this.go = go;
			this.hash = hash;
			this.handler = handler;
		}

		public Action<object> handler;

		public int hash;

		public GameObject go;
	}
}
