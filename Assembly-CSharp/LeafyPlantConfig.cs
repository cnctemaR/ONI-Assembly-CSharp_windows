using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class LeafyPlantConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "LeafyPlant";
		string text2 = global::STRINGS.CREATURES.SPECIES.LEAFYPLANT.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.LEAFYPLANT.DESC;
		float num = 1f;
		KAnimFile kanimFile = Assets.GetAnim("potted_leafy_kanim");
		string text4 = "grow_seed";
		EffectorValues positive_DECOR_EFFECT = this.POSITIVE_DECOR_EFFECT;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, kanimFile, text4, Grid.SceneLayer.BuildingFront, 1, 1, positive_DECOR_EFFECT, default(EffectorValues), SimHashes.Creature, null, 293f);
		GameObject gameObject2 = gameObject;
		SimHashes[] array = new SimHashes[]
		{
			SimHashes.Oxygen,
			SimHashes.ContaminatedOxygen,
			SimHashes.CarbonDioxide,
			SimHashes.ChlorineGas,
			SimHashes.Hydrogen
		};
		EntityTemplates.ExtendEntityToBasicPlant(gameObject2, 288f, 293.15f, 323.15f, 373f, array, true, 0f, 0.15f, null, true, false, true, true, 2400f);
		PrickleGrass prickleGrass = gameObject.AddOrGet<PrickleGrass>();
		prickleGrass.positive_decor_effect = this.POSITIVE_DECOR_EFFECT;
		prickleGrass.negative_decor_effect = this.NEGATIVE_DECOR_EFFECT;
		gameObject2 = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Hidden;
		text4 = "LeafyPlantSeed";
		text3 = global::STRINGS.CREATURES.SPECIES.SEEDS.LEAFYPLANT.NAME;
		text2 = global::STRINGS.CREATURES.SPECIES.SEEDS.LEAFYPLANT.DESC;
		kanimFile = Assets.GetAnim("seed_potted_leafy_kanim");
		List<Tag> list = new List<Tag> { GameTags.DecorSeed };
		GameObject gameObject3 = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject2, productionType, text4, text3, text2, kanimFile, "object", 1, list, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 7, global::STRINGS.CREATURES.SPECIES.LEAFYPLANT.DOMESTICATEDDESC, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.6f, null, string.Empty, false);
		EntityTemplates.CreateAndRegisterPreviewForPlant(gameObject3, "LeafyPlant_preview", Assets.GetAnim("potted_leafy_kanim"), "place", 1, 1);
		gameObject.AddOrGet<KBatchedAnimController>().randomiseLoopedOffset = true;
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "LeafyPlant";

	public const string SEED_ID = "LeafyPlantSeed";

	public readonly EffectorValues POSITIVE_DECOR_EFFECT = DECOR.BONUS.TIER3;

	public readonly EffectorValues NEGATIVE_DECOR_EFFECT = DECOR.PENALTY.TIER3;
}
