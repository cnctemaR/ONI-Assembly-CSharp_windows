using System;
using STRINGS;
using UnityEngine;

[EntityConfigOrder(2)]
public class BabyHatchVeggieConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = HatchVeggieConfig.CreateHatch("HatchVeggieBaby", CREATURES.SPECIES.HATCH.VARIANT_VEGGIE.BABY.NAME, CREATURES.SPECIES.HATCH.VARIANT_VEGGIE.BABY.DESC, "baby_hatch_kanim", true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "HatchVeggie", null, false, 5f);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "HatchVeggieBaby";
}
