using System;
using STRINGS;
using UnityEngine;

[EntityConfigOrder(2)]
public class BabyCrabConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = CrabConfig.CreateCrab("CrabBaby", CREATURES.SPECIES.CRAB.BABY.NAME, CREATURES.SPECIES.CRAB.BABY.DESC, "baby_pincher_kanim", true, "CrabShell", 0.5f);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "Crab", "CrabShell", false, 5f);
		gameObject.AddOrGetDef<BabyMonitor.Def>().onGrowDropUnits = 0.5f;
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "CrabBaby";
}
