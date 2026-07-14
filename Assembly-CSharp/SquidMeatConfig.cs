using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class SquidMeatConfig : IEntityConfig, IHasDlcRestrictions
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
		GameObject gameObject = EntityTemplates.CreateLooseEntity("SquidMeat", global::STRINGS.ITEMS.FOOD.SQUIDMEAT.NAME, global::STRINGS.ITEMS.FOOD.SQUIDMEAT.DESC, 1f, false, Assets.GetAnim("squid_meat_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.9f, 0.8f, true, 0, SimHashes.Creature, null);
		gameObject.GetComponent<KBoxCollider2D>().offset = new Vector2(0f, 0.1f);
		EntityTemplates.ExtendEntityToFood(gameObject, FOOD.FOOD_TYPES.SQUID_MEAT);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "SquidMeat";
}
