using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class MicrowavedLettuceConfig : IEntityConfig
{
	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		return EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity("MicrowavedLettuce", ITEMS.FOOD.MICROWAVEDLETTUCE.NAME, ITEMS.FOOD.MICROWAVEDLETTUCE.DESC, 1f, false, Assets.GetAnim("sea_lettuce_leaves_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, 0, SimHashes.Creature, null), FOOD.FOOD_TYPES.MICROWAVED_LETTUCE);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "MicrowavedLettuce";

	public static ComplexRecipe recipe;
}
