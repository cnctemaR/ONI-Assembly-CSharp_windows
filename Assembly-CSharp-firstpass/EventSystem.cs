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
		List<EventSystem.IntraObjectHandlerBase> list;
		if (EventSystem.intraObjectDispatcher.TryGetValue(hash, out list))
		{
			for (int num = 0; num != this.intraObjectRoutes.size; num++)
			{
				if (this.intraObjectRoutes[num].eventHash == hash)
				{
					list[this.intraObjectRoutes[num].handlerIndex].Trigger(go, data);
				}
			}
		}
		List<EventSystem.Entry> list2;
		if (this.entryMap.TryGetValue(hash, out list2))
		{
			int count = list2.Count;
			for (int i = 0; i < count; i++)
			{
				list2[i].Invoke(data);
			}
		}
		this.currentlyTriggering--;
		if (this.dirty && this.currentlyTriggering == 0)
		{
			this.dirty = false;
			List<EventSystem.Entry> list3 = null;
			int num2 = 0;
			int j = 0;
			while (j < this.pendingRemovals.Count)
			{
				EventSystem.RemovalEntry removalEntry = this.pendingRemovals[j];
				if (removalEntry.hash == num2)
				{
					goto IL_0115;
				}
				if (this.entryMap.TryGetValue(removalEntry.hash, out list3))
				{
					num2 = removalEntry.hash;
					goto IL_0115;
				}
				IL_0141:
				j++;
				continue;
				IL_0115:
				EventSystem.IdInfo idInfo;
				if (this.idToIndex.TryGetValue(removalEntry.id, out idInfo))
				{
					this.RemoveEntry(idInfo.hash, list3, idInfo.index);
					goto IL_0141;
				}
				goto IL_0141;
			}
			this.pendingRemovals.Clear();
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
				subscribedEntry.go.Unsubscribe(subscribedEntry.id);
			}
		}
		foreach (KeyValuePair<int, List<EventSystem.Entry>> keyValuePair in this.entryMap)
		{
			List<EventSystem.Entry> value = keyValuePair.Value;
			for (int j = 0; j < value.Count; j++)
			{
				EventSystem.Entry entry = value[j];
				entry.handler = null;
				entry.extHandler = null;
				value[j] = entry;
			}
			value.Clear();
		}
		this.entryMap.Clear();
		this.subscribedEvents.Clear();
		this.intraObjectRoutes.Clear();
		this.idToIndex.Clear();
	}

	public void UnregisterEvent(GameObject target, int id)
	{
		for (int i = 0; i < this.subscribedEvents.size; i++)
		{
			if (this.subscribedEvents[i].id == id && this.subscribedEvents[i].go == target)
			{
				this.subscribedEvents.RemoveAtSwap(i);
				return;
			}
		}
	}

	public void RegisterEvent(GameObject target, int id)
	{
		this.subscribedEvents.Add(new EventSystem.SubscribedEntry(target, id));
	}

	private void RemoveEntry(int hash, List<EventSystem.Entry> map_entries, int index)
	{
		if (this.currentlyTriggering == 0)
		{
			int id = map_entries[index].id;
			map_entries.RemoveAtSwap<EventSystem.Entry>(index);
			if (index < map_entries.Count)
			{
				int id2 = map_entries[index].id;
				EventSystem.IdInfo idInfo = this.idToIndex[id2];
				idInfo.index = index;
				this.idToIndex[id2] = idInfo;
			}
			this.idToIndex.Remove(id);
			return;
		}
		this.dirty = true;
		EventSystem.Entry entry = map_entries[index];
		entry.handler = null;
		entry.extHandler = null;
		map_entries[index] = entry;
		this.pendingRemovals.Add(new EventSystem.RemovalEntry(hash, entry.id));
	}

	public int Subscribe(int hash, Action<object> handler)
	{
		this.nextId = Mathf.Max(this.nextId + 1, 0);
		EventSystem.Entry entry = new EventSystem.Entry(hash, handler, this.nextId);
		List<EventSystem.Entry> list;
		if (!this.entryMap.TryGetValue(hash, out list))
		{
			list = new List<EventSystem.Entry>();
			this.entryMap.Add(hash, list);
		}
		list.Add(entry);
		this.idToIndex[entry.id] = new EventSystem.IdInfo(hash, list.Count - 1);
		return entry.id;
	}

	public int Subscribe(int hash, Action<object, object> handler, object handlerData)
	{
		this.nextId = Mathf.Max(this.nextId + 1, 0);
		EventSystem.Entry entry = new EventSystem.Entry(hash, handler, handlerData, this.nextId);
		List<EventSystem.Entry> list;
		if (!this.entryMap.TryGetValue(hash, out list))
		{
			list = new List<EventSystem.Entry>();
			this.entryMap.Add(hash, list);
		}
		list.Add(entry);
		this.idToIndex[entry.id] = new EventSystem.IdInfo(hash, list.Count - 1);
		return entry.id;
	}

	public void Unsubscribe(int hash, Action<object> handler)
	{
		List<EventSystem.Entry> list;
		if (this.entryMap.TryGetValue(hash, out list))
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].handler == handler)
				{
					this.RemoveEntry(hash, list, i);
					return;
				}
			}
		}
	}

	public void Unsubscribe(int id)
	{
		EventSystem.IdInfo idInfo;
		List<EventSystem.Entry> list;
		if (this.idToIndex.TryGetValue(id, out idInfo) && this.entryMap.TryGetValue(idInfo.hash, out list))
		{
			this.RemoveEntry(idInfo.hash, list, idInfo.index);
		}
	}

	public void Unsubscribe(ref int id)
	{
		this.Unsubscribe(id);
		id = -1;
	}

	public int Subscribe(GameObject target, int eventName, Action<object> handler)
	{
		int num = KObjectManager.Instance.GetOrCreateObject(target).GetOrCreateEventSystem().Subscribe(eventName, handler);
		this.RegisterEvent(target, num);
		return num;
	}

	public int Subscribe(GameObject target, int eventName, Action<object, object> handler, object handlerData)
	{
		int num = KObjectManager.Instance.GetOrCreateObject(target).GetOrCreateEventSystem().Subscribe(eventName, handler, handlerData);
		this.RegisterEvent(target, num);
		return num;
	}

	public int Subscribe<ComponentType>(int eventName, EventSystem.IntraObjectHandler<ComponentType> handler) where ComponentType : Component
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
		if (target == null)
		{
			return;
		}
		KObject kobject = KObjectManager.Instance.Get(target);
		EventSystem eventSystem;
		if (kobject == null || !kobject.GetEventSystem(out eventSystem))
		{
			return;
		}
		List<EventSystem.Entry> list;
		if (eventSystem.entryMap.TryGetValue(eventName, out list))
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].handler == handler)
				{
					this.UnregisterEvent(target, list[i].id);
					break;
				}
			}
		}
		eventSystem.Unsubscribe(eventName, handler);
	}

	public void Unsubscribe(GameObject target, int id)
	{
		this.UnregisterEvent(target, id);
		if (target != null)
		{
			KObject kobject = KObjectManager.Instance.Get(target);
			EventSystem eventSystem;
			if (kobject == null || !kobject.GetEventSystem(out eventSystem))
			{
				return;
			}
			eventSystem.Unsubscribe(id);
		}
	}

	public void Unsubscribe(GameObject target, ref int id)
	{
		this.UnregisterEvent(target, id);
		if (target != null)
		{
			KObject kobject = KObjectManager.Instance.Get(target);
			EventSystem eventSystem;
			if (kobject == null || !kobject.GetEventSystem(out eventSystem))
			{
				return;
			}
			eventSystem.Unsubscribe(id);
		}
		id = -1;
	}

	public void Unsubscribe(int eventName, int subscribeHandle, bool suppressWarnings = false)
	{
		int num = -1;
		for (int i = 0; i < this.intraObjectRoutes.Count; i++)
		{
			EventSystem.IntraObjectRoute intraObjectRoute = this.intraObjectRoutes[i];
			if (intraObjectRoute.eventHash == eventName && intraObjectRoute.handlerIndex == subscribeHandle)
			{
				num = i;
				break;
			}
		}
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

	public void Unsubscribe<ComponentType>(int eventName, EventSystem.IntraObjectHandler<ComponentType> handler, bool suppressWarnings) where ComponentType : Component
	{
		List<EventSystem.IntraObjectHandlerBase> list;
		if (!EventSystem.intraObjectDispatcher.TryGetValue(eventName, out list))
		{
			if (!suppressWarnings)
			{
				global::Debug.LogWarning(string.Format("Failed to Unsubscribe event handler: {0}\nNo subscriptions have been made to event {1}", handler.ToString(), eventName));
			}
			return;
		}
		int num = list.IndexOf(handler);
		if (num == -1)
		{
			if (!suppressWarnings)
			{
				global::Debug.LogWarning(string.Format("Failed to Unsubscribe event handler: {0}\nNot subscribed to event {1}", handler.ToString(), eventName));
			}
			return;
		}
		this.Unsubscribe(eventName, num, suppressWarnings);
	}

	[Obsolete]
	public void Unsubscribe(string[] eventNames, Action<object> handler)
	{
		for (int i = 0; i < eventNames.Length; i++)
		{
			int num = Hash.SDBMLower(eventNames[i]);
			this.Unsubscribe(num, handler);
		}
	}

	public const int InvalidHandle = -1;

	private int nextId;

	private int currentlyTriggering;

	private bool dirty;

	private ArrayRef<EventSystem.SubscribedEntry> subscribedEvents;

	private Dictionary<int, List<EventSystem.Entry>> entryMap = new Dictionary<int, List<EventSystem.Entry>>();

	private Dictionary<int, EventSystem.IdInfo> idToIndex = new Dictionary<int, EventSystem.IdInfo>();

	private ArrayRef<EventSystem.IntraObjectRoute> intraObjectRoutes;

	private ArrayRef<EventSystem.RemovalEntry> pendingRemovals;

	private static Dictionary<int, List<EventSystem.IntraObjectHandlerBase>> intraObjectDispatcher = new Dictionary<int, List<EventSystem.IntraObjectHandlerBase>>();

	private LoggerFIO log;

	private struct Entry
	{
		public Entry(int hash, Action<object> handler, int id)
		{
			this.handler = handler;
			this.hash = hash;
			this.id = id;
			this.extended = false;
			this.extHandler = null;
			this.context = null;
		}

		public Entry(int hash, Action<object, object> handler, object data, int id)
		{
			this.extended = true;
			this.handler = null;
			this.extHandler = handler;
			this.context = data;
			this.id = id;
			this.hash = hash;
		}

		public void Invoke(object data)
		{
			if (this.extended)
			{
				if (this.extHandler != null)
				{
					this.extHandler(this.context, data);
					return;
				}
			}
			else if (this.handler != null)
			{
				this.handler(data);
			}
		}

		public Action<object> handler;

		public Action<object, object> extHandler;

		public object context;

		public int hash;

		public int id;

		public bool extended;
	}

	private struct SubscribedEntry
	{
		public SubscribedEntry(GameObject go, int id)
		{
			this.go = go;
			this.id = id;
		}

		public GameObject go;

		public int id;
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

	private struct RemovalEntry
	{
		public RemovalEntry(int hash, int id)
		{
			this.hash = hash;
			this.id = id;
		}

		public int hash;

		public int id;
	}

	private struct IdInfo
	{
		public IdInfo(int hash, int index)
		{
			this.hash = hash;
			this.index = index;
		}

		public int hash;

		public int index;
	}

	public abstract class IntraObjectHandlerBase
	{
		public abstract void Trigger(GameObject gameObject, object eventData);
	}

	public class IntraObjectHandler<ComponentType> : EventSystem.IntraObjectHandlerBase where ComponentType : Component
	{
		public static bool IsStatic(Delegate del)
		{
			return del.Target == null || del.Target.GetType().GetCustomAttributes(false).OfType<CompilerGeneratedAttribute>()
				.Any<CompilerGeneratedAttribute>();
		}

		public IntraObjectHandler(Action<ComponentType, object> handler)
		{
			global::Debug.Assert(EventSystem.IntraObjectHandler<ComponentType>.IsStatic(handler), "IntraObjectHandler method must be static to avoid allocations");
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
