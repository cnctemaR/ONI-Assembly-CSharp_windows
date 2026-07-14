using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class WaterCupsPlantConfig : IEntityConfig, IHasDlcRestrictions
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(WaterCupsPlantConfig.ID, global::STRINGS.CREATURES.SPECIES.WATERCUPS.NAME, global::STRINGS.CREATURES.SPECIES.WATERCUPS.DESC, 1f, Assets.GetAnim("watercups_kanim"), "idle", Grid.SceneLayer.BuildingFront, 1, 1, WaterCupsPlantConfig.POSITIVE_DECOR_EFFECT, NOISE_POLLUTION.NONE, SimHashes.Creature, null, 298.15f);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 288.15f, 293.15f, 323.15f, 373.15f, new SimHashes[]
		{
			SimHashes.Oxygen,
			SimHashes.ContaminatedOxygen,
			SimHashes.CarbonDioxide
		}, true, 0f, 0.15f, null, true, false, true, false, true, 2400f, 0f, 2200f, WaterCupsPlantConfig.BASETRAIT_ID, global::STRINGS.CREATURES.SPECIES.WATERCUPS.NAME);
		PrickleGrass prickleGrass = gameObject.AddOrGet<PrickleGrass>();
		prickleGrass.positive_decor_effect = WaterCupsPlantConfig.POSITIVE_DECOR_EFFECT;
		prickleGrass.negative_decor_effect = WaterCupsPlantConfig.NEGATIVE_DECOR_EFFECT;
		gameObject.AddOrGetDef<DecorPlantMonitor.Def>();
		GameObject gameObject2 = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Hidden;
		string seed_ID = WaterCupsPlantConfig.SEED_ID;
		string text = global::STRINGS.CREATURES.SPECIES.SEEDS.WATERCUPS.NAME;
		string text2 = global::STRINGS.CREATURES.SPECIES.SEEDS.WATERCUPS.DESC;
		KAnimFile anim = Assets.GetAnim("seed_watercups_kanim");
		string text3 = "object";
		int num = 1;
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.DecorSeed);
		SingleEntityReceptacle.ReceptacleDirection receptacleDirection = SingleEntityReceptacle.ReceptacleDirection.Top;
		string text4 = global::STRINGS.CREATURES.SPECIES.WATERCUPS.DOMESTICATEDDESC;
		EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(gameObject2, this, productionType, seed_ID, text, text2, anim, text3, num, list, receptacleDirection, default(Tag), 12, text4, EntityTemplates.CollisionShape.CIRCLE, 0.22f, 0.22f, null, "", false), WaterCupsPlantConfig.PREVIEW_ID, Assets.GetAnim("watercups_kanim"), "place", 1, 1);
		return gameObject;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC5;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public static string ID = "WaterCups";

	public static string SEED_ID = "WaterCupsSeed";

	public static string BASETRAIT_ID = "WaterCupsOriginal";

	public static string PREVIEW_ID = "WaterCupsPreview";

	public static EffectorValues POSITIVE_DECOR_EFFECT = DECOR.BONUS.TIER3;

	public static EffectorValues NEGATIVE_DECOR_EFFECT = DECOR.PENALTY.TIER3;
}
