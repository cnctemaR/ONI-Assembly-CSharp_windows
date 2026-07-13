using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class ButterflyFoodConfig : IEntityConfig, IHasDlcRestrictions
{
	public string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC4;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public GameObject CreatePrefab()
	{
		return EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity("ButterflyFood", global::STRINGS.ITEMS.FOOD.BUTTERFLYFOOD.NAME, global::STRINGS.ITEMS.FOOD.BUTTERFLYFOOD.DESC, 1f, false, Assets.GetAnim("fried_mimillet_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.85f, 0.75f, true, 0, SimHashes.Creature, null), FOOD.FOOD_TYPES.BUTTERFLYFOOD);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "ButterflyFood";

	public static ComplexRecipe recipe;
}
