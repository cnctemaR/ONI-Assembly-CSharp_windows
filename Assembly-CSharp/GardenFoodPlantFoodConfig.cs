using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class GardenFoodPlantFoodConfig : IEntityConfig, IHasDlcRestrictions
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
		GameObject gameObject = EntityTemplates.CreateLooseEntity("GardenFoodPlantFood", global::STRINGS.ITEMS.FOOD.GARDENFOODPLANTFOOD.NAME, global::STRINGS.ITEMS.FOOD.GARDENFOODPLANTFOOD.DESC, 1f, false, Assets.GetAnim("spikefruit_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.CIRCLE, 0.25f, 0.25f, true, 0, SimHashes.Creature, null);
		EntityTemplates.ExtendEntityToFood(gameObject, FOOD.FOOD_TYPES.GARDENFOODPLANT);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "GardenFoodPlantFood";
}
