using System;
using UnityEngine;

public static class EventExtensions
{
	public static int Subscribe<ComponentType>(this GameObject go, int hash, EventSystem.IntraObjectHandler<ComponentType> handler) where ComponentType : Component
	{
		return KObjectManager.Instance.GetOrCreateObject(go).GetOrCreateEventSystem().Subscribe<ComponentType>(hash, handler);
	}

	public static int Subscribe(this GameObject go, int hash, Action<object, object> handler, object handlerData)
	{
		return KObjectManager.Instance.GetOrCreateObject(go).GetOrCreateEventSystem().Subscribe(hash, handler, handlerData);
	}

	public static void Trigger(this GameObject go, int hash, object data = null)
	{
		KObject kobject = KObjectManager.Instance.Get(go);
		EventSystem eventSystem;
		if (kobject != null && kobject.GetEventSystem(out eventSystem))
		{
			eventSystem.Trigger(go, hash, data);
		}
	}

	[Obsolete("Use BoxingTrigger to avoid sended boxing object to garbage collection, be careful to unbox parameter in any handlers")]
	public static void Trigger<T>(this GameObject go, int hash, T data) where T : struct
	{
		go.Trigger(hash, data);
	}

	public static void BoxingTrigger(this GameObject go, int hash, bool data)
	{
		go.Trigger(hash, BoxedBools.Box(data));
	}

	public static void BoxingTrigger<T>(this GameObject go, int hash, T data) where T : struct
	{
		Boxed<T> boxed = Boxed<T>.Get(data);
		go.Trigger(hash, boxed);
		boxed.Release();
	}
}
