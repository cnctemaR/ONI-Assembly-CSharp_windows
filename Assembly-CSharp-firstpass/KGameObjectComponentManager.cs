using System;
using UnityEngine;

public abstract class KGameObjectComponentManager<T> : KComponentManager<T> where T : new()
{
	public HandleVector<int>.Handle Add(GameObject go, T cmp)
	{
		return base.InternalAddComponent(go, cmp);
	}

	public virtual void Remove(GameObject go)
	{
		if (!KComponentCleanUp.InCleanUpPhase)
		{
			this.cleanupList.Add(go);
		}
		else
		{
			base.RemoveFromCleanupList(go);
			HandleVector<int>.Handle handle = base.GetHandle(go);
			this.OnCleanUp(handle);
			base.InternalRemoveComponent(go);
		}
	}
}
