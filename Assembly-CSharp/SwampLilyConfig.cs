using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class SwampLilyConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		EffectorValues tier = DECOR.BONUS.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("SwampLily", global::STRINGS.CREATURES.SPECIES.SWAMPLILY.NAME, global::STRINGS.CREATURES.SPECIES.SWAMPLILY.DESC, 1f, Assets.GetAnim("swamplily_kanim"), "idle_empty", Grid.SceneLayer.BuildingBack, 1, 2, tier, default(EffectorValues), SimHashes.Creature, null, 328.15f);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 15f, 5f, 258.15f, 308.15f, 328.15f, 333.15f, 358.15f, 448.15f, new SimHashes[] { SimHashes.ChlorineGas }, true, 0f, 0.15f, 1f, SwampLilyFlowerConfig.ID, true);
		gameObject.UpdateComponentRequirement<StandardCropPlant>(true);
		List<Tag> list = new List<Tag> { GameTags.CropSeed };
		GameObject gameObject2 = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Harvest, "SwampLilySeed", global::STRINGS.CREATURES.SPECIES.SEEDS.SWAMPLILY.NAME, global::STRINGS.CREATURES.SPECIES.SEEDS.SWAMPLILY.DESC, Assets.GetAnim("seed_swampLily_kanim"), "object", 1, list, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 4, global::STRINGS.CREATURES.SPECIES.SWAMPLILY.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f, null, string.Empty);
		EntityTemplates.CreateAndRegisterPreviewForPlant(gameObject2, SwampLilyConfig.ID + "_preview", Assets.GetAnim("swamplily_kanim"), "place", 1, 2);
		SoundEventVolumeCache.instance.AddVolume("swamplily_kanim", "SwampLily_grow", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("swamplily_kanim", "SwampLily_harvest", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("swamplily_kanim", "SwampLily_death", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("swamplily_kanim", "SwampLily_death_bloom", NOISE_POLLUTION.CREATURES.TIER3);
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.HarvestableIDs, SwampLilyConfig.ID);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string SEED_ID = "SwampLilySeed";

	public static string ID = "SwampLily";
}
