using System;
using STRINGS;
using UnityEngine;

public class BabyStaterpillarLiquidConfig : IEntityConfig
{
	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = StaterpillarLiquidConfig.CreateStaterpillarLiquid("StaterpillarLiquidBaby", CREATURES.SPECIES.STATERPILLAR.VARIANT_LIQUID.BABY.NAME, CREATURES.SPECIES.STATERPILLAR.VARIANT_LIQUID.BABY.DESC, "baby_caterpillar_kanim", true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "StaterpillarLiquid", null, false, 5f);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
		prefab.GetComponent<KBatchedAnimController>().SetSymbolVisiblity("electric_bolt_c_bloom", false);
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "StaterpillarLiquidBaby";
}
