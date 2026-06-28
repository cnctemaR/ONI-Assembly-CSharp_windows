using System;
using System.Reflection;
using UnityEngine;

public class CmpFns
{
	public CmpFns(Type type)
	{
		Type[] array = new Type[] { type };
		this.mFindOrAddFn = this.GetMethod("FindOrAddComponent", array);
		this.mFindFn = this.GetMethod("FindComponent", array);
		this.mRequireFn = this.GetMethod("RequireComponent", array);
	}

	public static Component FindComponent<T>(KMonoBehaviour c) where T : KMonoBehaviour
	{
		return c.FindComponent<T>();
	}

	public static Component RequireComponent<T>(KMonoBehaviour c) where T : KMonoBehaviour
	{
		return c.RequireComponent<T>();
	}

	public static Component FindOrAddComponent<T>(KMonoBehaviour c) where T : KMonoBehaviour
	{
		return c.FindOrAddComponent<T>();
	}

	private Func<KMonoBehaviour, Component> GetMethod(string name, Type[] type_array)
	{
		MethodInfo method = typeof(CmpFns).GetMethod(name);
		MethodInfo methodInfo = method.MakeGenericMethod(type_array);
		return (Func<KMonoBehaviour, Component>)Delegate.CreateDelegate(typeof(Func<KMonoBehaviour, Component>), methodInfo);
	}

	public Func<KMonoBehaviour, Component> mFindOrAddFn;

	public Func<KMonoBehaviour, Component> mFindFn;

	public Func<KMonoBehaviour, Component> mRequireFn;
}
