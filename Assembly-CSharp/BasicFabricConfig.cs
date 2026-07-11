using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class BasicFabricConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string id = BasicFabricConfig.ID;
		string text = ITEMS.INDUSTRIAL_PRODUCTS.BASIC_FABRIC.NAME;
		string text2 = ITEMS.INDUSTRIAL_PRODUCTS.BASIC_FABRIC.DESC;
		float num = 1f;
		bool flag = true;
		KAnimFile anim = Assets.GetAnim("swampreedwool_kanim");
		string text3 = "object";
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.BuildingBack;
		EntityTemplates.CollisionShape collisionShape = EntityTemplates.CollisionShape.RECTANGLE;
		float num2 = 0.8f;
		float num3 = 0.45f;
		bool flag2 = true;
		List<Tag> list = new List<Tag> { GameTags.IndustrialIngredient };
		GameObject gameObject = EntityTemplates.CreateLooseEntity(id, text, text2, num, flag, anim, text3, sceneLayer, collisionShape, num2, num3, flag2, SimHashes.Creature, list);
		gameObject.AddOrGet<EntitySplitter>();
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
