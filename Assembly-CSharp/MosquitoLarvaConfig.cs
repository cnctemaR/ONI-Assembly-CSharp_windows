using System;
using STRINGS;
using UnityEngine;

[EntityConfigOrder(2)]
public class MosquitoLarvaConfig : IEntityConfig, IHasDlcRestrictions
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
		GameObject gameObject = MosquitoConfig.CreateMosquito("MosquitoBaby", CREATURES.SPECIES.MOSQUITO.BABY.NAME, CREATURES.SPECIES.MOSQUITO.BABY.DESC, "baby_mosquito_kanim", true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "Mosquito", null, false, 5f);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "MosquitoBaby";
}
