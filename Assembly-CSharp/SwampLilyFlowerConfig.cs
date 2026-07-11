using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class SwampLilyFlowerConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string id = SwampLilyFlowerConfig.ID;
		string text = ITEMS.INGREDIENTS.SWAMPLILYFLOWER.NAME;
		string text2 = ITEMS.INGREDIENTS.SWAMPLILYFLOWER.DESC;
		float num = 1f;
		bool flag = false;
		KAnimFile anim = Assets.GetAnim("swamplilyflower_kanim");
		string text3 = "object";
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.Front;
		EntityTemplates.CollisionShape collisionShape = EntityTemplates.CollisionShape.RECTANGLE;
		float num2 = 0.8f;
		float num3 = 0.4f;
		bool flag2 = true;
		List<Tag> list = new List<Tag> { GameTags.IndustrialIngredient };
		GameObject gameObject = EntityTemplates.CreateLooseEntity(id, text, text2, num, flag, anim, text3, sceneLayer, collisionShape, num2, num3, flag2, 0, SimHashes.Creature, list);
		gameObject.AddOrGet<EntitySplitter>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public static float SEEDS_PER_FRUIT = 1f;

	public static string ID = "SwampLilyFlower";
}
