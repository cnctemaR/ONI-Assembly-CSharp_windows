using System;
using STRINGS;
using UnityEngine;

[EntityConfigOrder(3)]
public class BabyCrabConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = CrabConfig.CreateCrab("CrabBaby", CREATURES.SPECIES.CRAB.BABY.NAME, CREATURES.SPECIES.CRAB.BABY.DESC, "baby_pincher_kanim", true, new string[] { "CrabShell", "ShellfishMeat" }, new float[] { 30f, 1.2f });
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "Crab", "CrabShell", false, 5f);
		gameObject.AddOrGetDef<BabyMonitor.Def>().onGrowDropUnits = 30f;
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
