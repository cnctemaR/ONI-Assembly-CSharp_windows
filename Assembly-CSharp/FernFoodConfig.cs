using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class FernFoodConfig : IEntityConfig, IHasDlcRestrictions
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
		return EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity(FernFoodConfig.ID, global::STRINGS.ITEMS.FOOD.FERNFOOD.NAME, global::STRINGS.ITEMS.FOOD.FERNFOOD.DESC, 1f, true, Assets.GetAnim("megafrond_grain_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, 0, SimHashes.Creature, null), FOOD.FOOD_TYPES.FERNFOOD);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public static string ID = "FernFood";
}
