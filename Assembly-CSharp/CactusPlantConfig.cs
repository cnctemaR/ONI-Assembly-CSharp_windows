using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class CactusPlantConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "CactusPlant";
		string text2 = CREATURES.SPECIES.CACTUSPLANT.NAME;
		string text3 = CREATURES.SPECIES.CACTUSPLANT.DESC;
		float num = 1f;
		KAnimFile kanimFile = Assets.GetAnim("potted_cactus_kanim");
		string text4 = "grow_seed";
		EffectorValues decor_EFFECT = this.DECOR_EFFECT;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, kanimFile, text4, Grid.SceneLayer.BuildingFront, 1, 1, decor_EFFECT, default(EffectorValues), SimHashes.Creature, null, 293f);
		GameObject gameObject2 = gameObject;
		SimHashes[] array = new SimHashes[]
		{
			SimHashes.Oxygen,
			SimHashes.ContaminatedOxygen,
			SimHashes.CarbonDioxide
		};
		EntityTemplates.ExtendEntityToBasicPlant(gameObject2, 200f, 273.15f, 373.15f, 400f, array, false, 0f, 0.15f, null, true, false);
		PrickleGrass prickleGrass = gameObject.AddOrGet<PrickleGrass>();
		prickleGrass.positive_decor_effect = this.DECOR_EFFECT;
		gameObject2 = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Hidden;
		text4 = "CactusPlantSeed";
		text3 = CREATURES.SPECIES.SEEDS.CACTUSPLANT.NAME;
		text2 = CREATURES.SPECIES.SEEDS.CACTUSPLANT.DESC;
		kanimFile = Assets.GetAnim("seed_potted_cactus_kanim");
		List<Tag> list = new List<Tag> { GameTags.DecorSeed };
		GameObject gameObject3 = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject2, productionType, text4, text3, text2, kanimFile, "object", 1, list, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 8, CREATURES.SPECIES.CACTUSPLANT.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.25f, 0.25f, null, string.Empty, false);
		EntityTemplates.CreateAndRegisterPreviewForPlant(gameObject3, "CactusPlant_preview", Assets.GetAnim("potted_cactus_kanim"), "place", 1, 1);
		gameObject.AddOrGet<KBatchedAnimController>().randomiseLoopedOffset = true;
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "CactusPlant";

	public const string SEED_ID = "CactusPlantSeed";

	public readonly EffectorValues DECOR_EFFECT = new EffectorValues
	{
		amount = 1,
		radius = 5
	};
}
