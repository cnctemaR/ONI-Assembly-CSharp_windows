using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class ColdBreatherConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("ColdBreather", global::STRINGS.CREATURES.SPECIES.COLDBREATHER.NAME, global::STRINGS.CREATURES.SPECIES.COLDBREATHER.DESC, 400f, Assets.GetAnim("coldbreather_kanim"), "grow_seed", Grid.SceneLayer.BuildingFront, 1, 2, DECOR.BONUS.TIER1, NOISE_POLLUTION.NOISY.TIER2, SimHashes.Creature, null, 293f);
		gameObject.UpdateComponentRequirement<ReceptacleMonitor>(true);
		gameObject.UpdateComponentRequirement<EntombVulnerable>(true);
		gameObject.UpdateComponentRequirement<WiltCondition>(true);
		gameObject.UpdateComponentRequirement<Uprootable>(true);
		gameObject.UpdateComponentRequirement<UprootedMonitor>(true);
		DrowningMonitor drowningMonitor = gameObject.UpdateComponentRequirement<DrowningMonitor>(true);
		drowningMonitor.Configure(15f, 5f, 0.95f);
		TemperatureVulnerable temperatureVulnerable = gameObject.UpdateComponentRequirement<TemperatureVulnerable>(true);
		temperatureVulnerable.Configure(213.15f, 183.15f, 368.15f, 463.15f, 0f, 0f);
		gameObject.UpdateComponentRequirement<OccupyArea>(true).objectLayer = ObjectLayer.Building;
		ColdBreather coldBreather = gameObject.UpdateComponentRequirement<ColdBreather>(true);
		coldBreather.deltaEmitTemperature = -5f;
		coldBreather.emitOffsetCell = new Vector3(0f, 1f);
		Storage storage = BuildingTemplates.CreateDefaultStorage(gameObject, false);
		storage.showInUI = false;
		ElementConsumer elementConsumer = gameObject.UpdateComponentRequirement<ElementConsumer>(true);
		elementConsumer.storeOnConsume = true;
		elementConsumer.configuration = ElementConsumer.Configuration.AllGas;
		elementConsumer.consumptionRate = 1f;
		elementConsumer.consumptionRadius = 1;
		elementConsumer.sampleCellOffset = new Vector3(0f, 0f);
		SimTemperatureTransfer component = gameObject.GetComponent<SimTemperatureTransfer>();
		component.SurfaceArea = 10f;
		component.Thickness = 0.001f;
		List<Tag> list = new List<Tag> { GameTags.DecorSeed };
		GameObject gameObject2 = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Hidden, "ColdBreatherSeed", global::STRINGS.CREATURES.SPECIES.SEEDS.COLDBREATHER.NAME, global::STRINGS.CREATURES.SPECIES.SEEDS.COLDBREATHER.DESC, Assets.GetAnim("seed_coldbreather_kanim"), "object", 1, list, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 2, global::STRINGS.CREATURES.SPECIES.COLDBREATHER.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f, null, string.Empty);
		EntityTemplates.CreateAndRegisterPreviewForPlant(gameObject2, "ColdBreather_preview", Assets.GetAnim("coldbreather_kanim"), "place", 1, 2);
		SoundEventVolumeCache.instance.AddVolume("coldbreather_kanim", "ColdBreather_grow", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("coldbreather_kanim", "ColdBreather_intake", NOISE_POLLUTION.CREATURES.TIER3);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "ColdBreather";

	public const float FERTILIZATION_RATE = 0.033333335f;

	public const SimHashes FERTILIZER = SimHashes.Phosphorite;

	public const float TEMP_DELTA = -5f;

	public const float CONSUMPTION_RATE = 1f;

	public const string SEED_ID = "ColdBreatherSeed";

	public static readonly Tag TAG = TagManager.Create("ColdBreather", null);

	public static readonly Tag SEED_TAG = TagManager.Create("ColdBreatherSeed", null);
}
