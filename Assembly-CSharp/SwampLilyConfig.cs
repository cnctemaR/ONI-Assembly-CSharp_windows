using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class SwampLilyConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "SwampLily";
		string text2 = global::STRINGS.CREATURES.SPECIES.SWAMPLILY.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.SWAMPLILY.DESC;
		float num = 1f;
		EffectorValues tier = DECOR.BONUS.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("swamplily_kanim"), "idle_empty", Grid.SceneLayer.BuildingBack, 1, 2, tier, default(EffectorValues), SimHashes.Creature, null, 328.15f);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 258.15f, 308.15f, 358.15f, 448.15f, new SimHashes[] { SimHashes.ChlorineGas }, true, 0f, 0.15f, SwampLilyFlowerConfig.ID, true, true, true, true, 2400f);
		gameObject.AddOrGet<StandardCropPlant>();
		EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Harvest, "SwampLilySeed", global::STRINGS.CREATURES.SPECIES.SEEDS.SWAMPLILY.NAME, global::STRINGS.CREATURES.SPECIES.SEEDS.SWAMPLILY.DESC, Assets.GetAnim("seed_swampLily_kanim"), "object", 1, new List<Tag> { GameTags.CropSeed }, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 4, global::STRINGS.CREATURES.SPECIES.SWAMPLILY.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f, null, "", false), SwampLilyConfig.ID + "_preview", Assets.GetAnim("swamplily_kanim"), "place", 1, 2);
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

	public static string ID = "SwampLily";

	public const string SEED_ID = "SwampLilySeed";
}
