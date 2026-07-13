using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class GravitasFridgeConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "GravitasFridge";
		string text2 = global::STRINGS.BUILDINGS.PREFABS.PROPGRAVITASFRIDGE.NAME;
		string text3 = global::STRINGS.BUILDINGS.PREFABS.PROPGRAVITASFRIDGE.DESC;
		float num = 50f;
		EffectorValues tier = global::TUNING.BUILDINGS.DECOR.BONUS.TIER0;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("gravitas_fridge_kanim"), "off", Grid.SceneLayer.Building, 2, 2, tier, PermittedRotations.Unrotatable, Orientation.Neutral, tier2, SimHashes.Creature, new List<Tag> { GameTags.Gravitas }, 293f);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Steel, true);
		component.Temperature = 294.15f;
		LoreBearerUtil.AddLoreTo(gameObject, LoreBearerUtil.UnlockSpecificEntryThenNext("story_trait_hijackheadquarters_initial", UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_SUCCESS.SEARCH3, new Action<InfoDialogScreen>(LoreBearerUtil.UnlockNextResearchNote), true));
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
}
