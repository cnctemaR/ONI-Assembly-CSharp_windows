using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class GinkoNutConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("GinkoNut", ITEMS.PILLS.GINKONUT.NAME, ITEMS.PILLS.GINKONUT.DESC, 1f, "pill_1_kanim", "kukumelon", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.POLYGONAL, 1f, 1f, true);
		EntityTemplates.ExtendEntityToMedicine(gameObject, MEDICINE.GINKONUT);
		EntityTemplates.SetDescriptionOrder(gameObject);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
