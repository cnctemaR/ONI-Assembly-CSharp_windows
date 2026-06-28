using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class BasicForagePlantPlantedConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		DecorValues tier = DECOR.BONUS.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("BasicForagePlantPlanted", global::STRINGS.CREATURES.SPECIES.BASICFORAGEPLANTPLANTED.NAME, global::STRINGS.CREATURES.SPECIES.BASICFORAGEPLANTPLANTED.DESC, 100f, Assets.GetAnim("muckroot_kanim"), "idle", Grid.SceneLayer.BuildingBack, 1, 1, tier, SimHashes.Creature, null);
		gameObject.UpdateComponentRequirement<SimTemperatureTransfer>(true);
		gameObject.UpdateComponentRequirement<PreserveOnEntomb>(true);
		OccupyArea occupyArea = gameObject.UpdateComponentRequirement<OccupyArea>(true);
		occupyArea.objectLayer = ObjectLayer.Building;
		gameObject.UpdateComponentRequirement<EntombVulnerable>(true);
		DrowningMonitor drowningMonitor = gameObject.UpdateComponentRequirement<DrowningMonitor>(true);
		drowningMonitor.Configure(15f, 5f, 0.95f);
		gameObject.UpdateComponentRequirement<Uprootable>(true);
		gameObject.UpdateComponentRequirement<UprootedMonitor>(true);
		gameObject.UpdateComponentRequirement<Harvestable>(true);
		SeedProducer seedProducer = gameObject.UpdateComponentRequirement<SeedProducer>(true);
		seedProducer.Configure("BasicForagePlant", SeedProducer.ProductionType.DigOnly, 1);
		gameObject.UpdateComponentRequirement<BasicForagePlantPlanted>(true);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "BasicForagePlantPlanted";
}
