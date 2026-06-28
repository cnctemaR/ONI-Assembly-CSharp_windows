using System;
using STRINGS;
using UnityEngine;

public class BasicFabricConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity(BasicFabricConfig.ID, ITEMS.INDUSTRIAL_PRODUCTS.BASIC_FABRIC.NAME, ITEMS.INDUSTRIAL_PRODUCTS.BASIC_FABRIC.DESC, 1f, true, Assets.GetAnim("swampreedwool_kanim"), "object", Grid.SceneLayer.BuildingBack, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.45f, true, SimHashes.Creature, null);
		gameObject.AddOrGet<EntitySplitter>();
		gameObject.GetComponent<KPrefabID>().AddPrefabTag(GameTags.IndustrialIngredient);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public static string ID = "BasicFabric";
}
