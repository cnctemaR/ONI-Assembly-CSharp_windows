using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class UrchinMeatConfig : IEntityConfig, IHasDlcRestrictions
{
	public string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC5;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity(UrchinMeatConfig.ID, global::STRINGS.ITEMS.FOOD.URCHINMEAT.NAME, global::STRINGS.ITEMS.FOOD.URCHINMEAT.DESC, 1f, false, Assets.GetAnim("ooohni_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, 0, SimHashes.Creature, null);
		EntityTemplates.ExtendEntityToFood(gameObject, FOOD.FOOD_TYPES.URCHINMEAT);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public static string ID = "UrchinMeat";

	public static ComplexRecipe recipe;
}
