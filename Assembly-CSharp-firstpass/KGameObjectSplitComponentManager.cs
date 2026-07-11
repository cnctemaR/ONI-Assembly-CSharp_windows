using System;
using UnityEngine;

public abstract class KGameObjectSplitComponentManager<Header, Payload> : KSplitComponentManager<Header, Payload> where Header : new() where Payload : new()
{
	public HandleVector<int>.Handle Add(GameObject go, Header header, ref Payload payload)
	{
		return base.InternalAddComponent(go, header, ref payload);
	}

	public virtual void Remove(GameObject go)
	{
		HandleVector<int>.Handle handle = this.GetHandle(go);
		KSplitComponentManager<Header, Payload>.CleanupInfo cleanupInfo = new KSplitComponentManager<Header, Payload>.CleanupInfo(go, handle);
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
