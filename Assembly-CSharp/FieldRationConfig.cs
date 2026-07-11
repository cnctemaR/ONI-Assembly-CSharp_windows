using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class FieldRationConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		return EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity("FieldRation", ITEMS.FOOD.FIELDRATION.NAME, ITEMS.FOOD.FIELDRATION.DESC, 1f, false, Assets.GetAnim("fieldration_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, 0, SimHashes.Creature, null), FOOD.FOOD_TYPES.FIELDRATION);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "FieldRation";
}
