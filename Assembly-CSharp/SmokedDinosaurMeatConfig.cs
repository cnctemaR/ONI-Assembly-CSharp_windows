using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class SmokedDinosaurMeatConfig : IEntityConfig, IHasDlcRestrictions
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
		return EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity("SmokedDinosaurMeat", global::STRINGS.ITEMS.FOOD.SMOKEDDINOSAURMEAT.NAME, global::STRINGS.ITEMS.FOOD.SMOKEDDINOSAURMEAT.DESC, 1f, false, Assets.GetAnim("dinobrisket_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, 0, SimHashes.Creature, null), FOOD.FOOD_TYPES.SMOKED_DINOSAURMEAT);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "SmokedDinosaurMeat";

	public static ComplexRecipe recipe;
}
