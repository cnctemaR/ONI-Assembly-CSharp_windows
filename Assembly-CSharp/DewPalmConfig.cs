using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class DewPalmConfig : IEntityConfig, IHasDlcRestrictions
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
		string id = DewPalmConfig.ID;
		string text = global::STRINGS.CREATURES.SPECIES.DEWPALM.NAME;
		string text2 = global::STRINGS.CREATURES.SPECIES.DEWPALM.DESC;
		float num = 100f;
		KAnimFile anim = Assets.GetAnim("rubber_tree_kanim");
		string text3 = "idle_empty";
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.BuildingBack;
		int num2 = 3;
		int num3 = 4;
		EffectorValues tier = DECOR.BONUS.TIER2;
		float hot = global::TUNING.CREATURES.TEMPERATURE.HOT;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, text, text2, num, anim, text3, sceneLayer, num2, num3, tier, default(EffectorValues), SimHashes.Creature, null, hot);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, GameUtil.GetTemperatureConvertedToKelvin(16f, GameUtil.TemperatureUnit.Celsius), GameUtil.GetTemperatureConvertedToKelvin(24f, GameUtil.TemperatureUnit.Celsius), GameUtil.GetTemperatureConvertedToKelvin(54f, GameUtil.TemperatureUnit.Celsius), GameUtil.GetTemperatureConvertedToKelvin(56f, GameUtil.TemperatureUnit.Celsius), null, true, 0f, 0.15f, SimHashes.PalmWood.ToString(), true, true, true, false, true, 12000f, 0f, 2200f, DewPalmConfig.BASE_TRAIT_ID, global::STRINGS.CREATURES.SPECIES.DEWPALM.NAME);
		PlantElementAbsorber.ConsumeInfo[] array = new PlantElementAbsorber.ConsumeInfo[]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = SimHashes.Sulfur.CreateTag(),
				massConsumptionRate = 0.033333335f
			}
		};
		EntityTemplates.ExtendPlantToFertilizable(gameObject, array);
		gameObject.AddOrGet<StandardCropPlant>();
		GameObject gameObject2 = gameObject;
		IHasDlcRestrictions hasDlcRestrictions = null;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Harvest;
		string seed_ID = DewPalmConfig.SEED_ID;
		string text4 = global::STRINGS.CREATURES.SPECIES.SEEDS.DEWPALM.NAME;
		string text5 = global::STRINGS.CREATURES.SPECIES.SEEDS.DEWPALM.DESC;
		KAnimFile anim2 = Assets.GetAnim("seed_rubbertree_kanim");
		string text6 = "object";
		int num4 = 1;
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.LargeSeed);
		SingleEntityReceptacle.ReceptacleDirection receptacleDirection = SingleEntityReceptacle.ReceptacleDirection.Top;
		string text7 = global::STRINGS.CREATURES.SPECIES.DEWPALM.DOMESTICATEDDESC;
		EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(gameObject2, hasDlcRestrictions, productionType, seed_ID, text4, text5, anim2, text6, num4, list, receptacleDirection, default(Tag), 3, text7, EntityTemplates.CollisionShape.CIRCLE, 0.45f, 0.33f, null, "", false), DewPalmConfig.PREVIEW_ID, Assets.GetAnim("rubber_tree_kanim"), "place", 3, 4);
		gameObject.AddOrGet<DirectlyEdiblePlant_Growth>();
		gameObject.AddTag(GameTags.BlockBuildOverPlantFeature);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public static readonly string ID = "DewPalm";

	public static readonly string SEED_ID = "DewPalmSeed";

	public static readonly string PREVIEW_ID = "DewPalmPreview";

	public static readonly string BASE_TRAIT_ID = "DewPalmOriginal";

	public const float GROWTH_CYCLES = 10f;

	public const int WOOD_HARVEST_YIELD = 700;
}
