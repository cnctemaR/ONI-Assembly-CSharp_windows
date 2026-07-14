using System;
using STRINGS;
using UnityEngine;

[EntityConfigOrder(3)]
public class BabySnailIronConfig : IEntityConfig, IHasDlcRestrictions
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = SnailIronConfig.CreateSnail("SnailIronBaby", CREATURES.SPECIES.SNAIL.VARIANT_IRON.BABY.NAME, CREATURES.SPECIES.SNAIL.VARIANT_IRON.BABY.DESC, "baby_snail_iron_kanim", true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "SnailIron", null, true, 5f);
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

	public const string ID = "SnailIronBaby";
}
