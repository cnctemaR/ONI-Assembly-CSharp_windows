using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class OxyCoralConfig : IEntityConfig, IHasDlcRestrictions
{
	public static float OXYGEN_PER_SECOND
	{
		get
		{
			return DUPLICANTSTATS.STANDARD.BaseStats.OXYGEN_USED_PER_SECOND * 1.5f;
		}
	}

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
		string text = "OxyCoral";
		string text2 = global::STRINGS.CREATURES.SPECIES.OXYCORAL.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.OXYCORAL.DESC;
		float num = 1f;
		EffectorValues tier = DECOR.BONUS.TIER2;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("thalassaire_coral_kanim"), "grow", Grid.SceneLayer.BuildingBack, 3, 2, tier, default(EffectorValues), SimHashes.Creature, null, 303.15f);
		gameObject = EntityTemplates.ExtendEntityToBasicPlant(gameObject, 253.15f, 298.15f, 318.15f, 373.15f, new SimHashes[]
		{
			SimHashes.Water,
			SimHashes.SaltWater,
			SimHashes.DirtyWater,
			SimHashes.Brine
		}, false, 0f, 0.15f, null, false, false, true, false, true, 2400f, 0f, 2200f, "OxyCoralOriginal", global::STRINGS.CREATURES.SPECIES.OXYCORAL.NAME);
		gameObject.AddOrGet<LoopingSounds>();
		GameObject gameObject2 = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Hidden;
		string text4 = "OxyCoralSeed";
		string text5 = global::STRINGS.CREATURES.SPECIES.SEEDS.OXYCORAL.NAME;
		string text6 = global::STRINGS.CREATURES.SPECIES.SEEDS.OXYCORAL.DESC;
		KAnimFile anim = Assets.GetAnim("seed_thalassaire_coral_kanim");
		string text7 = "object";
		int num2 = 1;
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.LargeSeed);
		SingleEntityReceptacle.ReceptacleDirection receptacleDirection = SingleEntityReceptacle.ReceptacleDirection.Top;
		string text8 = global::STRINGS.CREATURES.SPECIES.OXYCORAL.DOMESTICATEDDESC;
		EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(gameObject2, this, productionType, text4, text5, text6, anim, text7, num2, list, receptacleDirection, default(Tag), 20, text8, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f, null, "", false), "OxyCoral_preview", Assets.GetAnim("thalassaire_coral_kanim"), "place", 3, 2);
		EntityTemplates.ExtendPlantToIrrigated(gameObject, new PlantElementAbsorber.ConsumeInfo[]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = ElementLoader.FindElementByHash(SimHashes.SaltWater).tag,
				massConsumptionRate = 0.033333335f
			}
		});
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new PlantElementAbsorber.ConsumeInfo[]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = SimHashes.Lime.CreateTag(),
				massConsumptionRate = 0.008333334f
			}
		});
		gameObject.AddTag(GameTags.BlockBuildOverPlantFeature);
		OxyCoral.Def def = gameObject.AddOrGetDef<OxyCoral.Def>();
		def.OxygenProductionRate = OxyCoralConfig.OXYGEN_PER_SECOND;
		def.MinLuxRequired = 2500;
		def.OutputBubbleCells = new CellOffset[]
		{
			new CellOffset(-1, 1),
			new CellOffset(0, 1),
			new CellOffset(1, 1)
		};
		gameObject.AddOrGet<PressureVulnerable>().allCellsMustBeSafe = true;
		SoundEventVolumeCache.instance.AddVolume("oxy_fern_kanim", "MealLice_harvest", NOISE_POLLUTION.CREATURES.TIER3);
		SoundEventVolumeCache.instance.AddVolume("oxy_fern_kanim", "MealLice_LP", NOISE_POLLUTION.CREATURES.TIER4);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "OxyCoral";

	public const string SEED_ID = "OxyCoralSeed";

	public const int MIN_LUX_REQUIRED = 2500;

	public const float MINIONS_SUPPORTED_PER_PLANT = 1.5f;

	public const float LIME_CONSUMPTION_RATE = 0.008333334f;

	public const float WATER_CONSUMPTION_RATE = 0.033333335f;
}
