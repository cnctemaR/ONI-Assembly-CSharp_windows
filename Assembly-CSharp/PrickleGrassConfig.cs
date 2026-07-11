using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class PrickleGrassConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "PrickleGrass";
		string text2 = CREATURES.SPECIES.PRICKLEGRASS.NAME;
		string text3 = CREATURES.SPECIES.PRICKLEGRASS.DESC;
		float num = 1f;
		KAnimFile kanimFile = Assets.GetAnim("bristlebriar_kanim");
		string text4 = "grow_seed";
		EffectorValues effectorValues = new EffectorValues
		{
			amount = 1,
			radius = 5
		};
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, kanimFile, text4, Grid.SceneLayer.BuildingFront, 1, 1, effectorValues, default(EffectorValues), SimHashes.Creature, null, 293f);
		GameObject gameObject2 = gameObject;
		SimHashes[] array = new SimHashes[]
		{
			SimHashes.Oxygen,
			SimHashes.ContaminatedOxygen,
			SimHashes.CarbonDioxide
		};
		EntityTemplates.ExtendEntityToBasicPlant(gameObject2, 218.15f, 283.15f, 303.15f, 398.15f, array, true, 0f, 0.15f, null, true, false);
		gameObject.AddOrGet<PrickleGrass>();
		gameObject2 = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Hidden;
		text4 = "PrickleGrassSeed";
		text3 = CREATURES.SPECIES.SEEDS.PRICKLEGRASS.NAME;
		text2 = CREATURES.SPECIES.SEEDS.PRICKLEGRASS.DESC;
		kanimFile = Assets.GetAnim("seed_bristlebriar_kanim");
		List<Tag> list = new List<Tag> { GameTags.DecorSeed };
		GameObject gameObject3 = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject2, productionType, text4, text3, text2, kanimFile, "object", 1, list, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 5, CREATURES.SPECIES.PRICKLEGRASS.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.25f, 0.25f, null, string.Empty);
		EntityTemplates.CreateAndRegisterPreviewForPlant(gameObject3, "PrickleGrass_preview", Assets.GetAnim("bristlebriar_kanim"), "place", 1, 1);
		gameObject.AddOrGet<KBatchedAnimController>().randomiseLoopedOffset = true;
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
