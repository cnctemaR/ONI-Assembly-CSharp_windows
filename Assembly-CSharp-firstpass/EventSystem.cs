using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EventSystem
{
	public EventSystem()
	{
		this.log = new LoggerFIO("Events", 35);
	}

	public void Trigger(GameObject go, int hash, object data = null)
	{
		if (App.IsExiting)
		{
			return;
		}
		this.currentlyTriggering++;
		for (int num = 0; num != this.intraObjectRoutes.size; num++)
		{
			if (this.intraObjectRoutes[num].eventHash == hash)
			{
				EventSystem.intraObjectDispatcher[hash][this.intraObjectRoutes[num].handlerIndex].Trigger(go, data);
			}
		}
		int size = this.entries.size;
		if (EventSystem.ENABLE_DETAILED_EVENT_PROFILE_INFO)
		{
			for (int i = 0; i < size; i++)
			{
				if (this.entries[i].hash == hash && this.entries[i].handler != null)
				{
					this.entries[i].handler(data);
				}
			}
		}
		else
		{
			for (int j = 0; j < size; j++)
			{
				if (this.entries[j].hash == hash && this.entries[j].handler != null)
				{
					this.entries[j].handler(data);
				}
			}
		}
		this.currentlyTriggering--;
		if (this.dirty && this.currentlyTriggering == 0)
		{
			this.dirty = false;
			this.entries.RemoveAllSwap((EventSystem.Entry x) => x.handler == null);
			this.intraObjectRoutes.RemoveAllSwap((EventSystem.IntraObjectRoute route) => !route.IsValid());
		}
	}

	public void OnCleanUp()
	{
		for (int i = this.subscribedEvents.size - 1; i >= 0; i--)
		{
			EventSystem.SubscribedEntry subscribedEntry = this.subscribedEvents[i];
			if (subscribedEntry.go != null)
			{
				this.Unsubscribe(subscribedEntry.go, subscribedEntry.hash, subscribedEntry.handler);
			}
		}
		for (int j = 0; j < this.entries.size; j++)
		{
			EventSystem.Entry entry = this.entries[j];
			entry.handler = null;
			this.entries[j] = entry;
		}
		this.entries.Clear();
		this.subscribedEvents.Clear();
		this.intraObjectRoutes.Clear();
	}

	public void UnregisterEvent(GameObject target, int eventName, Action<object> handler)
	{
		for (int i = 0; i < this.subscribedEvents.size; i++)
		{
			if (this.subscribedEvents[i].hash == eventName && this.subscribedEvents[i].handler == handler && this.subscribedEvents[i].go == target)
			{
				this.subscribedEvents.RemoveAt(i);
				return;
			}
		}
	}

	public void RegisterEvent(GameObject target, int eventName, Action<object> handler)
	{
		this.subscribedEvents.Add(new EventSystem.SubscribedEntry(target, eventName, handler));
	}

	public int Subscribe(int hash, Action<object> handler)
	{
		int num = this.nextId + 1;
		this.nextId = num;
		this.entries.Add(new EventSystem.Entry(hash, handler, num));
		return this.nextId;
	}

	public void Unsubscribe(int hash, Action<object> handler)
	{
		int i = 0;
		while (i < this.entries.size)
		{
			if (this.entries[i].hash == hash && this.entries[i].handler == handler)
			{
				if (this.currentlyTriggering == 0)
				{
					this.entries.RemoveAt(i);
					return;
				}
				this.dirty = true;
				EventSystem.Entry entry = this.entries[i];
				entry.handler = null;
				this.entries[i] = entry;
				return;
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
		while (i < this.entries.size)
		{
			if (this.entries[i].id == id)
			{
				if (this.currentlyTriggering == 0)
				{
					this.entries.RemoveAt(i);
					return;
				}
				this.dirty = true;
				EventSystem.Entry entry = this.entries[i];
				entry.handler = null;
				this.entries[i] = entry;
				return;
			}
			else
			{
				i++;
			}
		}
	}

	public int Subscribe(GameObject target, int eventName, Action<object> handler)
	{
		this.RegisterEvent(target, eventName, handler);
		return KObjectManager.Instance.GetOrCreateObject(target).GetEventSystem().Subscribe(eventName, handler);
	}

	public int Subscribe<ComponentType>(int eventName, EventSystem.IntraObjectHandler<ComponentType> handler)
	{
		List<EventSystem.IntraObjectHandlerBase> list;
		if (!EventSystem.intraObjectDispatcher.TryGetValue(eventName, out list))
		{
			list = new List<EventSystem.IntraObjectHandlerBase>();
			EventSystem.intraObjectDispatcher.Add(eventName, list);
		}
		int num = list.IndexOf(handler);
		if (num == -1)
		{
			list.Add(handler);
			num = list.Count - 1;
		}
		this.intraObjectRoutes.Add(new EventSystem.IntraObjectRoute(eventName, num));
		return num;
	}

	public void Unsubscribe(GameObject target, int eventName, Action<object> handler)
	{
		this.UnregisterEvent(target, eventName, handler);
		if (target == null)
		{
			return;
		}
		KObjectManager.Instance.GetOrCreateObject(target).GetEventSystem().Unsubscribe(eventName, handler);
	}

	public void Unsubscribe(int eventName, int subscribeHandle, bool suppressWarnings = false)
	{
		int num = this.intraObjectRoutes.FindIndex((EventSystem.IntraObjectRoute route) => route.eventHash == eventName && route.handlerIndex == subscribeHandle);
		if (num == -1)
		{
			if (!suppressWarnings)
			{
				global::Debug.LogWarning("Failed to Unsubscribe event handler: " + EventSystem.intraObjectDispatcher[eventName][subscribeHandle].ToString() + "\nNot subscribed to event");
			}
			return;
		}
		if (this.currentlyTriggering == 0)
		{
			this.intraObjectRoutes.RemoveAtSwap(num);
			return;
		}
		this.dirty = true;
		this.intraObjectRoutes[num] = default(EventSystem.IntraObjectRoute);
	}

	public void Unsubscribe<ComponentType>(int eventName, EventSystem.IntraObjectHandler<ComponentType> handler, bool suppressWarnings)
	{
		List<EventSystem.IntraObjectHandlerBase> list;
		if (!EventSystem.intraObjectDispatcher.TryGetValue(eventName, out list))
		{
			if (!suppressWarnings)
			{
				global::Debug.LogWarning("Failed to Unsubscribe event handler: " + handler.ToString() + "\nNo subscriptions have been made to event");
			}
			return;
		}
		int num = list.IndexOf(handler);
		if (num == -1)
		{
			if (!suppressWarnings)
			{
				global::Debug.LogWarning("Failed to Unsubscribe event handler: " + handler.ToString() + "\nNot subscribed to event");
			}
			return;
		}
		this.Unsubscribe(eventName, num, suppressWarnings);
	}

	public void Unsubscribe(string[] eventNames, Action<object> handler)
	{
		for (int i = 0; i < eventNames.Length; i++)
		{
			int num = Hash.SDBMLower(eventNames[i]);
			this.Unsubscribe(num, handler);
		}
	}

	private static bool ENABLE_DETAILED_EVENT_PROFILE_INFO = false;

	private int nextId;

	private int currentlyTriggering;

	private bool dirty;

	private ArrayRef<EventSystem.SubscribedEntry> subscribedEvents;

	private ArrayRef<EventSystem.Entry> entries;

	private ArrayRef<EventSystem.IntraObjectRoute> intraObjectRoutes;

	private static Dictionary<int, List<EventSystem.IntraObjectHandlerBase>> intraObjectDispatcher = new Dictionary<int, List<EventSystem.IntraObjectHandlerBase>>();

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

	private struct IntraObjectRoute
	{
		public IntraObjectRoute(int eventHash, int handlerIndex)
		{
			this.eventHash = eventHash;
			this.handlerIndex = handlerIndex;
		}

		public bool IsValid()
		{
			return this.eventHash != 0;
		}

		public int eventHash;

		public int handlerIndex;
	}

	public abstract class IntraObjectHandlerBase
	{
		public abstract void Trigger(GameObject gameObject, object eventData);
	}

	public class IntraObjectHandler<ComponentType> : EventSystem.IntraObjectHandlerBase
	{
		public static bool IsStatic(Delegate del)
		{
			return del.Target == null || del.Target.GetType().GetCustomAttributes(false).OfType<CompilerGeneratedAttribute>()
				.Any<CompilerGeneratedAttribute>();
		}

		public IntraObjectHandler(Action<ComponentType, object> handler)
		{
			global::Debug.Assert(EventSystem.IntraObjectHandler<ComponentType>.IsStatic(handler));
			this.handler = handler;
		}

		public static implicit operator EventSystem.IntraObjectHandler<ComponentType>(Action<ComponentType, object> handler)
		{
			return new EventSystem.IntraObjectHandler<ComponentType>(handler);
		}

		public override void Trigger(GameObject gameObject, object eventData)
		{
			ListPool<ComponentType, EventSystem.IntraObjectHandler<ComponentType>>.PooledList pooledList = ListPool<ComponentType, EventSystem.IntraObjectHandler<ComponentType>>.Allocate();
			gameObject.GetComponents<ComponentType>(pooledList);
			foreach (ComponentType componentType in pooledList)
			{
				this.handler(componentType, eventData);
			}
			pooledList.Recycle();
		}

		public override string ToString()
		{
			return ((this.handler.Target != null) ? this.handler.Target.GetType().ToString() : "STATIC") + "." + this.handler.Method.ToString();
		}

		private Action<ComponentType, object> handler;
	}
}
