using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class ProExoSetLockerConfig : IEntityConfig
{
	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		string text = "PropExoSetLocker";
		string text2 = global::STRINGS.BUILDINGS.PREFABS.PROPEXOSETLOCKER.NAME;
		string text3 = global::STRINGS.BUILDINGS.PREFABS.PROPEXOSETLOCKER.DESC;
		float num = 100f;
		EffectorValues tier = global::TUNING.BUILDINGS.DECOR.BONUS.TIER0;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("zipper_locker_kanim"), "on", Grid.SceneLayer.Building, 1, 2, tier, tier2, SimHashes.Creature, new List<Tag> { GameTags.Gravitas }, 293f);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Unobtanium, true);
		component.Temperature = 294.15f;
		Workable workable = gameObject.AddOrGet<Workable>();
		workable.synchronizeAnims = false;
		workable.resetProgressOnStop = true;
		SetLocker setLocker = gameObject.AddOrGet<SetLocker>();
		setLocker.overrideAnim = "anim_interacts_zipper_locker_kanim";
		setLocker.dropOffset = new Vector2I(0, 1);
		setLocker.numDataBanks = new int[] { 1, 4 };
		LoreBearerUtil.AddLoreTo(gameObject);
		gameObject.AddOrGet<OccupyArea>().objectLayers = new ObjectLayer[] { ObjectLayer.Building };
		gameObject.AddOrGet<Demolishable>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
		SetLocker component = inst.GetComponent<SetLocker>();
		component.possible_contents_ids = new string[][]
		{
			new string[] { "Warm_Vest" },
			new string[] { "Funky_Vest" }
		};
		component.ChooseContents();
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
