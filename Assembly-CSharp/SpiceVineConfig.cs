using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class SpiceVineConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "SpiceVine";
		string text2 = global::STRINGS.CREATURES.SPECIES.SPICE_VINE.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.SPICE_VINE.DESC;
		float num = 2f;
		KAnimFile kanimFile = Assets.GetAnim("vinespicenut_kanim");
		string text4 = "idle_empty";
		EffectorValues tier = DECOR.BONUS.TIER1;
		List<Tag> list = new List<Tag> { GameTags.Hanging };
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, kanimFile, text4, Grid.SceneLayer.BuildingFront, 1, 3, tier, default(EffectorValues), SimHashes.Creature, list, 320f);
		EntityTemplates.MakeHangingOffsets(gameObject, 1, 3);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 15f, 5f, 258.15f, 308.15f, 328.15f, 333.15f, 358.15f, 448.15f, null, true, 0f, 0.15f, 1f, SpiceNutConfig.ID, true);
		Tag tag = ElementLoader.FindElementByHash(SimHashes.DirtyWater).tag;
		EntityTemplates.ExtendPlantToIrrigated(gameObject, new IrrigationMonitor.LiquidResourceInfo[]
		{
			new IrrigationMonitor.LiquidResourceInfo
			{
				tag = tag,
				massConsumptionRate = 0.058333334f
			}
		});
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new FertilizationMonitor.FertilizerInfo[]
		{
			new FertilizationMonitor.FertilizerInfo
			{
				tag = GameTags.Phosphorite,
				massConsumptionRate = 0.0016666667f
			}
		});
		UprootedMonitor component = gameObject.GetComponent<UprootedMonitor>();
		component.monitorCell = new CellOffset(0, 1);
		gameObject.UpdateComponentRequirement<StandardCropPlant>(true);
		GameObject gameObject2 = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Harvest;
		text4 = "SpiceVineSeed";
		text3 = global::STRINGS.CREATURES.SPECIES.SEEDS.SPICE_VINE.NAME;
		text2 = global::STRINGS.CREATURES.SPECIES.SEEDS.SPICE_VINE.DESC;
		kanimFile = Assets.GetAnim("seed_spicenut_kanim");
		list = new List<Tag> { GameTags.CropSeed };
		GameObject gameObject3 = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject2, productionType, text4, text3, text2, kanimFile, "object", 1, list, SingleEntityReceptacle.ReceptacleDirection.Bottom, default(Tag), 4, global::STRINGS.CREATURES.SPECIES.SPICE_VINE.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f, null, "");
		GameObject gameObject4 = EntityTemplates.CreateAndRegisterPreviewForPlant(gameObject3, "SpiceVine_preview", Assets.GetAnim("vinespicenut_kanim"), "place", 1, 3);
		EntityTemplates.MakeHangingOffsets(gameObject4, 1, 3);
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
