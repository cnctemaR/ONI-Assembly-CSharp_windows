using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class PropGravitasFirstAidKitConfig : IEntityConfig
{
	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		string text = "PropGravitasFirstAidKit";
		string text2 = global::STRINGS.BUILDINGS.PREFABS.PROPGRAVITASFIRSTAIDKIT.NAME;
		string text3 = global::STRINGS.BUILDINGS.PREFABS.PROPGRAVITASFIRSTAIDKIT.DESC;
		float num = 50f;
		EffectorValues tier = global::TUNING.BUILDINGS.DECOR.BONUS.TIER0;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("gravitas_first_aid_kit_kanim"), "off", Grid.SceneLayer.Building, 1, 1, tier, tier2, SimHashes.Creature, new List<Tag> { GameTags.Gravitas }, 293f);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Granite, true);
		component.Temperature = 294.15f;
		Workable workable = gameObject.AddOrGet<Workable>();
		workable.synchronizeAnims = false;
		workable.resetProgressOnStop = true;
		SetLocker setLocker = gameObject.AddOrGet<SetLocker>();
		setLocker.overrideAnim = "anim_interacts_clothingfactory_kanim";
		setLocker.dropOffset = new Vector2I(0, 1);
		gameObject.AddOrGet<Demolishable>();
		return gameObject;
	}

	public static string[][] GetLockerBaseContents()
	{
		string text = (DlcManager.FeatureRadiationEnabled() ? "BasicRadPill" : "IntermediateCure");
		return new string[][]
		{
			new string[] { "BasicCure", "BasicCure", "BasicCure" },
			new string[] { text, text }
		};
	}

	public void OnPrefabInit(GameObject inst)
	{
		inst.GetComponent<OccupyArea>().objectLayers = new ObjectLayer[] { ObjectLayer.Building };
		SetLocker component = inst.GetComponent<SetLocker>();
		component.possible_contents_ids = PropGravitasFirstAidKitConfig.GetLockerBaseContents();
		component.ChooseContents();
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
