using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class GravitasClockSquareConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "GravitasClockSquare";
		string text2 = global::STRINGS.BUILDINGS.PREFABS.PROPGRAVITASCLOCKSQUARE.NAME;
		string text3 = global::STRINGS.BUILDINGS.PREFABS.PROPGRAVITASCLOCKSQUARE.DESC;
		float num = 50f;
		EffectorValues tier = global::TUNING.BUILDINGS.DECOR.BONUS.TIER0;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("gravitas_clock_square_kanim"), "off", Grid.SceneLayer.Building, 1, 1, tier, PermittedRotations.Unrotatable, Orientation.Neutral, tier2, SimHashes.Creature, new List<Tag> { GameTags.Gravitas }, 293f);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Glass, true);
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
