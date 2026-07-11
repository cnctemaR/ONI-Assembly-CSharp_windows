using System;
using System.Collections.Generic;
using UnityEngine;

public static class KPrefabIDExtensions
{
	public static Tag PrefabID(this Component cmp)
	{
		return cmp.gameObject.PrefabID();
	}

	public static Tag PrefabID(this GameObject go)
	{
		return go.GetComponent<KPrefabID>().PrefabTag;
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
		go.GetComponent<KPrefabID>().SetTag(tag, set);
	}

	public static bool HasTags(this GameObject go, IList<Tag> tags)
	{
		KPrefabID component = go.GetComponent<KPrefabID>();
		return component.HasTags(tags);
	}
}
