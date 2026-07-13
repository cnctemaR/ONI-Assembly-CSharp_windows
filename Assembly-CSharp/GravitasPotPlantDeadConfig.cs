using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class GravitasPotPlantDeadConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "GravitasPotPlantDead";
		string text2 = global::STRINGS.BUILDINGS.PREFABS.PROPGRAVITASPOTPLANTDEAD.NAME;
		string text3 = global::STRINGS.BUILDINGS.PREFABS.PROPGRAVITASPOTPLANTDEAD.DESC;
		float num = 25f;
		EffectorValues tier = global::TUNING.BUILDINGS.DECOR.BONUS.TIER0;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("gravitas_tall_plant_dead_kanim"), "off", Grid.SceneLayer.Building, 1, 2, tier, PermittedRotations.Unrotatable, Orientation.Neutral, tier2, SimHashes.Creature, new List<Tag> { GameTags.Gravitas }, 293f);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Ceramic, true);
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
}
