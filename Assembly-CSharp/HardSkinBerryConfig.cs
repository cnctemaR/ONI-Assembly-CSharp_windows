using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class HardSkinBerryConfig : IEntityConfig
{
	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_DLC_2;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("HardSkinBerry", ITEMS.FOOD.HARDSKINBERRY.NAME, ITEMS.FOOD.HARDSKINBERRY.DESC, 1f, false, Assets.GetAnim("iceBerry_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.CIRCLE, 0.25f, 0.25f, true, 0, SimHashes.Creature, null);
		EntityTemplates.ExtendEntityToFood(gameObject, FOOD.FOOD_TYPES.HARDSKINBERRY);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "HardSkinBerry";
}
