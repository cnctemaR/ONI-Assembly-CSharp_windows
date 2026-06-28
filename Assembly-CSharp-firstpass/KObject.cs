using System;
using UnityEngine;

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
			this.eventSystem.OnCleanUp();
			this.eventSystem = null;
		}
	}

	public EventSystem GetEventSystem()
	{
		if (this.eventSystem == null)
		{
			this.eventSystem = new EventSystem();
		}
		return this.eventSystem;
	}

	public int id { get; private set; }

	public bool hasEventSystem
	{
		get
		{
			return this.eventSystem != null;
		}
	}

	private EventSystem eventSystem;
}
