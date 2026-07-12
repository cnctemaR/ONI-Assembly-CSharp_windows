using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class SpiceVineConfig : IEntityConfig
{
	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		string text = "SpiceVine";
		string text2 = global::STRINGS.CREATURES.SPECIES.SPICE_VINE.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.SPICE_VINE.DESC;
		float num = 2f;
		EffectorValues tier = DECOR.BONUS.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("vinespicenut_kanim"), "idle_empty", Grid.SceneLayer.BuildingFront, 1, 3, tier, default(EffectorValues), SimHashes.Creature, new List<Tag> { GameTags.Hanging }, 320f);
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
		EntityTemplates.MakeHangingOffsets(EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Harvest, "SpiceVineSeed", global::STRINGS.CREATURES.SPECIES.SEEDS.SPICE_VINE.NAME, global::STRINGS.CREATURES.SPECIES.SEEDS.SPICE_VINE.DESC, Assets.GetAnim("seed_spicenut_kanim"), "object", 1, new List<Tag> { GameTags.CropSeed }, SingleEntityReceptacle.ReceptacleDirection.Bottom, default(Tag), 4, global::STRINGS.CREATURES.SPECIES.SPICE_VINE.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f, null, "", false, null), "SpiceVine_preview", Assets.GetAnim("vinespicenut_kanim"), "place", 1, 3), 1, 3);
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
}
