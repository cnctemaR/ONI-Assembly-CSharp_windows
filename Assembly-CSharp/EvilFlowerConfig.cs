using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class EvilFlowerConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "EvilFlower";
		string text2 = CREATURES.SPECIES.EVILFLOWER.NAME;
		string text3 = CREATURES.SPECIES.EVILFLOWER.DESC;
		float num = 1f;
		KAnimFile kanimFile = Assets.GetAnim("potted_evilflower_kanim");
		string text4 = "grow_seed";
		EffectorValues decor_EFFECT = this.DECOR_EFFECT;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, kanimFile, text4, Grid.SceneLayer.BuildingFront, 1, 1, decor_EFFECT, default(EffectorValues), SimHashes.Creature, null, 293f);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 168.15f, 258.15f, 513.15f, 563.15f, new SimHashes[] { SimHashes.CarbonDioxide }, true, 0f, 0.15f, null, true, false);
		EvilFlower evilFlower = gameObject.AddOrGet<EvilFlower>();
		evilFlower.positive_decor_effect = this.DECOR_EFFECT;
		GameObject gameObject2 = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Hidden;
		text4 = "EvilFlowerSeed";
		text3 = CREATURES.SPECIES.SEEDS.EVILFLOWER.NAME;
		text2 = CREATURES.SPECIES.SEEDS.EVILFLOWER.DESC;
		kanimFile = Assets.GetAnim("seed_bristlebriar_kanim");
		List<Tag> list = new List<Tag> { GameTags.DecorSeed };
		GameObject gameObject3 = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject2, productionType, text4, text3, text2, kanimFile, "object", 1, list, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 5, CREATURES.SPECIES.EVILFLOWER.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.25f, 0.25f, null, string.Empty, false);
		EntityTemplates.CreateAndRegisterPreviewForPlant(gameObject3, "EvilFlower_preview", Assets.GetAnim("potted_evilflower_kanim"), "place", 1, 1);
		gameObject.AddOrGet<KBatchedAnimController>().randomiseLoopedOffset = true;
		DiseaseDropper.Def def = gameObject.AddOrGetDef<DiseaseDropper.Def>();
		def.diseaseIdx = Db.Get().Diseases.GetIndex("ZombieSpores");
		def.emitFrequency = 1f;
		def.averageEmitPerSecond = 1000;
		def.singleEmitQuantity = 100000;
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "EvilFlower";

	public const string SEED_ID = "EvilFlowerSeed";

	public readonly EffectorValues DECOR_EFFECT = new EffectorValues
	{
		amount = 1,
		radius = 5
	};
}
