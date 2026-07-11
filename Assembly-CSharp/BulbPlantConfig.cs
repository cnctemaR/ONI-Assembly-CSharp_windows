using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class BulbPlantConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "BulbPlant";
		string text2 = CREATURES.SPECIES.BULBPLANT.NAME;
		string text3 = CREATURES.SPECIES.BULBPLANT.DESC;
		float num = 1f;
		KAnimFile kanimFile = Assets.GetAnim("potted_bulb_kanim");
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
		EntityTemplates.ExtendEntityToBasicPlant(gameObject2, 288f, 293.15f, 313.15f, 333.15f, array, true, 0f, 0.15f, null, true, false);
		PrickleGrass prickleGrass = gameObject.AddOrGet<PrickleGrass>();
		prickleGrass.positive_decor_effect = this.DECOR_EFFECT;
		gameObject2 = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Hidden;
		text4 = "BulbPlantSeed";
		text3 = CREATURES.SPECIES.SEEDS.BULBPLANT.NAME;
		text2 = CREATURES.SPECIES.SEEDS.BULBPLANT.DESC;
		kanimFile = Assets.GetAnim("seed_potted_bulb_kanim");
		List<Tag> list = new List<Tag> { GameTags.DecorSeed };
		GameObject gameObject3 = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject2, productionType, text4, text3, text2, kanimFile, "object", 1, list, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 6, CREATURES.SPECIES.BULBPLANT.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.25f, 0.25f, null, string.Empty, false);
		EntityTemplates.CreateAndRegisterPreviewForPlant(gameObject3, "BulbPlant_preview", Assets.GetAnim("potted_bulb_kanim"), "place", 1, 1);
		gameObject.AddOrGet<KBatchedAnimController>().randomiseLoopedOffset = true;
		DiseaseDropper.Def def = gameObject.AddOrGetDef<DiseaseDropper.Def>();
		def.diseaseIdx = Db.Get().Diseases.GetIndex(Db.Get().Diseases.PollenGerms.id);
		def.singleEmitQuantity = 0;
		def.averageEmitPerSecond = 5000;
		def.emitFrequency = 5f;
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "BulbPlant";

	public const string SEED_ID = "BulbPlantSeed";

	public readonly EffectorValues DECOR_EFFECT = new EffectorValues
	{
		amount = 1,
		radius = 5
	};
}
