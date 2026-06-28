using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class BasicForagePlantPlantedConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		DecorValues tier = DECOR.BONUS.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("BasicForagePlantPlanted", global::STRINGS.CREATURES.SPECIES.BASICFORAGEPLANTPLANTED.NAME, global::STRINGS.CREATURES.SPECIES.BASICFORAGEPLANTPLANTED.DESC, 100f, "muckroot", "idle_loop", Grid.SceneLayer.BuildingBack, 1, 1, tier);
		gameObject.UpdateComponentRequirement<SimTemperatureTransfer>(true);
		OccupyArea occupyArea = gameObject.UpdateComponentRequirement<OccupyArea>(true);
		occupyArea.objectLayer = ObjectLayer.Building;
		EntombVulnerable entombVulnerable = gameObject.UpdateComponentRequirement<EntombVulnerable>(true);
		entombVulnerable.Configure(false);
		DrowningMonitor drowningMonitor = gameObject.UpdateComponentRequirement<DrowningMonitor>(true);
		drowningMonitor.Configure(15f, 5f, 0.95f);
		gameObject.UpdateComponentRequirement<Uprootable>(true);
		gameObject.UpdateComponentRequirement<UprootedMonitor>(true);
		gameObject.UpdateComponentRequirement<Harvestable>(true);
		SeedProducer seedProducer = gameObject.UpdateComponentRequirement<SeedProducer>(true);
		seedProducer.SeedID = "BasicForagePlant";
		gameObject.UpdateComponentRequirement<BasicForagePlantPlanted>(true);
		EntityTemplates.SetDescriptionOrder(gameObject);
		gameObject.UpdateComponentRequirement<BasicForagePlantPlanted>(true);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
