using System;
using STRINGS;
using UnityEngine;

[EntityConfigOrder(2)]
public class BabyPuftConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = PuftConfig.CreatePuft("PuftBaby", CREATURES.SPECIES.PUFT.BABY.NAME, CREATURES.SPECIES.PUFT.BABY.DESC, "baby_puft_kanim", true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "Puft", null, false, 5f);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "PuftBaby";
}
