using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class ResearchDatabankConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "ResearchDatabank";
		string text2 = ITEMS.INDUSTRIAL_PRODUCTS.RESEARCH_DATABANK.NAME;
		string text3 = ITEMS.INDUSTRIAL_PRODUCTS.RESEARCH_DATABANK.DESC;
		float num = 1f;
		bool flag = true;
		KAnimFile anim = Assets.GetAnim("floppy_disc_kanim");
		string text4 = "object";
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.Front;
		EntityTemplates.CollisionShape collisionShape = EntityTemplates.CollisionShape.CIRCLE;
		float num2 = 0.35f;
		float num3 = 0.35f;
		bool flag2 = true;
		List<Tag> list = new List<Tag>
		{
			GameTags.IndustrialIngredient,
			GameTags.Experimental
		};
		GameObject gameObject = EntityTemplates.CreateLooseEntity(text, text2, text3, num, flag, anim, text4, sceneLayer, collisionShape, num2, num3, flag2, SimHashes.Creature, list);
		EntitySplitter entitySplitter = gameObject.AddOrGet<EntitySplitter>();
		entitySplitter.maxStackSize = (float)ROCKETRY.DESTINATION_RESEARCH.BASIC;
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "ResearchDatabank";

	public static readonly Tag TAG = TagManager.Create("ResearchDatabank");

	public const float MASS = 1f;
}
