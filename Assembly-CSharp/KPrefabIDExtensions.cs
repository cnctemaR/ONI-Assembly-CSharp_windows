using System;
using System.Collections.Generic;
using UnityEngine;

public static class KPrefabIDExtensions
{
	public static T CloneOriginalPrefab<T>(this T cmp) where T : Component
	{
		KPrefabID component = cmp.GetComponent<KPrefabID>();
		GameObject prefab = Assets.GetPrefab(component.PrefabTag);
		GameObject gameObject = Util.KInstantiate(prefab, null, null);
		gameObject.transform.SetPosition(cmp.transform.GetPosition());
		gameObject.transform.parent = cmp.transform.parent;
		return gameObject.GetComponent<T>();
	}

	public static Tag PrefabID(this Component cmp)
	{
		return cmp.gameObject.PrefabID();
	}

	public static Tag PrefabID(this GameObject go)
	{
		return go.GetComponent<KPrefabID>().PrefabTag;
	}

	public static int PrefabIDHash(this Component cmp)
	{
		return cmp.gameObject.GetComponent<KPrefabID>().GetHashCode();
	}

	public static bool HasTag(this Component cmp, Tag tag)
	{
		return cmp.gameObject.HasTag(tag);
	}

	public static bool HasTag(this GameObject go, Tag tag)
	{
		return go.GetComponent<KPrefabID>().HasTag(tag);
	}

	public static void AddTag(this GameObject go, Tag tag)
	{
		go.GetComponent<KPrefabID>().AddTag(tag);
	}

	public static void RemoveTag(this GameObject go, Tag tag)
	{
		go.GetComponent<KPrefabID>().RemoveTag(tag);
	}

	public static void SetTag(this GameObject go, Tag tag, bool set)
	{
		if (set)
		{
			go.AddTag(tag);
		}
		else
		{
			go.RemoveTag(tag);
		}
	}

	public static bool HasTags(this GameObject go, IList<Tag> tags)
	{
		KPrefabID component = go.GetComponent<KPrefabID>();
		return component.HasTags(tags);
	}

	public static string DebugName(this Component cmp)
	{
		return cmp.gameObject.DebugName();
	}

	public static string DebugName(this GameObject go)
	{
		return go.GetComponent<KPrefabID>().GetDebugName();
	}
}
