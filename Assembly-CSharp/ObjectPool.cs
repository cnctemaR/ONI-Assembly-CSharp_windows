using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
	public ObjectPool(Func<GameObject> instantiator, int initial_count = 0)
	{
		this.instantiator = instantiator;
		this.unused = new List<GameObject>();
		for (int i = 0; i < initial_count; i++)
		{
			this.unused.Add(instantiator());
		}
	}

	public GameObject GetInstance()
	{
		GameObject gameObject;
		if (this.unused.Count > 0)
		{
			gameObject = this.unused[this.unused.Count - 1];
			this.unused.RemoveAt(this.unused.Count - 1);
		}
		else
		{
			gameObject = this.instantiator();
		}
		return gameObject;
	}

	public void ReleaseInstance(GameObject go)
	{
		this.unused.Add(go);
	}

	public void Destroy()
	{
		for (int i = 0; i < this.unused.Count; i++)
		{
			global::UnityEngine.Object.Destroy(this.unused[i]);
		}
		this.unused.Clear();
	}

	private List<GameObject> unused;

	private Func<GameObject> instantiator;
}
