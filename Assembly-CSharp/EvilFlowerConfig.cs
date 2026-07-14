using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class EvilFlowerConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "EvilFlower";
		string text2 = global::STRINGS.CREATURES.SPECIES.EVILFLOWER.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.EVILFLOWER.DESC;
		float num = 1f;
		EffectorValues positive_DECOR_EFFECT = this.POSITIVE_DECOR_EFFECT;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("potted_evilflower_kanim"), "grow_seed", Grid.SceneLayer.BuildingFront, 1, 1, positive_DECOR_EFFECT, default(EffectorValues), SimHashes.Creature, null, 293f);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 168.15f, 258.15f, 513.15f, 563.15f, new SimHashes[] { SimHashes.CarbonDioxide }, true, 0f, 0.15f, null, true, false, true, false, true, 2400f, 0f, 12200f, "EvilFlowerOriginal", global::STRINGS.CREATURES.SPECIES.EVILFLOWER.NAME);
		EvilFlower evilFlower = gameObject.AddOrGet<EvilFlower>();
		evilFlower.positive_decor_effect = this.POSITIVE_DECOR_EFFECT;
		evilFlower.negative_decor_effect = this.NEGATIVE_DECOR_EFFECT;
		GameObject gameObject2 = gameObject;
		IHasDlcRestrictions hasDlcRestrictions = this as IHasDlcRestrictions;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Hidden;
		string text4 = "EvilFlowerSeed";
		string text5 = global::STRINGS.CREATURES.SPECIES.SEEDS.EVILFLOWER.NAME;
		string text6 = global::STRINGS.CREATURES.SPECIES.SEEDS.EVILFLOWER.DESC;
		KAnimFile anim = Assets.GetAnim("seed_potted_evilflower_kanim");
		string text7 = "object";
		int num2 = 1;
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.DecorSeed);
		SingleEntityReceptacle.ReceptacleDirection receptacleDirection = SingleEntityReceptacle.ReceptacleDirection.Top;
		string text8 = global::STRINGS.CREATURES.SPECIES.EVILFLOWER.DOMESTICATEDDESC;
		EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(gameObject2, hasDlcRestrictions, productionType, text4, text5, text6, anim, text7, num2, list, receptacleDirection, default(Tag), 19, text8, EntityTemplates.CollisionShape.CIRCLE, 0.4f, 0.4f, null, "", false), "EvilFlower_preview", Assets.GetAnim("potted_evilflower_kanim"), "place", 1, 1);
		DiseaseDropper.Def def = gameObject.AddOrGetDef<DiseaseDropper.Def>();
		def.diseaseIdx = Db.Get().Diseases.GetIndex("ZombieSpores");
		def.emitFrequency = 1f;
		def.averageEmitPerSecond = 1000;
		def.singleEmitQuantity = 100000;
		gameObject.AddOrGet<DiseaseSourceVisualizer>().alwaysShowDisease = "ZombieSpores";
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

	public readonly EffectorValues POSITIVE_DECOR_EFFECT = DECOR.BONUS.TIER7;

	public readonly EffectorValues NEGATIVE_DECOR_EFFECT = DECOR.PENALTY.TIER5;

	public const int GERMS_PER_SECOND = 1000;
}
