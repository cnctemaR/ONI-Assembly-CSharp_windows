using System;
using UnityEngine;

public class SleepLocator : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity(SleepLocator.ID, SleepLocator.ID, false);
		gameObject.AddTag(GameTags.NotAPrefab);
		gameObject.AddOrGet<Approachable>();
		gameObject.AddOrGet<Sleepable>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject go)
	{
	}

	public void OnSpawn(GameObject go)
	{
	}

	public static readonly string ID = "SleepLocator";
}
