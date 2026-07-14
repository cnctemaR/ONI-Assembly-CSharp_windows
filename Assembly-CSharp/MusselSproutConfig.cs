using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class MusselSproutConfig : IEntityConfig, IHasDlcRestrictions
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(MusselSproutConfig.ID, global::STRINGS.CREATURES.SPECIES.MUSSELSPROUT.NAME, global::STRINGS.CREATURES.SPECIES.MUSSELSPROUT.DESC, 50f, Assets.GetAnim("mussel_sprout_kanim"), "idle", Grid.SceneLayer.BuildingBack, 1, 1, DECOR.PENALTY.TIER0, default(EffectorValues), SimHashes.Creature, null, 293f);
		gameObject.AddOrGet<SimTemperatureTransfer>();
		gameObject.AddOrGet<OccupyArea>().objectLayers = new ObjectLayer[] { ObjectLayer.Building };
		gameObject.AddOrGet<EntombVulnerable>();
		gameObject.AddOrGet<Prioritizable>();
		gameObject.AddOrGet<DelayedUprootable>().deathAnimation = "harvest";
		gameObject.AddOrGet<UprootedMonitor>();
		gameObject.AddOrGet<Harvestable>();
		gameObject.AddOrGet<HarvestDesignatable>();
		gameObject.AddOrGet<SeedProducer>().Configure(MusselTongueConfig.ID, SeedProducer.ProductionType.DigOnly, 1);
		gameObject.AddOrGet<BasicForagePlantPlanted>().Pre_Death_Anim = "harvest";
		gameObject.AddOrGet<KBatchedAnimController>().randomiseLoopedOffset = true;
		return gameObject;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC5;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		inst.GetComponent<KBatchedAnimController>().animScale *= 0.75f;
	}

	public static string ID = "MusselSprout";

	public static string BASE_TRAIT_ID = MusselSproutConfig.ID + "_BaseTrait";

	private const string HARVEST_ANIM = "harvest";
}
