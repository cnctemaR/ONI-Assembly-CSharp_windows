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
		HandleVector<int>.Handle handle = this.GetHandle(go);
		KComponentManager<T>.CleanupInfo cleanupInfo = new KComponentManager<T>.CleanupInfo(go, handle);
		if (!KComponentCleanUp.InCleanUpPhase)
		{
			this.cleanupList.Add(cleanupInfo);
		}
		else
		{
			base.RemoveFromCleanupList(go);
			this.OnCleanUp(handle);
			base.InternalRemoveComponent(cleanupInfo);
		}
	}

	public HandleVector<int>.Handle GetHandle(GameObject obj)
	{
		return base.GetHandle(obj);
	}

	public HandleVector<int>.Handle GetHandle(MonoBehaviour obj)
	{
		return base.GetHandle(obj.gameObject);
	}
}
