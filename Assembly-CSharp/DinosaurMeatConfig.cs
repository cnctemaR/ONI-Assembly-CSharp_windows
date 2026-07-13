using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class DinosaurMeatConfig : IEntityConfig, IHasDlcRestrictions
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
		GameObject gameObject = EntityTemplates.CreateLooseEntity("DinosaurMeat", global::STRINGS.ITEMS.FOOD.DINOSAURMEAT.NAME, global::STRINGS.ITEMS.FOOD.DINOSAURMEAT.DESC, 1f, false, Assets.GetAnim("dinomeat_raw_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.6f, true, 0, SimHashes.Creature, null);
		EntityTemplates.ExtendEntityToFood(gameObject, FOOD.FOOD_TYPES.DINOSAURMEAT);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "DinosaurMeat";
}
