using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class PlanktonCoralConfig : IEntityConfig, IHasDlcRestrictions
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
		string text = "PlanktonCoral";
		string text2 = global::STRINGS.CREATURES.SPECIES.PLANKTONCORAL.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.PLANKTONCORAL.DESC;
		float num = 1f;
		EffectorValues tier = DECOR.BONUS.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("parrotfish_coral_kanim"), "idle_full", Grid.SceneLayer.Building, 2, 2, tier, default(EffectorValues), SimHashes.Creature, null, 303.15f);
		GameObject gameObject2 = gameObject;
		float num2 = 273.15f;
		float num3 = 298.15f;
		float num4 = 318.15f;
		float num5 = 373.15f;
		string text4 = SimHashes.Phosphorite.ToString();
		gameObject = EntityTemplates.ExtendEntityToBasicPlant(gameObject2, num2, num3, num4, num5, PLANTS.SAFE_ELEMENTS.AllWaters, false, 0f, 0.15f, text4, false, true, false, true, true, 2400f, 0f, 2200f, "PlanktonCoralOriginal", global::STRINGS.CREATURES.SPECIES.PLANKTONCORAL.NAME);
		gameObject.AddOrGet<StandardCropPlant>();
		gameObject.AddOrGet<LoopingSounds>();
		GameObject gameObject3 = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Harvest;
		string text5 = "PlanktonCoralSeed";
		string text6 = global::STRINGS.CREATURES.SPECIES.SEEDS.PLANKTONCORAL.NAME;
		string text7 = global::STRINGS.CREATURES.SPECIES.SEEDS.PLANKTONCORAL.DESC;
		KAnimFile anim = Assets.GetAnim("seed_parrotfish_coral_kanim");
		string text8 = "object";
		int num6 = 1;
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.CropSeed);
		list.Add(GameTags.BackwallSeed);
		SingleEntityReceptacle.ReceptacleDirection receptacleDirection = SingleEntityReceptacle.ReceptacleDirection.Top;
		text4 = global::STRINGS.CREATURES.SPECIES.PLANKTONCORAL.DOMESTICATEDDESC;
		EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(gameObject3, this, productionType, text5, text6, text7, anim, text8, num6, list, receptacleDirection, default(Tag), 20, text4, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f, null, "", false), "PlanktonCoral_preview", Assets.GetAnim("parrotfish_coral_kanim"), "place", 2, 2);
		PlantElementAbsorber.ConsumeInfo[] array = new PlantElementAbsorber.ConsumeInfo[]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = SimHashes.Coquina.CreateTag(),
				massConsumptionRate = 0.033333335f
			}
		};
		EntityTemplates.ExtendPlantToFertilizable(gameObject, array);
		gameObject.AddOrGet<PressureVulnerable>().allCellsMustBeSafe = true;
		gameObject.AddOrGet<DirectlyEdiblePlant_Growth>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
		EntityTemplates.ExtendPlantEntityToRequireBackwall(prefab);
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "PlanktonCoral";

	public const string SEED_ID = "PlanktonCoralSeed";

	public const float LIFETIME_CYCLES = 4f;

	public const int HARVEST_YIELD_MASS = 80;

	public const float CALCULATED_YIELD_MASS_PER_HARVEST = 80f;

	public const float CALCULATED_YIELD_MASS_PER_CYCLE = 20f;

	public const float CALCULATED_GROWTH_PER_CYCLE = 0.25f;

	public const float CALCULATED_LIFETIME_SEC = 2400f;

	public const float CALCIUM_CARBONATE_CONSUMPTION_RATE = 0.025f;
}
