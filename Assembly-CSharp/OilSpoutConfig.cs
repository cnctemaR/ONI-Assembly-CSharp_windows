using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class OilSpoutConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		DecorValues tier = global::TUNING.BUILDINGS.DECOR.BONUS.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("OilSpout", global::STRINGS.CREATURES.SPECIES.OILSPOUT.NAME, global::STRINGS.CREATURES.SPECIES.OILSPOUT.DESC, 400f, "elementspout", "idle", Grid.SceneLayer.BuildingFront, 1, 1, tier);
		gameObject.UpdateComponentRequirement<ElementSpout>(true).ConfigureEmissionSettings(10f, 5f, 2000f, 20f);
		gameObject.AddElementEmitter(SimHashes.CrudeOil, 0f, 0f);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
