using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class SpiceVineConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		DecorValues tier = DECOR.BONUS.TIER1;
		List<Tag> list = new List<Tag> { GameTags.Hanging };
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("SpiceVine", global::STRINGS.CREATURES.SPECIES.SPICE_VINE.NAME, global::STRINGS.CREATURES.SPECIES.SPICE_VINE.DESC, 400f, Assets.GetAnim("vinespicenut_kanim"), "idle_empty", Grid.SceneLayer.BuildingFront, 1, 3, tier, SimHashes.Creature, list);
		EntityTemplates.MakeHangingOffsets(gameObject, 1, 3);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 15f, 5f, 258.15f, 308.15f, 328.15f, 333.15f, 358.15f, 448.15f, 0f, 0.15f, 1f, SpiceNutConfig.ID);
		Tag tag = ElementLoader.FindElementByHash(SimHashes.DirtyWater).tag;
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new FertilizationMonitor.FertilizerInfo[]
		{
			new FertilizationMonitor.FertilizerInfo
			{
				tag = GameTags.Phosphorus,
				massConsumptionRate = 0.0016666667f
			}
		});
		EntityTemplates.ExtendPlantToIrrigated(gameObject, new FertilizationMonitor.FertilizerInfo[]
		{
			new FertilizationMonitor.FertilizerInfo
			{
				tag = tag,
				massConsumptionRate = 0.058333334f
			}
		});
		EntityTemplates.ExtendPlantWithYield(gameObject, new IYieldEffect[]
		{
			new YieldEffect.AddSeeds(1)
		}, new IYieldEffect[]
		{
			new YieldEffect.AddHarvestUnitsMultiple(1f)
		});
		UprootedMonitor component = gameObject.GetComponent<UprootedMonitor>();
		component.monitorCell = new CellOffset(0, 1);
		gameObject.UpdateComponentRequirement<StandardCropPlant>(true);
		list = new List<Tag> { GameTags.CropSeed };
		GameObject gameObject2 = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Harvest, "SpiceVineSeed", global::STRINGS.CREATURES.SPECIES.SEEDS.SPICE_VINE.NAME, global::STRINGS.CREATURES.SPECIES.SEEDS.SPICE_VINE.DESC, Assets.GetAnim("seed_spicenut_kanim"), "object", 1, list, SingleEntityReceptacle.ReceptacleDirection.Bottom, default(Tag), 4, global::STRINGS.CREATURES.SPECIES.SPICE_VINE.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f);
		GameObject gameObject3 = EntityTemplates.CreateAndRegisterPreviewForPlant(gameObject, gameObject2, "SpiceVine_preview", Assets.GetAnim("vinespicenut_kanim"), "place", 1, 3);
		EntityTemplates.MakeHangingOffsets(gameObject3, 1, 3);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "SpiceVine";

	public const string SEED_ID = "SpiceVineSeed";

	public const float FERTILIZATION_RATE = 0.0016666667f;

	public const float WATER_RATE = 0.058333334f;

	public const int MID_YIELD_SEEDS = 1;

	public const float HIGH_YIELD_MOD = 1f;
}
