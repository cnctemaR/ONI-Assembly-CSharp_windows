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

	public static Component FindComponent<T>(MonoBehaviour c) where T : Component
	{
		return c.FindComponent<T>();
	}

	public static Component RequireComponent<T>(MonoBehaviour c) where T : Component
	{
		return c.RequireComponent<T>();
	}

	public static Component FindOrAddComponent<T>(MonoBehaviour c) where T : Component
	{
		return c.FindOrAddComponent<T>();
	}

	private Func<KMonoBehaviour, Component> GetMethod(string name, Type[] type_array)
	{
		MethodInfo method = typeof(CmpFns).GetMethod(name);
		MethodInfo methodInfo = null;
		try
		{
			methodInfo = method.MakeGenericMethod(type_array);
		}
		catch (Exception ex)
		{
			global::Debug.LogError(ex, null);
			foreach (Type type in type_array)
			{
				global::Debug.Log(type, null);
			}
		}
		return (Func<KMonoBehaviour, Component>)Delegate.CreateDelegate(typeof(Func<KMonoBehaviour, Component>), methodInfo);
	}

	public Func<KMonoBehaviour, Component> mFindOrAddFn;

	public Func<KMonoBehaviour, Component> mFindFn;

	public Func<KMonoBehaviour, Component> mRequireFn;
}
