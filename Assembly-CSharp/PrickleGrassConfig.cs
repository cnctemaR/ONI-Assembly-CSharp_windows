using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class PrickleGrassConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		DecorValues tier = DECOR.BONUS.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("PrickleGrass", global::STRINGS.CREATURES.SPECIES.PRICKLEGRASS.NAME, global::STRINGS.CREATURES.SPECIES.PRICKLEGRASS.DESC, 400f, "bristlebriar", "idle_loop", Grid.SceneLayer.BuildingFront, 1, 1, tier);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, true, 15f, 5f, 273f, 283f, 303f, 315f, 0f, 0.15f, 1f, null);
		gameObject.AddOrGet<PrickleGrass>();
		EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, "PrickleGrassSeed", global::STRINGS.CREATURES.SPECIES.SEEDS.PRICKLEGRASS.NAME, global::STRINGS.CREATURES.SPECIES.SEEDS.PRICKLEGRASS.DESC, "seed_bristlebriar", new List<Tag> { GameTags.DecorSeed }, default(Tag), 5, global::STRINGS.CREATURES.SPECIES.PRICKLEGRASS.DOMESTICATEDDESC);
		EntityTemplates.SetDescriptionOrder(gameObject);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
