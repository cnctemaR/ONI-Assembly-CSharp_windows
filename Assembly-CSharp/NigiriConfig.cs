using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class NigiriConfig : IEntityConfig, IHasDlcRestrictions
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
		return EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity("Nigiri", global::STRINGS.ITEMS.FOOD.NIGIRI.NAME, global::STRINGS.ITEMS.FOOD.NIGIRI.DESC, 1f, false, Assets.GetAnim("nigiri_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.9f, 0.8f, true, 0, SimHashes.Creature, null), FOOD.FOOD_TYPES.NIGIRI);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "Nigiri";

	public const string ANIM = "nigiri_kanim";

	public static ComplexRecipe recipe;
}
