using System;
using STRINGS;
using UnityEngine;

[EntityConfigOrder(2)]
public class BabyCrabWoodConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = CrabWoodConfig.CreateCrabWood("CrabWoodBaby", CREATURES.SPECIES.CRAB.VARIANT_WOOD.BABY.NAME, CREATURES.SPECIES.CRAB.VARIANT_WOOD.BABY.DESC, "baby_pincher_kanim", true, "CrabWoodShell", 0.5f);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "CrabWood", "CrabWoodShell", false, 5f);
		gameObject.AddOrGetDef<BabyMonitor.Def>().onGrowDropUnits = 0.5f;
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "CrabWoodBaby";
}
