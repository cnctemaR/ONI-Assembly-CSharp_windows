using System;
using System.Reflection;
using UnityEngine;

public class CmpFns
{
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

	public CmpFns(Type type)
	{
		Type[] array = new Type[] { type };
		this.mFindOrAddFn = this.GetMethod("FindOrAddComponent", array);
		this.mFindFn = this.GetMethod("FindComponent", array);
		this.mRequireFn = this.GetMethod("RequireComponent", array);
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
			global::Debug.LogError(ex);
			for (int i = 0; i < type_array.Length; i++)
			{
				global::Debug.Log(type_array[i]);
			}
		}
		return (Func<KMonoBehaviour, Component>)Delegate.CreateDelegate(typeof(Func<KMonoBehaviour, Component>), methodInfo);
	}

	public Func<KMonoBehaviour, Component> mFindOrAddFn;

	public Func<KMonoBehaviour, Component> mFindFn;

	public Func<KMonoBehaviour, Component> mRequireFn;
}
