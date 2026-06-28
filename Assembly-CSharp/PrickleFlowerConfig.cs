using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class PrickleFlowerConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		DecorValues tier = DECOR.BONUS.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("PrickleFlower", global::STRINGS.CREATURES.SPECIES.PRICKLEFLOWER.NAME, global::STRINGS.CREATURES.SPECIES.PRICKLEFLOWER.DESC, 400f, "bristleblossom", "idle_empty", Grid.SceneLayer.BuildingFront, 1, 2, tier);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, true, 15f, 5f, 280.65f, 285.65f, 295.65f, 300.65f, 0f, 0.15f, 1f, PrickleFruitConfig.ID);
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new ElementConverter.ConsumedElement[]
		{
			new ElementConverter.ConsumedElement(GameTags.Fertilizer, PrickleFlowerConfig.FERTILIZATION_RATE)
		});
		gameObject.AddOrGet<PrickleFlower>();
		EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, PrickleFlowerConfig.SEED_ID, global::STRINGS.CREATURES.SPECIES.SEEDS.PRICKLEFLOWER.NAME, global::STRINGS.CREATURES.SPECIES.SEEDS.PRICKLEFLOWER.DESC, "seed_bristleblossom", new List<Tag> { GameTags.CropSeed }, default(Tag), 2, global::STRINGS.CREATURES.SPECIES.PRICKLEFLOWER.DOMESTICATEDDESC);
		EntityTemplates.SetDescriptionOrder(gameObject);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public static float FERTILIZATION_RATE = 0.033333335f;

	public static string SEED_ID = "PrickleFlowerSeed";
}
