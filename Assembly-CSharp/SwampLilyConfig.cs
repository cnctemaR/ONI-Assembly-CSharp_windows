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
		KAnimFile kanimFile = Assets.GetAnim("swamplily_kanim");
		string text4 = "idle_empty";
		EffectorValues tier = DECOR.BONUS.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, kanimFile, text4, Grid.SceneLayer.BuildingBack, 1, 2, tier, default(EffectorValues), SimHashes.Creature, null, 328.15f);
		GameObject gameObject2 = gameObject;
		num = 258.15f;
		float num2 = 308.15f;
		float num3 = 358.15f;
		float num4 = 448.15f;
		SimHashes[] array = new SimHashes[] { SimHashes.ChlorineGas };
		text4 = SwampLilyFlowerConfig.ID;
		EntityTemplates.ExtendEntityToBasicPlant(gameObject2, num, num2, num3, num4, array, true, 0f, 0.15f, text4, true, true, true, true, 2400f);
		gameObject.AddOrGet<StandardCropPlant>();
		gameObject2 = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Harvest;
		text4 = "SwampLilySeed";
		text3 = global::STRINGS.CREATURES.SPECIES.SEEDS.SWAMPLILY.NAME;
		text2 = global::STRINGS.CREATURES.SPECIES.SEEDS.SWAMPLILY.DESC;
		kanimFile = Assets.GetAnim("seed_swampLily_kanim");
		List<Tag> list = new List<Tag> { GameTags.CropSeed };
		GameObject gameObject3 = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject2, productionType, text4, text3, text2, kanimFile, "object", 1, list, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 4, global::STRINGS.CREATURES.SPECIES.SWAMPLILY.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f, null, string.Empty, false);
		EntityTemplates.CreateAndRegisterPreviewForPlant(gameObject3, SwampLilyConfig.ID + "_preview", Assets.GetAnim("swamplily_kanim"), "place", 1, 2);
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
