using System;
using UnityEngine;
using UnityEngine.Pool;

public class KObject
{
	public KObject(GameObject go)
	{
		this.id = go.GetInstanceID();
	}

	~KObject()
	{
		this.OnCleanUp();
	}

	public void OnCleanUp()
	{
		if (this.eventSystem != null)
		{
			KObject.eventSystemPool.Release(this.eventSystem);
			this.eventSystem = null;
		}
	}

	public EventSystem GetOrCreateEventSystem()
	{
		if (this.eventSystem == null)
		{
			this.eventSystem = KObject.eventSystemPool.Get();
		}
		return this.eventSystem;
	}

	public bool GetEventSystem(out EventSystem evtSys)
	{
		evtSys = this.eventSystem;
		return this.hasEventSystem;
	}

	public int id { get; private set; }

	public bool hasEventSystem
	{
		get
		{
			return this.eventSystem != null;
		}
	}

	private static ObjectPool<EventSystem> eventSystemPool = new ObjectPool<EventSystem>(() => new EventSystem(), null, delegate(EventSystem es)
	{
		es.OnCleanUp();
	}, null, false, 32, 10000);

	private EventSystem eventSystem;
}
