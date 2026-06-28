using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class SteamSpoutConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		DecorValues tier = global::TUNING.BUILDINGS.DECOR.BONUS.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("SteamSpout", global::STRINGS.CREATURES.SPECIES.STEAMSPOUT.NAME, global::STRINGS.CREATURES.SPECIES.STEAMSPOUT.DESC, 400f, "elementspout", "idle", Grid.SceneLayer.BuildingFront, 1, 1, tier);
		gameObject.UpdateComponentRequirement<ElementSpout>(true).ConfigureEmissionSettings(3f, 1.5f, 3f, 5f);
		gameObject.AddElementEmitter(SimHashes.Steam, 0f, 0f);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
