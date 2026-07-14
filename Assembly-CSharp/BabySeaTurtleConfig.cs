using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

[EntityConfigOrder(3)]
public class BabySeaTurtleConfig : IEntityConfig, IHasDlcRestrictions
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
		GameObject gameObject = SeaTurtleConfig.CreateSeaTurtle("SeaTurtleBaby", CREATURES.SPECIES.SEATURTLE.BABY.NAME, CREATURES.SPECIES.SEATURTLE.BABY.DESC, "baby_turtle_kanim", true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "SeaTurtle", null, false, 5f).AddOrGetDef<BabyMonitor.Def>().configureAdultOnMaturation = delegate(GameObject go)
		{
			AmountInstance amountInstance = Db.Get().Amounts.ScaleGrowth.Lookup(go);
			amountInstance.value = amountInstance.GetMax() * SeaTurtleTuning.SCALE_INITIAL_GROWTH_PCT;
		};
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "SeaTurtleBaby";
}
