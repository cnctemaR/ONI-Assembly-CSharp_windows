using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class MuscarinicAntagonistConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("MuscarinicAntagonist", ITEMS.PILLS.MUSCARINICANTAGONIST.NAME, ITEMS.PILLS.MUSCARINICANTAGONIST.DESC, 1f, "pill_1_kanim", "pyrus", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.POLYGONAL, 1f, 1f, true);
		EntityTemplates.ExtendEntityToMedicine(gameObject, MEDICINE.MUSCARINICANTAGONIST);
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
