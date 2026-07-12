using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class CarrotPlantConfig : IEntityConfig
{
	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_DLC_2;
	}

	public GameObject CreatePrefab()
	{
		string text = "CarrotPlant";
		string text2 = global::STRINGS.CREATURES.SPECIES.CARROTPLANT.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.CARROTPLANT.DESC;
		float num = 1f;
		EffectorValues tier = DECOR.PENALTY.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("purpleroot_kanim"), "idle_empty", Grid.SceneLayer.BuildingBack, 1, 2, tier, default(EffectorValues), SimHashes.Creature, null, 255f);
		GameObject gameObject2 = gameObject;
		float num2 = 118.149994f;
		float num3 = 218.15f;
		float num4 = 259.15f;
		float num5 = 269.15f;
		string id = CarrotConfig.ID;
		EntityTemplates.ExtendEntityToBasicPlant(gameObject2, num2, num3, num4, num5, new SimHashes[]
		{
			SimHashes.Oxygen,
			SimHashes.ContaminatedOxygen,
			SimHashes.CarbonDioxide
		}, true, 0f, 0.15f, id, true, true, true, true, 2400f, 0f, 4600f, "CarrotPlantOriginal", global::STRINGS.CREATURES.SPECIES.CARROTPLANT.NAME);
		gameObject.AddOrGet<StandardCropPlant>();
		gameObject.AddOrGet<DirectlyEdiblePlant_Growth>();
		gameObject.AddOrGet<LoopingSounds>();
		GameObject gameObject3 = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Harvest, "CarrotPlantSeed", global::STRINGS.CREATURES.SPECIES.SEEDS.CARROTPLANT.NAME, global::STRINGS.CREATURES.SPECIES.SEEDS.CARROTPLANT.DESC, Assets.GetAnim("seed_purpleroot_kanim"), "object", 1, new List<Tag> { GameTags.CropSeed }, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 1, global::STRINGS.CREATURES.SPECIES.CARROTPLANT.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f, null, "", false, this.GetDlcIds());
		EntityTemplates.ExtendPlantToIrrigated(gameObject, new PlantElementAbsorber.ConsumeInfo[]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = SimHashes.Ethanol.CreateTag(),
				massConsumptionRate = 0.025f
			}
		});
		EntityTemplates.CreateAndRegisterPreviewForPlant(gameObject3, "CarrotPlant_preview", Assets.GetAnim("purpleroot_kanim"), "place", 1, 2);
		SoundEventVolumeCache.instance.AddVolume("bristleblossom_kanim", "PrickleFlower_harvest", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("bristleblossom_kanim", "PrickleFlower_grow", NOISE_POLLUTION.CREATURES.TIER3);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "CarrotPlant";

	public const string SEED_ID = "CarrotPlantSeed";

	public const float Temperature_lethal_low = 118.149994f;

	public const float Temperature_warning_low = 218.15f;

	public const float Temperature_lethal_high = 269.15f;

	public const float Temperature_warning_high = 259.15f;

	public const float FERTILIZATION_RATE = 0.025f;
}
