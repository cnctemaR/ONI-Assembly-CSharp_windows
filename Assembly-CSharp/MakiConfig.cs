using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class MakiConfig : IEntityConfig, IHasDlcRestrictions
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
		return EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity("Maki", global::STRINGS.ITEMS.FOOD.MAKI.NAME, global::STRINGS.ITEMS.FOOD.MAKI.DESC, 1f, false, Assets.GetAnim("maki_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.9f, 0.6f, true, 0, SimHashes.Creature, null), FOOD.FOOD_TYPES.MAKI);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "Maki";

	public const string ANIM = "maki_kanim";

	public static ComplexRecipe recipe;
}
