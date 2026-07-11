using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class GeneShufflerRechargeConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "GeneShufflerRecharge";
		string text2 = ITEMS.INDUSTRIAL_PRODUCTS.GENE_SHUFFLER_RECHARGE.NAME;
		string text3 = ITEMS.INDUSTRIAL_PRODUCTS.GENE_SHUFFLER_RECHARGE.DESC;
		float num = 5f;
		bool flag = true;
		KAnimFile anim = Assets.GetAnim("vacillator_charge_kanim");
		string text4 = "object";
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.Front;
		EntityTemplates.CollisionShape collisionShape = EntityTemplates.CollisionShape.RECTANGLE;
		float num2 = 0.8f;
		float num3 = 0.6f;
		bool flag2 = true;
		List<Tag> list = new List<Tag> { GameTags.IndustrialIngredient };
		return EntityTemplates.CreateLooseEntity(text, text2, text3, num, flag, anim, text4, sceneLayer, collisionShape, num2, num3, flag2, 0, SimHashes.Creature, list);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "GeneShufflerRecharge";

	public static readonly Tag tag = TagManager.Create("GeneShufflerRecharge");

	public const float MASS = 5f;
}
