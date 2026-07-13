using System;
using STRINGS;
using UnityEngine;

[EntityConfigOrder(4)]
public class BabyAlgaeStegoConfig : IEntityConfig, IHasDlcRestrictions
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
		GameObject gameObject = AlgaeStegoConfig.CreateStego("AlgaeStegoBaby", CREATURES.SPECIES.ALGAE_STEGO.BABY.NAME, CREATURES.SPECIES.ALGAE_STEGO.BABY.DESC, "baby_stego_kanim", true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "AlgaeStego", null, false, 5f);
		KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
		component.SetSymbolVisiblity("baby_stego_eye_yellow", false);
		component.SetSymbolVisiblity("baby_stego_scale", false);
		component.SetSymbolVisiblity("baby_stego_pupil", false);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "AlgaeStegoBaby";
}
