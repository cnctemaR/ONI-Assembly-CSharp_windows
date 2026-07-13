using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class GravitasBathroomMirrorConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "GravitasBathroomMirror";
		string text2 = global::STRINGS.BUILDINGS.PREFABS.PROPGRAVITASBATHROOMMIRROR.NAME;
		string text3 = global::STRINGS.BUILDINGS.PREFABS.PROPGRAVITASBATHROOMMIRROR.DESC;
		float num = 50f;
		EffectorValues tier = global::TUNING.BUILDINGS.DECOR.BONUS.TIER0;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("gravitas_bathroom_mirror_kanim"), "on", Grid.SceneLayer.Building, 1, 1, tier, tier2, SimHashes.Creature, new List<Tag> { GameTags.Gravitas }, 293f);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Glass, true);
		component.Temperature = 294.15f;
		LoreBearerUtil.AddLoreTo(gameObject, LoreBearerUtil.UnlockSpecificEntryThenNext("story_trait_hijackheadquarters_mirror", UI.USERMENUACTIONS.READLORE.SEARCH_FLATOBJECT_SUCCESS.SEARCH1, new Action<InfoDialogScreen>(LoreBearerUtil.UnlockNextJournalEntry), true));
		gameObject.AddOrGet<Demolishable>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
		inst.GetComponent<OccupyArea>().objectLayers = new ObjectLayer[] { ObjectLayer.Building };
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "GravitasBathroomMirror";
}
