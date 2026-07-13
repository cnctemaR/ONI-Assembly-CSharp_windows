using System;
using STRINGS;
using UnityEngine;

public class CarePackageConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		return EntityTemplates.CreateLooseEntity(CarePackageConfig.ID, ITEMS.CARGO_CAPSULE.NAME, ITEMS.CARGO_CAPSULE.DESC, 1f, true, Assets.GetAnim("portal_carepackage_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 1f, 1f, false, 0, SimHashes.Creature, null);
	}

	public void OnPrefabInit(GameObject go)
	{
		go.AddOrGet<CarePackage>();
	}

	public void OnSpawn(GameObject go)
	{
	}

	public static readonly string ID = "CarePackage";
}
