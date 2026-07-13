using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class VineFruitConfig : IEntityConfig, IHasDlcRestrictions
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
		GameObject gameObject = EntityTemplates.CreateLooseEntity(VineFruitConfig.ID, global::STRINGS.ITEMS.FOOD.VINEFRUIT.NAME, global::STRINGS.ITEMS.FOOD.VINEFRUIT.DESC, 1f, false, Assets.GetAnim("ova_melon_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, 0, SimHashes.Creature, null);
		EntityTemplates.ExtendEntityToFood(gameObject, FOOD.FOOD_TYPES.VINEFRUIT);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public static string ID = "VineFruit";

	public const float KCalPerUnit = 325000f;
}
