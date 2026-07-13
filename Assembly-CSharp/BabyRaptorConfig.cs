using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

[EntityConfigOrder(2)]
public class BabyRaptorConfig : IEntityConfig, IHasDlcRestrictions
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
		GameObject gameObject = RaptorConfig.CreateRaptor("RaptorBaby", CREATURES.SPECIES.RAPTOR.BABY.NAME, CREATURES.SPECIES.RAPTOR.BABY.DESC, "baby_raptor_kanim", true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "Raptor", null, false, 5f).AddOrGetDef<BabyMonitor.Def>().configureAdultOnMaturation = delegate(GameObject go)
		{
			AmountInstance amountInstance = Db.Get().Amounts.ScaleGrowth.Lookup(go);
			amountInstance.value = amountInstance.GetMax() * RaptorConfig.SCALE_INITIAL_GROWTH_PCT;
		};
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "RaptorBaby";
}
