using System;
using UnityEngine;

public static class TagExtensions
{
	public static GameObject Prefab(this Tag tag)
	{
		return Assets.GetPrefab(tag);
	}

	public static string ProperName(this Tag tag)
	{
		return TagManager.GetProperName(tag);
	}
}
