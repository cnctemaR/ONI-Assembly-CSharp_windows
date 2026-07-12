using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class WineCupsConfig : IEntityConfig
{
	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public GameObject CreatePrefab()
	{
		string text = "WineCups";
		string text2 = global::STRINGS.CREATURES.SPECIES.WINECUPS.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.WINECUPS.DESC;
		float num = 1f;
		EffectorValues positive_DECOR_EFFECT = WineCupsConfig.POSITIVE_DECOR_EFFECT;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("potted_cups_kanim"), "grow_seed", Grid.SceneLayer.BuildingFront, 1, 1, positive_DECOR_EFFECT, default(EffectorValues), SimHashes.Creature, null, 293f);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 218.15f, 283.15f, 303.15f, 398.15f, new SimHashes[]
		{
			SimHashes.Oxygen,
			SimHashes.ContaminatedOxygen,
			SimHashes.CarbonDioxide
		}, true, 0f, 0.15f, null, true, false, true, true, 2400f, 0f, 900f, "WineCupsOriginal", global::STRINGS.CREATURES.SPECIES.WINECUPS.NAME);
		PrickleGrass prickleGrass = gameObject.AddOrGet<PrickleGrass>();
		prickleGrass.positive_decor_effect = WineCupsConfig.POSITIVE_DECOR_EFFECT;
		prickleGrass.negative_decor_effect = WineCupsConfig.NEGATIVE_DECOR_EFFECT;
		EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Hidden, "WineCupsSeed", global::STRINGS.CREATURES.SPECIES.SEEDS.WINECUPS.NAME, global::STRINGS.CREATURES.SPECIES.SEEDS.WINECUPS.DESC, Assets.GetAnim("seed_potted_cups_kanim"), "object", 1, new List<Tag> { GameTags.DecorSeed }, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 11, global::STRINGS.CREATURES.SPECIES.WINECUPS.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.25f, 0.25f, null, "", false, this.GetDlcIds()), "WineCups_preview", Assets.GetAnim("potted_cups_kanim"), "place", 1, 1);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "WineCups";

	public const string SEED_ID = "WineCupsSeed";

	public static readonly EffectorValues POSITIVE_DECOR_EFFECT = DECOR.BONUS.TIER3;

	public static readonly EffectorValues NEGATIVE_DECOR_EFFECT = DECOR.PENALTY.TIER3;
}
