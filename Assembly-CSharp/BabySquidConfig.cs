using System;
using STRINGS;
using UnityEngine;

[EntityConfigOrder(3)]
public class BabySquidConfig : IEntityConfig, IHasDlcRestrictions
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
		GameObject gameObject = SquidConfig.CreateSquid("SquidBaby", CREATURES.SPECIES.SQUID.BABY.NAME, CREATURES.SPECIES.SQUID.BABY.DESC, "baby_squid_kanim", true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "Squid", null, false, 5f);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "SquidBaby";
}
