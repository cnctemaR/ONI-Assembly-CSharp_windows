using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class GravitasCeilingLightConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "GravitasCeilingLight";
		string text2 = global::STRINGS.BUILDINGS.PREFABS.PROPGRAVITASCEILINGLIGHT.NAME;
		string text3 = global::STRINGS.BUILDINGS.PREFABS.PROPGRAVITASCEILINGLIGHT.DESC;
		float num = 25f;
		EffectorValues tier = global::TUNING.BUILDINGS.DECOR.BONUS.TIER0;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("gravitas_light2_kanim"), "off", Grid.SceneLayer.Building, 1, 1, tier, PermittedRotations.Unrotatable, Orientation.Neutral, tier2, SimHashes.Creature, new List<Tag> { GameTags.Gravitas }, 293f);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.IronOre, true);
		component.Temperature = 294.15f;
		LoreBearerUtil.AddLoreTo(gameObject, new LoreBearerAction(LoreBearerUtil.UnlockNextEmail));
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
