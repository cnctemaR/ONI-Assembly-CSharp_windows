using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class BeanPlantConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "BeanPlant";
		string text2 = global::STRINGS.CREATURES.SPECIES.BEAN_PLANT.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.BEAN_PLANT.DESC;
		float num = 2f;
		KAnimFile kanimFile = Assets.GetAnim("beanplant_kanim");
		string text4 = "idle_empty";
		EffectorValues tier = DECOR.BONUS.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, kanimFile, text4, Grid.SceneLayer.BuildingFront, 1, 2, tier, default(EffectorValues), SimHashes.Creature, null, 258.15f);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 198.15f, 248.15f, 273.15f, 323.15f, null, true, 0f, 0.15f, "BeanPlantSeed", true, true, true, true, 2400f);
		EntityTemplates.ExtendPlantToIrrigated(gameObject, new PlantElementAbsorber.ConsumeInfo[]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = SimHashes.Ethanol.CreateTag(),
				massConsumptionRate = 0.033333335f
			}
		});
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new PlantElementAbsorber.ConsumeInfo[]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = SimHashes.Dirt.CreateTag(),
				massConsumptionRate = 0.008333334f
			}
		});
		PressureVulnerable pressureVulnerable = gameObject.AddOrGet<PressureVulnerable>();
		PressureVulnerable pressureVulnerable2 = pressureVulnerable;
		num = 0.025f;
		float num2 = 0f;
		SimHashes[] array = new SimHashes[] { SimHashes.CarbonDioxide };
		pressureVulnerable2.Configure(num, num2, 10f, 30f, array);
		UprootedMonitor component = gameObject.GetComponent<UprootedMonitor>();
		component.monitorCell = new CellOffset(0, -1);
		gameObject.AddOrGet<StandardCropPlant>();
		GameObject gameObject2 = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Harvest;
		text4 = "BeanPlantSeed";
		text3 = global::STRINGS.CREATURES.SPECIES.SEEDS.BEAN_PLANT.NAME;
		text2 = global::STRINGS.CREATURES.SPECIES.SEEDS.BEAN_PLANT.DESC;
		kanimFile = Assets.GetAnim("seed_beanplant_kanim");
		List<Tag> list = new List<Tag> { GameTags.CropSeed };
		GameObject gameObject3 = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject2, productionType, text4, text3, text2, kanimFile, "object", 1, list, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 4, global::STRINGS.CREATURES.SPECIES.BEAN_PLANT.DOMESTICATEDDESC, EntityTemplates.CollisionShape.RECTANGLE, 0.6f, 0.3f, null, string.Empty, false);
		EntityTemplates.ExtendEntityToFood(gameObject3, FOOD.FOOD_TYPES.BEAN);
		EntityTemplates.CreateAndRegisterPreviewForPlant(gameObject3, "BeanPlant_preview", Assets.GetAnim("beanplant_kanim"), "place", 1, 3);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "BeanPlant";

	public const string SEED_ID = "BeanPlantSeed";

	public const float FERTILIZATION_RATE = 0.008333334f;

	public const float WATER_RATE = 0.033333335f;
}
