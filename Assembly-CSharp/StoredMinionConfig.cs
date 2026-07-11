using System;
using Klei.AI;
using UnityEngine;

public class StoredMinionConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity(StoredMinionConfig.ID, StoredMinionConfig.ID, true);
		gameObject.AddOrGet<SaveLoadRoot>();
		gameObject.AddOrGet<KPrefabID>().AddTag(StoredMinionConfig.ID);
		gameObject.AddOrGet<Equipment>();
		gameObject.AddOrGet<Ownables>();
		gameObject.AddOrGet<Traits>();
		gameObject.AddOrGet<Schedulable>();
		gameObject.AddOrGet<StoredMinionIdentity>();
		KSelectable kselectable = gameObject.AddOrGet<KSelectable>();
		kselectable.IsSelectable = false;
		return gameObject;
	}

	public void OnPrefabInit(GameObject go)
	{
	}

	public void OnSpawn(GameObject go)
	{
	}

	public static string ID = "StoredMinion";
}
