using System;
using STRINGS;
using UnityEngine;

[EntityConfigOrder(2)]
public class BabyBeeConfig : IEntityConfig, IHasDlcRestrictions
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
		GameObject gameObject = BeeConfig.CreateBee("BeeBaby", CREATURES.SPECIES.BEE.BABY.NAME, CREATURES.SPECIES.BEE.BABY.DESC, "baby_blarva_kanim", true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "Bee", null, true, 2f);
		gameObject.GetComponent<KPrefabID>().AddTag(GameTags.Creatures.Walker, false);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		BaseBeeConfig.SetupLoopingSounds(inst);
	}

	public const string ID = "BeeBaby";
}
