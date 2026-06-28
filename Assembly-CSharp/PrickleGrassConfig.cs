using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class PrickleGrassConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		EffectorValues effectorValues = new EffectorValues
		{
			amount = 1,
			radius = 5
		};
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("PrickleGrass", CREATURES.SPECIES.PRICKLEGRASS.NAME, CREATURES.SPECIES.PRICKLEGRASS.DESC, 1f, Assets.GetAnim("bristlebriar_kanim"), "grow_seed", Grid.SceneLayer.BuildingFront, 1, 1, effectorValues, default(EffectorValues), SimHashes.Creature, null, 293f);
		SimHashes[] array = new SimHashes[]
		{
			SimHashes.Oxygen,
			SimHashes.ContaminatedOxygen,
			SimHashes.CarbonDioxide
		};
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 15f, 5f, 218.15f, 283.15f, 291.15f, 295.15f, 303.15f, 398.15f, array, true, 0f, 0.15f, 1f, null, true);
		gameObject.AddOrGet<PrickleGrass>();
		List<Tag> list = new List<Tag> { GameTags.DecorSeed };
		GameObject gameObject2 = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Hidden, "PrickleGrassSeed", CREATURES.SPECIES.SEEDS.PRICKLEGRASS.NAME, CREATURES.SPECIES.SEEDS.PRICKLEGRASS.DESC, Assets.GetAnim("seed_bristlebriar_kanim"), "object", 1, list, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 5, CREATURES.SPECIES.PRICKLEGRASS.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.25f, 0.25f, null, string.Empty);
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
