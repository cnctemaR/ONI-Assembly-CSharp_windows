using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class WormBasicFoodConfig : IEntityConfig, IHasDlcRestrictions
{
	public string[] GetRequiredDlcIds()
	{
		return DlcManager.EXPANSION1;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public GameObject CreatePrefab()
	{
		return EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity("WormBasicFood", global::STRINGS.ITEMS.FOOD.WORMBASICFOOD.NAME, global::STRINGS.ITEMS.FOOD.WORMBASICFOOD.DESC, 1f, false, Assets.GetAnim("wormwood_roast_nuts_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.7f, 0.4f, true, 0, SimHashes.Creature, null), FOOD.FOOD_TYPES.WORMBASICFOOD);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "WormBasicFood";

	public static ComplexRecipe recipe;
}
