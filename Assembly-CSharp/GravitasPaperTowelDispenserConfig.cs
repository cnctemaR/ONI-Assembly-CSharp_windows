using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class GravitasPaperTowelDispenserConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "GravitasPaperTowelDispenser";
		string text2 = global::STRINGS.BUILDINGS.PREFABS.PROPGRAVITASPAPERTOLELDISPENSER.NAME;
		string text3 = global::STRINGS.BUILDINGS.PREFABS.PROPGRAVITASPAPERTOLELDISPENSER.DESC;
		float num = 50f;
		EffectorValues tier = global::TUNING.BUILDINGS.DECOR.BONUS.TIER0;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("gravitas_paper_towell_dispenser_kanim"), "on", Grid.SceneLayer.Building, 1, 1, tier, tier2, SimHashes.Creature, new List<Tag> { GameTags.Gravitas }, 293f);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Granite, true);
		component.Temperature = 294.15f;
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

	public const string ID = "GravitasPaperTowelDispenser";
}
