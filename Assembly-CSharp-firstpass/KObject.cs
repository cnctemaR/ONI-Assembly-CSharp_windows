using System;
using UnityEngine;

public class KObject
{
	public KObject(GameObject go)
	{
		this.go = go;
		this.id = go.GetInstanceID();
	}

	public void OnCleanUp()
	{
		if (this.eventSystem != null)
		{
			this.eventSystem.OnCleanUp();
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

	public GameObject go { get; private set; }

	private EventSystem eventSystem;
}
