using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

[EntityConfigOrder(3)]
public class BabyDreckoConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = DreckoConfig.CreateDrecko("DreckoBaby", CREATURES.SPECIES.DRECKO.BABY.NAME, CREATURES.SPECIES.DRECKO.BABY.DESC, "baby_drecko_kanim", true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "Drecko", null, false, 5f).AddOrGetDef<BabyMonitor.Def>().configureAdultOnMaturation = delegate(GameObject go)
		{
			AmountInstance amountInstance = Db.Get().Amounts.ScaleGrowth.Lookup(go);
			amountInstance.value = amountInstance.GetMax() * DreckoConfig.SCALE_INITIAL_GROWTH_PCT;
		};
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "DreckoBaby";
}
