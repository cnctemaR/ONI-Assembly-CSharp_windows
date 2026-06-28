using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class GinkoNutConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("GinkoNut", ITEMS.PILLS.GINKONUT.NAME, ITEMS.PILLS.GINKONUT.DESC, 1f, true, Assets.GetAnim("pill_1_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.POLYGONAL, 1f, 1f, true, SimHashes.Creature, null);
		EntityTemplates.ExtendEntityToMedicine(gameObject, MEDICINE.GINKONUT);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
