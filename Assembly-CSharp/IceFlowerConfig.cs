using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class IceFlowerConfig : IEntityConfig
{
	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_DLC_2;
	}

	public GameObject CreatePrefab()
	{
		string text = "IceFlower";
		string text2 = global::STRINGS.CREATURES.SPECIES.ICEFLOWER.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.ICEFLOWER.DESC;
		float num = 1f;
		EffectorValues positive_DECOR_EFFECT = this.POSITIVE_DECOR_EFFECT;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("potted_ice_flower_kanim"), "grow_seed", Grid.SceneLayer.BuildingFront, 1, 1, positive_DECOR_EFFECT, default(EffectorValues), SimHashes.Creature, null, 243.15f);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 173.15f, 203.15f, 278.15f, 318.15f, new SimHashes[]
		{
			SimHashes.Oxygen,
			SimHashes.ContaminatedOxygen,
			SimHashes.CarbonDioxide,
			SimHashes.ChlorineGas,
			SimHashes.Hydrogen
		}, true, 0f, 0.15f, null, true, false, true, true, 2400f, 0f, 2200f, "IceFlowerOriginal", global::STRINGS.CREATURES.SPECIES.ICEFLOWER.NAME);
		PrickleGrass prickleGrass = gameObject.AddOrGet<PrickleGrass>();
		prickleGrass.positive_decor_effect = this.POSITIVE_DECOR_EFFECT;
		prickleGrass.negative_decor_effect = this.NEGATIVE_DECOR_EFFECT;
		EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Hidden, "IceFlowerSeed", global::STRINGS.CREATURES.SPECIES.SEEDS.ICEFLOWER.NAME, global::STRINGS.CREATURES.SPECIES.SEEDS.ICEFLOWER.DESC, Assets.GetAnim("seed_ice_flower_kanim"), "object", 1, new List<Tag> { GameTags.DecorSeed }, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 12, global::STRINGS.CREATURES.SPECIES.ICEFLOWER.DOMESTICATEDDESC, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.6f, null, "", false, this.GetDlcIds()), "IceFlower_preview", Assets.GetAnim("potted_ice_flower_kanim"), "place", 1, 1);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "IceFlower";

	public const string SEED_ID = "IceFlowerSeed";

	public readonly EffectorValues POSITIVE_DECOR_EFFECT = DECOR.BONUS.TIER3;

	public readonly EffectorValues NEGATIVE_DECOR_EFFECT = DECOR.PENALTY.TIER3;
}
