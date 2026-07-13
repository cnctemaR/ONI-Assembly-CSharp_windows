using System;
using UnityEngine;
using UnityEngine.Pool;

public class GameObjectPool
{
	public GameObjectPool(Func<GameObject> instantiator, Action<GameObject> recycler, int initial_count = 0)
	{
		this.impl = new ObjectPool<GameObject>(instantiator, null, recycler, null, false, initial_count, 10000);
	}

	public GameObject GetInstance()
	{
		return this.impl.Get();
	}

	public void ReleaseInstance(GameObject instance)
	{
		if (object.Equals(instance, null))
		{
			return;
		}
		this.impl.Release(instance);
	}

	private ObjectPool<GameObject> impl;
}
