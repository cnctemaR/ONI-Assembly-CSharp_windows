using System;
using UnityEngine;

public abstract class KGameObjectComponentManager<T> : KComponentManager<T>, IComponentManager where T : new()
{
	public int Count
	{
		get
		{
			return this.data.Count;
		}
	}

	public HandleVector<int>.Handle Add(GameObject go, T cmp)
	{
		return base.InternalAddComponent(go, cmp);
	}

	public bool Has(GameObject go)
	{
		return this.instanceHandleMap.ContainsKey(go);
	}

	public virtual void Remove(GameObject go)
	{
		base.InternalRemoveComponent(go);
	}

	public virtual void FixedUpdate(float dt)
	{
	}

	public virtual void Update(float dt)
	{
	}
}
