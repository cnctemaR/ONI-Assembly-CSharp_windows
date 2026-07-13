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
		EffectorValues tier = DECOR.BONUS.TIER1;
		KAnimFile anim = Assets.GetAnim("vinespicenut_kanim");
		string text4 = "idle_empty";
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.BuildingFront;
		int num2 = 1;
		int num3 = 3;
		EffectorValues effectorValues = tier;
		List<Tag> list = new List<Tag> { GameTags.Hanging };
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, anim, text4, sceneLayer, num2, num3, effectorValues, default(EffectorValues), SimHashes.Creature, list, 320f);
		EntityTemplates.MakeHangingOffsets(gameObject, 1, 3);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 258.15f, 308.15f, 358.15f, 448.15f, null, true, 0f, 0.15f, SpiceNutConfig.ID, true, true, true, true, 2400f, 0f, 9800f, "SpiceVineOriginal", global::STRINGS.CREATURES.SPECIES.SPICE_VINE.NAME);
		Tag tag = ElementLoader.FindElementByHash(SimHashes.DirtyWater).tag;
		EntityTemplates.ExtendPlantToIrrigated(gameObject, new PlantElementAbsorber.ConsumeInfo[]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = tag,
				massConsumptionRate = 0.058333334f
			}
		});
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new PlantElementAbsorber.ConsumeInfo[]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = GameTags.Phosphorite,
				massConsumptionRate = 0.0016666667f
			}
		});
		gameObject.AddOrGet<DirectlyEdiblePlant_Growth>();
		gameObject.GetComponent<UprootedMonitor>().monitorCells = new CellOffset[]
		{
			new CellOffset(0, 1)
		};
		gameObject.AddOrGet<StandardCropPlant>();
		GameObject gameObject2 = gameObject;
		IHasDlcRestrictions hasDlcRestrictions = this as IHasDlcRestrictions;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Harvest;
		string text5 = "SpiceVineSeed";
		string text6 = global::STRINGS.CREATURES.SPECIES.SEEDS.SPICE_VINE.NAME;
		string text7 = global::STRINGS.CREATURES.SPECIES.SEEDS.SPICE_VINE.DESC;
		KAnimFile anim2 = Assets.GetAnim("seed_spicenut_kanim");
		string text8 = "object";
		int num4 = 1;
		List<Tag> list2 = new List<Tag>();
		list2.Add(GameTags.CropSeed);
		SingleEntityReceptacle.ReceptacleDirection receptacleDirection = SingleEntityReceptacle.ReceptacleDirection.Bottom;
		string text9 = global::STRINGS.CREATURES.SPECIES.SPICE_VINE.DOMESTICATEDDESC;
		GameObject gameObject3 = EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(gameObject2, hasDlcRestrictions, productionType, text5, text6, text7, anim2, text8, num4, list2, receptacleDirection, default(Tag), 4, text9, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f, null, "", false), "SpiceVine_preview", Assets.GetAnim("vinespicenut_kanim"), "place", 1, 3);
		gameObject.AddOrGet<PlantFiberProducer>().amount = 16f;
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

	public const float PLANT_FIBER_PRODUCED_PER_CYCLE = 16f;
}
