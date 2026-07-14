using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class SaltySticksFoodConfig : IEntityConfig, IHasDlcRestrictions
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("SaltySticksFood", global::STRINGS.ITEMS.FOOD.SALTYSTICKSFOOD.NAME, global::STRINGS.ITEMS.FOOD.SALTYSTICKSFOOD.DESC, 1f, false, Assets.GetAnim("saltysticksfood_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.9f, 0.37f, true, 0, SimHashes.Creature, null);
		EntityTemplates.ExtendEntityToFood(gameObject, FOOD.FOOD_TYPES.SALTYSTICKSFOOD);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC5;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public const string ID = "SaltySticksFood";
}
