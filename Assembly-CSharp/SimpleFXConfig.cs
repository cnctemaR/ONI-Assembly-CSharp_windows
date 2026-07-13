using System;
using UnityEngine;

public class SimpleFXConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity(SimpleFXConfig.ID, SimpleFXConfig.ID, false);
		gameObject.AddOrGet<KBatchedAnimController>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject go)
	{
	}

	public void OnSpawn(GameObject go)
	{
	}

	public static readonly string ID = "SimpleFX";
}
