using System;
using STRINGS;
using UnityEngine;

[EntityConfigOrder(2)]
public class BabyPrehistoricPacuConfig : IEntityConfig, IHasDlcRestrictions
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
		GameObject gameObject = PrehistoricPacuConfig.CreatePrehistoricPacu("PrehistoricPacuBaby", CREATURES.SPECIES.PREHISTORICPACU.BABY.NAME, CREATURES.SPECIES.PREHISTORICPACU.BABY.DESC, "baby_paculacanth_kanim", true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "PrehistoricPacu", null, false, 5f);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "PrehistoricPacuBaby";
}
