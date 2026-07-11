using System;
using UnityEngine;

public class TargetLocator : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity(TargetLocator.ID, TargetLocator.ID, false);
		gameObject.AddOrGet<KPrefabID>().PrefabTags = new Tag[] { GameTags.NotAPrefab };
		return gameObject;
	}

	public void OnPrefabInit(GameObject go)
	{
	}

	public void OnSpawn(GameObject go)
	{
	}

	public static readonly string ID = "TargetLocator";
}
