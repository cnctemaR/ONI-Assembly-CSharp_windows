using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class KMonoBehaviourExtensions
{
	public static void Subscribe(this GameObject go, int hash, EventSystem.EventHandler handler)
	{
		KMonoBehaviour component = go.GetComponent<KMonoBehaviour>();
		if (component != null)
		{
			component.Subscribe(hash, handler);
		}
	}

	public static void Subscribe(this GameObject go, GameObject target, int hash, EventSystem.EventHandler handler)
	{
		KMonoBehaviour component = go.GetComponent<KMonoBehaviour>();
		if (component != null)
		{
			component.Subscribe(target, hash, handler);
		}
	}

	public static void Unsubscribe(this GameObject go, int hash, EventSystem.EventHandler handler)
	{
		KMonoBehaviour component = go.GetComponent<KMonoBehaviour>();
		if (component != null)
		{
			component.Unsubscribe(hash, handler);
		}
	}

	public static void Unsubscribe(this GameObject go, GameObject target, int hash, EventSystem.EventHandler handler)
	{
		KMonoBehaviour component = go.GetComponent<KMonoBehaviour>();
		if (component != null)
		{
			component.Unsubscribe(target, hash, handler);
		}
	}

	public static T GetComponentInChildrenOnly<T>(this GameObject go) where T : Component
	{
		T[] componentsInChildren = go.GetComponentsInChildren<T>();
		foreach (T t in componentsInChildren)
		{
			if (t.gameObject != go)
			{
				return t;
			}
		}
		return (T)((object)null);
	}

	public static T[] GetComponentsInChildrenOnly<T>(this GameObject go) where T : Component
	{
		List<T> list = new List<T>();
		list.AddRange(go.GetComponentsInChildren<T>());
		list.RemoveAll((T t) => t.gameObject == go);
		return list.ToArray();
	}

	public static void SetAlpha(this Image img, float alpha)
	{
		Color color = img.color;
		color.a = alpha;
		img.color = color;
	}

	public static void SetAlpha(this Text txt, float alpha)
	{
		Color color = txt.color;
		color.a = alpha;
		txt.color = color;
	}
}
