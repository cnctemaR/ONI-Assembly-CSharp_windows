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
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("PrickleGrass", global::STRINGS.CREATURES.SPECIES.PRICKLEGRASS.NAME, global::STRINGS.CREATURES.SPECIES.PRICKLEGRASS.DESC, 400f, Assets.GetAnim("bristlebriar_kanim"), "grow_seed", Grid.SceneLayer.BuildingFront, 1, 1, tier, SimHashes.Creature, null);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 15f, 5f, 218.15f, 283.15f, 291.15f, 295.15f, 303.15f, 398.15f, 0f, 0.15f, 1f, null);
		gameObject.AddOrGet<PrickleGrass>();
		List<Tag> list = new List<Tag> { GameTags.DecorSeed };
		GameObject gameObject2 = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Hidden, "PrickleGrassSeed", global::STRINGS.CREATURES.SPECIES.SEEDS.PRICKLEGRASS.NAME, global::STRINGS.CREATURES.SPECIES.SEEDS.PRICKLEGRASS.DESC, Assets.GetAnim("seed_bristlebriar_kanim"), "object", 1, list, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 5, global::STRINGS.CREATURES.SPECIES.PRICKLEGRASS.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.25f, 0.25f);
		EntityTemplates.CreateAndRegisterPreviewForPlant(gameObject, gameObject2, "PrickleGrass_preview", Assets.GetAnim("bristlebriar_kanim"), "place", 1, 1);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "PrickleGrass";

	public const string SEED_ID = "PrickleGrassSeed";
}
