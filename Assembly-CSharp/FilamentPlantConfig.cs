using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class FilamentPlantConfig : IEntityConfig, IHasDlcRestrictions
{
	public string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC5;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public GameObject CreatePrefab()
	{
		string id = FilamentPlantConfig.ID;
		string text = global::STRINGS.CREATURES.SPECIES.FILAMENTPLANT.NAME;
		string text2 = global::STRINGS.CREATURES.SPECIES.FILAMENTPLANT.DESC;
		float num = 1f;
		EffectorValues tier = DECOR.BONUS.TIER3;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, text, text2, num, Assets.GetAnim("potted_petta_pouf_kanim"), "idle", Grid.SceneLayer.BuildingFront, 1, 1, tier, default(EffectorValues), SimHashes.Creature, null, 303.15f);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 295.15f, 299.15f, 315.15f, 311.15f, PLANTS.SAFE_ELEMENTS.AllWaters, false, 0f, 0.15f, null, false, false, true, false, true, 2400f, 0f, 2200f, FilamentPlantConfig.ID + "Original", global::STRINGS.CREATURES.SPECIES.FILAMENTPLANT.NAME);
		gameObject.AddOrGetDef<DecorPlantMonitor.Def>();
		PrickleGrass prickleGrass = gameObject.AddOrGet<PrickleGrass>();
		prickleGrass.positive_decor_effect = DECOR.BONUS.TIER3;
		prickleGrass.negative_decor_effect = DECOR.PENALTY.TIER3;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Hidden;
		string seed_ID = FilamentPlantConfig.SEED_ID;
		string text3 = global::STRINGS.CREATURES.SPECIES.SEEDS.FILAMENTPLANT.NAME;
		string text4 = global::STRINGS.CREATURES.SPECIES.SEEDS.FILAMENTPLANT.DESC;
		KAnimFile anim = Assets.GetAnim("seed_potted_petta_pouf_kanim");
		string text5 = "object";
		int num2 = 1;
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.DecorSeed);
		SingleEntityReceptacle.ReceptacleDirection receptacleDirection = SingleEntityReceptacle.ReceptacleDirection.Top;
		string text6 = global::STRINGS.CREATURES.SPECIES.FILAMENTPLANT.DOMESTICATEDDESC;
		EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, this, productionType, seed_ID, text3, text4, anim, text5, num2, list, receptacleDirection, default(Tag), 13, text6, EntityTemplates.CollisionShape.CIRCLE, 0.25f, 0.25f, null, "", false), FilamentPlantConfig.ID + "_preview", Assets.GetAnim("filament_plant_kanim"), "place", 1, 1);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public static readonly string ID = "FilamentPlant";

	public static readonly string SEED_ID = "FilamentPlantSeed";

	public static readonly EffectorValues POSITIVE_DECOR_EFFECT = DECOR.BONUS.TIER3;

	public static readonly EffectorValues NEGATIVE_DECOR_EFFECT = DECOR.PENALTY.TIER3;
}
