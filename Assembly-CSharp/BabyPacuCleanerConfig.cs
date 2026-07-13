using System;
using STRINGS;
using UnityEngine;

[EntityConfigOrder(2)]
public class BabyPacuCleanerConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = PacuCleanerConfig.CreatePacu("PacuCleanerBaby", CREATURES.SPECIES.PACU.VARIANT_CLEANER.BABY.NAME, CREATURES.SPECIES.PACU.VARIANT_CLEANER.BABY.DESC, "baby_pacu_kanim", true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "PacuCleaner", null, false, 5f);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "PacuCleanerBaby";
}
