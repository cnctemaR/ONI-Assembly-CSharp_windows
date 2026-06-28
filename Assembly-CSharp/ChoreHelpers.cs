using System;
using UnityEngine;

public static class ChoreHelpers
{
	public static GameObject CreateLocator(string name, Vector3 pos)
	{
		GameObject gameObject = new GameObject(name);
		KPrefabID kprefabID = gameObject.AddComponent<KPrefabID>();
		kprefabID.PrefabTag = ChoreHelpers.LocatorTag;
		gameObject.transform.SetPosition(pos);
		gameObject.AddComponent<Approachable>();
		return gameObject;
	}

	public static void DestroyLocator(GameObject locator)
	{
		if (locator != null)
		{
			locator.gameObject.DeleteObject();
		}
	}

	private static Tag LocatorTag = TagManager.Create("Locator", null);
}
