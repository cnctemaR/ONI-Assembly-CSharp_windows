using System;
using STRINGS;
using UnityEngine;

[EntityConfigOrder(3)]
public class BabySeaHorseConfig : IEntityConfig, IHasDlcRestrictions
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = SeaHorseConfig.CreateSeaHorse("SeaHorseBaby", CREATURES.SPECIES.SEAHORSE.BABY.NAME, CREATURES.SPECIES.SEAHORSE.BABY.DESC, "baby_seahorse_kanim", true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "SeaHorse", null, false, 5f);
		return gameObject;
	}

	public string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC5;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "SeaHorseBaby";
}
