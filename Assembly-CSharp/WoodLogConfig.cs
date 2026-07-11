using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class WoodLogConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "WoodLog";
		string text2 = ITEMS.INDUSTRIAL_PRODUCTS.WOOD.NAME;
		string text3 = ITEMS.INDUSTRIAL_PRODUCTS.WOOD.DESC;
		float num = 1f;
		bool flag = false;
		KAnimFile anim = Assets.GetAnim("wood_kanim");
		string text4 = "object";
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.Front;
		EntityTemplates.CollisionShape collisionShape = EntityTemplates.CollisionShape.CIRCLE;
		float num2 = 0.35f;
		float num3 = 0.35f;
		bool flag2 = true;
		List<Tag> list = new List<Tag>
		{
			GameTags.IndustrialIngredient,
			GameTags.Organics,
			GameTags.BuildingWood
		};
		GameObject gameObject = EntityTemplates.CreateLooseEntity(text, text2, text3, num, flag, anim, text4, sceneLayer, collisionShape, num2, num3, flag2, 0, SimHashes.Creature, list);
		gameObject.AddOrGet<EntitySplitter>();
		gameObject.AddOrGet<SimpleMassStatusItem>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "WoodLog";

	public static readonly Tag TAG = TagManager.Create("WoodLog");
}
