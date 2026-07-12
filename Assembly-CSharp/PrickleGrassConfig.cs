using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class PrickleGrassConfig : IEntityConfig
{
	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		string text = "PrickleGrass";
		string text2 = global::STRINGS.CREATURES.SPECIES.PRICKLEGRASS.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.PRICKLEGRASS.DESC;
		float num = 1f;
		EffectorValues positive_DECOR_EFFECT = PrickleGrassConfig.POSITIVE_DECOR_EFFECT;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("bristlebriar_kanim"), "grow_seed", Grid.SceneLayer.BuildingFront, 1, 1, positive_DECOR_EFFECT, default(EffectorValues), SimHashes.Creature, null, 293f);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 218.15f, 283.15f, 303.15f, 398.15f, new SimHashes[]
		{
			SimHashes.Oxygen,
			SimHashes.ContaminatedOxygen,
			SimHashes.CarbonDioxide
		}, true, 0f, 0.15f, null, true, false, true, true, 2400f, 0f, 900f, "PrickleGrassOriginal", global::STRINGS.CREATURES.SPECIES.PRICKLEGRASS.NAME);
		PrickleGrass prickleGrass = gameObject.AddOrGet<PrickleGrass>();
		prickleGrass.positive_decor_effect = PrickleGrassConfig.POSITIVE_DECOR_EFFECT;
		prickleGrass.negative_decor_effect = PrickleGrassConfig.NEGATIVE_DECOR_EFFECT;
		EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Hidden, "PrickleGrassSeed", global::STRINGS.CREATURES.SPECIES.SEEDS.PRICKLEGRASS.NAME, global::STRINGS.CREATURES.SPECIES.SEEDS.PRICKLEGRASS.DESC, Assets.GetAnim("seed_bristlebriar_kanim"), "object", 1, new List<Tag> { GameTags.DecorSeed }, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 10, global::STRINGS.CREATURES.SPECIES.PRICKLEGRASS.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.25f, 0.25f, null, "", false, null), "PrickleGrass_preview", Assets.GetAnim("bristlebriar_kanim"), "place", 1, 1);
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

	public static readonly EffectorValues POSITIVE_DECOR_EFFECT = DECOR.BONUS.TIER3;

	public static readonly EffectorValues NEGATIVE_DECOR_EFFECT = DECOR.PENALTY.TIER3;
}
