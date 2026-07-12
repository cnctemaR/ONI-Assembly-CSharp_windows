using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class CactusPlantConfig : IEntityConfig
{
	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		string text = "CactusPlant";
		string text2 = global::STRINGS.CREATURES.SPECIES.CACTUSPLANT.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.CACTUSPLANT.DESC;
		float num = 1f;
		EffectorValues positive_DECOR_EFFECT = this.POSITIVE_DECOR_EFFECT;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("potted_cactus_kanim"), "grow_seed", Grid.SceneLayer.BuildingFront, 1, 1, positive_DECOR_EFFECT, default(EffectorValues), SimHashes.Creature, null, 293f);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 200f, 273.15f, 373.15f, 400f, new SimHashes[]
		{
			SimHashes.Oxygen,
			SimHashes.ContaminatedOxygen,
			SimHashes.CarbonDioxide
		}, false, 0f, 0.15f, null, true, false, true, true, 2400f, 0f, 2200f, "CactusPlantOriginal", global::STRINGS.CREATURES.SPECIES.CACTUSPLANT.NAME);
		PrickleGrass prickleGrass = gameObject.AddOrGet<PrickleGrass>();
		prickleGrass.positive_decor_effect = this.POSITIVE_DECOR_EFFECT;
		prickleGrass.negative_decor_effect = this.NEGATIVE_DECOR_EFFECT;
		EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Hidden, "CactusPlantSeed", global::STRINGS.CREATURES.SPECIES.SEEDS.CACTUSPLANT.NAME, global::STRINGS.CREATURES.SPECIES.SEEDS.CACTUSPLANT.DESC, Assets.GetAnim("seed_potted_cactus_kanim"), "object", 1, new List<Tag> { GameTags.DecorSeed }, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 13, global::STRINGS.CREATURES.SPECIES.CACTUSPLANT.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.25f, 0.25f, null, "", false, null), "CactusPlant_preview", Assets.GetAnim("potted_cactus_kanim"), "place", 1, 1);
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

	public readonly EffectorValues POSITIVE_DECOR_EFFECT = DECOR.BONUS.TIER3;

	public readonly EffectorValues NEGATIVE_DECOR_EFFECT = DECOR.PENALTY.TIER3;
}
