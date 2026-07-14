using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class MusselTongueConfig : IEntityConfig, IHasDlcRestrictions
{
	public GameObject CreatePrefab()
	{
		return EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity(MusselTongueConfig.ID, global::STRINGS.ITEMS.FOOD.MUSSELTONGUE.NAME, global::STRINGS.ITEMS.FOOD.MUSSELTONGUE.DESC, 1f, false, Assets.GetAnim("musseltongue_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.3f, true, 0, SimHashes.Creature, null), FOOD.FOOD_TYPES.MUSSELTONGUE);
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC5;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public static string ID = "MusselTongue";
}
