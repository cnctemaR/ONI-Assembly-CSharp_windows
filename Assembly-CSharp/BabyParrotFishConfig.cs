using System;
using STRINGS;
using UnityEngine;

[EntityConfigOrder(3)]
public class BabyParrotFishConfig : IEntityConfig, IHasDlcRestrictions
{
	public string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC5;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = ParrotFishConfig.CreateParrotFish("ParrotFishBaby", CREATURES.SPECIES.PARROTFISH.BABY.NAME, CREATURES.SPECIES.PARROTFISH.BABY.DESC, "baby_bioluminescent_kanim", true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "ParrotFish", null, false, 5f);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "ParrotFishBaby";
}
