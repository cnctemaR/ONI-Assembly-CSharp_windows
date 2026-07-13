using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class WormSuperFruitConfig : IEntityConfig, IHasDlcRestrictions
{
	public string[] GetRequiredDlcIds()
	{
		return DlcManager.EXPANSION1;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public GameObject CreatePrefab()
	{
		return EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity("WormSuperFruit", global::STRINGS.ITEMS.FOOD.WORMSUPERFRUIT.NAME, global::STRINGS.ITEMS.FOOD.WORMSUPERFRUIT.DESC, 1f, false, Assets.GetAnim("wormwood_super_fruits_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.6f, 0.4f, true, 0, SimHashes.Creature, null), FOOD.FOOD_TYPES.WORMSUPERFRUIT);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "WormSuperFruit";
}
