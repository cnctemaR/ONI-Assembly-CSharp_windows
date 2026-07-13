using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class SmokedFish : IEntityConfig, IHasDlcRestrictions
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
		return EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity("SmokedFish", global::STRINGS.ITEMS.FOOD.SMOKEDFISH.NAME, global::STRINGS.ITEMS.FOOD.SMOKEDFISH.DESC, 1f, false, Assets.GetAnim("smokedfish_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, 0, SimHashes.Creature, null), FOOD.FOOD_TYPES.SMOKED_FISH);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "SmokedFish";

	public static ComplexRecipe recipe;
}
