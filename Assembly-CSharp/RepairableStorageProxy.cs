using System;
using UnityEngine;

public class RepairableStorageProxy : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity(RepairableStorageProxy.ID, RepairableStorageProxy.ID);
		gameObject.AddOrGet<Storage>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject go)
	{
	}

	public void OnSpawn(GameObject go)
	{
	}

	public static string ID = "RepairableStorageProxy";
}
