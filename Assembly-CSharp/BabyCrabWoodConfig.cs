using System;
using STRINGS;
using UnityEngine;

[EntityConfigOrder(4)]
public class BabyCrabWoodConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = CrabWoodConfig.CreateCrabWood("CrabWoodBaby", CREATURES.SPECIES.CRAB.VARIANT_WOOD.BABY.NAME, CREATURES.SPECIES.CRAB.VARIANT_WOOD.BABY.DESC, "baby_pincher_kanim", true, new string[] { "CrabWoodShell", "ShellfishMeat" }, new float[] { 100f, 1.2f });
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "CrabWood", "CrabWoodShell", false, 5f);
		gameObject.AddOrGetDef<BabyMonitor.Def>().onGrowDropUnits = 50f;
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
