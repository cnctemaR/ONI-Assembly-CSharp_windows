using System;
using STRINGS;
using UnityEngine;

[EntityConfigOrder(2)]
public class BabyStaterpillarGasConfig : IEntityConfig, IHasDlcRestrictions
{
	public string[] GetRequiredDlcIds()
	{
		return DlcManager.EXPANSION1;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = StaterpillarGasConfig.CreateStaterpillarGas("StaterpillarGasBaby", CREATURES.SPECIES.STATERPILLAR.VARIANT_GAS.BABY.NAME, CREATURES.SPECIES.STATERPILLAR.VARIANT_GAS.BABY.DESC, "baby_caterpillar_kanim", true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "StaterpillarGas", null, false, 5f);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
		prefab.GetComponent<KBatchedAnimController>().SetSymbolVisiblity("electric_bolt_c_bloom", false);
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "StaterpillarGasBaby";
}
