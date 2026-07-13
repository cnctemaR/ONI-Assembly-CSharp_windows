using System;
using STRINGS;
using UnityEngine;

[EntityConfigOrder(2)]
public class BabyStegoConfig : IEntityConfig, IHasDlcRestrictions
{
	public string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC4;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = StegoConfig.CreateStego("StegoBaby", CREATURES.SPECIES.STEGO.BABY.NAME, CREATURES.SPECIES.STEGO.BABY.DESC, "baby_stego_kanim", true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "Stego", null, false, 5f);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "StegoBaby";
}
