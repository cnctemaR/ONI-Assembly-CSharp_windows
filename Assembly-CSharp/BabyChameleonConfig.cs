using System;
using STRINGS;
using UnityEngine;

[EntityConfigOrder(2)]
public class BabyChameleonConfig : IEntityConfig, IHasDlcRestrictions
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
		GameObject gameObject = ChameleonConfig.CreateChameleon("ChameleonBaby", CREATURES.SPECIES.CHAMELEON.BABY.NAME, CREATURES.SPECIES.CHAMELEON.BABY.DESC, "baby_chameleo_kanim", true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "Chameleon", null, false, 5f);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "ChameleonBaby";
}
