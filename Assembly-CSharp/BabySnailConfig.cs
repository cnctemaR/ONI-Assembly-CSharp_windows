using System;
using STRINGS;
using UnityEngine;

[EntityConfigOrder(3)]
public class BabySnailConfig : IEntityConfig, IHasDlcRestrictions
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = SnailConfig.CreateSnail("SnailBaby", CREATURES.SPECIES.SNAIL.BABY.NAME, CREATURES.SPECIES.SNAIL.BABY.DESC, "baby_snail_kanim", true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "Snail", null, true, 5f);
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

	public const string ID = "SnailBaby";
}
