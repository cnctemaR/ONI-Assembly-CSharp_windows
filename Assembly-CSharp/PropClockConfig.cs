using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class PropClockConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "PropClock";
		string text2 = global::STRINGS.BUILDINGS.PREFABS.PROPCLOCK.NAME;
		string text3 = global::STRINGS.BUILDINGS.PREFABS.PROPCLOCK.DESC;
		float num = 50f;
		EffectorValues tier = global::TUNING.BUILDINGS.DECOR.BONUS.TIER0;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER0;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("clock_poi_kanim"), "off", Grid.SceneLayer.Building, 1, 2, tier, tier2, SimHashes.Creature, null, 293f);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Unobtanium);
		component.Temperature = 294.15f;
		gameObject.AddOrGet<OccupyArea>().objectLayers = new ObjectLayer[] { ObjectLayer.Building };
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
