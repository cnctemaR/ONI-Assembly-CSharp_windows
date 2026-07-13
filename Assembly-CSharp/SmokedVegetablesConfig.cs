using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class SmokedVegetablesConfig : IEntityConfig, IHasDlcRestrictions
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
		return EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity("SmokedVegetables", global::STRINGS.ITEMS.FOOD.SMOKEDVEGETABLES.NAME, global::STRINGS.ITEMS.FOOD.SMOKEDVEGETABLES.DESC, 1f, false, Assets.GetAnim("smokedvegetables_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.7f, true, 0, SimHashes.Creature, null), FOOD.FOOD_TYPES.SMOKED_VEGETABLES);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "SmokedVegetables";

	public static ComplexRecipe recipe;
}
