using System;
using UnityEngine;

public static class EventExtensions
{
	public static int Subscribe<ComponentType>(this GameObject go, int hash, EventSystem.IntraObjectHandler<ComponentType> handler) where ComponentType : Component
	{
		return KObjectManager.Instance.GetOrCreateObject(go).GetEventSystem().Subscribe<ComponentType>(hash, handler);
	}

	public static void Trigger(this GameObject go, int hash, object data = null)
	{
		KObject kobject = KObjectManager.Instance.Get(go);
		if (kobject != null && kobject.hasEventSystem)
		{
			kobject.GetEventSystem().Trigger(go, hash, data);
		}
	}
}
