using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class CrabShellConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "CrabShell";
		string text2 = ITEMS.INDUSTRIAL_PRODUCTS.CRAB_SHELL.NAME;
		string text3 = ITEMS.INDUSTRIAL_PRODUCTS.CRAB_SHELL.DESC;
		float num = 10f;
		bool flag = true;
		KAnimFile anim = Assets.GetAnim("crabshells_large_kanim");
		string text4 = "object";
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.Front;
		EntityTemplates.CollisionShape collisionShape = EntityTemplates.CollisionShape.RECTANGLE;
		float num2 = 0.9f;
		float num3 = 0.6f;
		bool flag2 = true;
		List<Tag> list = new List<Tag>
		{
			GameTags.IndustrialIngredient,
			GameTags.Organics
		};
		GameObject gameObject = EntityTemplates.CreateLooseEntity(text, text2, text3, num, flag, anim, text4, sceneLayer, collisionShape, num2, num3, flag2, 0, SimHashes.Creature, list);
		gameObject.AddOrGet<EntitySplitter>();
		gameObject.AddOrGet<SimpleMassStatusItem>();
		EntityTemplates.CreateAndRegisterCompostableFromPrefab(gameObject);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "CrabShell";

	public static readonly Tag TAG = TagManager.Create("CrabShell");

	public const float MASS = 10f;
}
