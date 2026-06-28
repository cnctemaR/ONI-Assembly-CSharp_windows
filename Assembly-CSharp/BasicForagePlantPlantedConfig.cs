using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class BasicForagePlantPlantedConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "BasicForagePlantPlanted";
		string text2 = global::STRINGS.CREATURES.SPECIES.BASICFORAGEPLANTPLANTED.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.BASICFORAGEPLANTPLANTED.DESC;
		float num = 100f;
		KAnimFile anim = Assets.GetAnim("muckroot_kanim");
		string text4 = "idle";
		EffectorValues tier = DECOR.BONUS.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, anim, text4, Grid.SceneLayer.BuildingBack, 1, 1, tier, default(EffectorValues), SimHashes.Creature, null, 293f);
		gameObject.UpdateComponentRequirement<SimTemperatureTransfer>(true);
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
