using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class SaltPlantConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		string text = "SaltPlant";
		string text2 = global::STRINGS.CREATURES.SPECIES.SALTPLANT.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.SALTPLANT.DESC;
		float num = 2f;
		KAnimFile kanimFile = Assets.GetAnim("saltplant_kanim");
		string text4 = "idle_empty";
		EffectorValues tier = DECOR.PENALTY.TIER1;
		List<Tag> list = new List<Tag> { GameTags.Hanging };
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, kanimFile, text4, Grid.SceneLayer.BuildingFront, 1, 2, tier, default(EffectorValues), SimHashes.Creature, list, 258.15f);
		EntityTemplates.MakeHangingOffsets(gameObject, 1, 2);
		GameObject gameObject2 = gameObject;
		num = 198.15f;
		float num2 = 248.15f;
		float num3 = 323.15f;
		float num4 = 393.15f;
		text4 = SimHashes.Salt.ToString();
		EntityTemplates.ExtendEntityToBasicPlant(gameObject2, num, num2, num3, num4, null, true, 0f, 0.15f, text4, true, true, true, true, 2400f);
		gameObject.AddOrGet<SaltPlant>();
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new PlantElementAbsorber.ConsumeInfo[]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = SimHashes.Sand.CreateTag(),
				massConsumptionRate = 0.011666667f
			}
		});
		PressureVulnerable pressureVulnerable = gameObject.AddOrGet<PressureVulnerable>();
		PressureVulnerable pressureVulnerable2 = pressureVulnerable;
		num4 = 0.025f;
		num3 = 0f;
		SimHashes[] array = new SimHashes[] { SimHashes.ChlorineGas };
		pressureVulnerable2.Configure(num4, num3, 10f, 30f, array);
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.prefabInitFn += delegate(GameObject inst)
		{
			PressureVulnerable component3 = inst.GetComponent<PressureVulnerable>();
			component3.safe_atmospheres.Add(ElementLoader.FindElementByHash(SimHashes.ChlorineGas));
		};
		Storage storage = gameObject.AddOrGet<Storage>();
		storage.showInUI = false;
		storage.capacityKg = 1f;
		ElementConsumer elementConsumer = gameObject.AddOrGet<ElementConsumer>();
		elementConsumer.showInStatusPanel = true;
		elementConsumer.showDescriptor = true;
		elementConsumer.storeOnConsume = false;
		elementConsumer.elementToConsume = SimHashes.ChlorineGas;
		elementConsumer.configuration = ElementConsumer.Configuration.Element;
		elementConsumer.consumptionRadius = 4;
		elementConsumer.sampleCellOffset = new Vector3(0f, -1f);
		elementConsumer.consumptionRate = 0.006f;
		UprootedMonitor component2 = gameObject.GetComponent<UprootedMonitor>();
		component2.monitorCell = new CellOffset(0, 1);
		gameObject.AddOrGet<StandardCropPlant>();
		gameObject2 = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Harvest;
		text4 = "SaltPlantSeed";
		text3 = global::STRINGS.CREATURES.SPECIES.SEEDS.SALTPLANT.NAME;
		text2 = global::STRINGS.CREATURES.SPECIES.SEEDS.SALTPLANT.DESC;
		kanimFile = Assets.GetAnim("seed_saltplant_kanim");
		list = new List<Tag> { GameTags.CropSeed };
		GameObject gameObject3 = EntityTemplates.CreateAndRegisterSeedForPlant(gameObject2, productionType, text4, text3, text2, kanimFile, "object", 1, list, SingleEntityReceptacle.ReceptacleDirection.Bottom, default(Tag), 4, global::STRINGS.CREATURES.SPECIES.SALTPLANT.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.35f, 0.35f, null, string.Empty, false);
		GameObject gameObject4 = EntityTemplates.CreateAndRegisterPreviewForPlant(gameObject3, "SaltPlant_preview", Assets.GetAnim("saltplant_kanim"), "place", 1, 2);
		EntityTemplates.MakeHangingOffsets(gameObject4, 1, 2);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		inst.GetComponent<ElementConsumer>().EnableConsumption(true);
	}

	public const string ID = "SaltPlant";

	public const string SEED_ID = "SaltPlantSeed";

	public const float FERTILIZATION_RATE = 0.011666667f;

	public const float CHLORINE_CONSUMPTION_RATE = 0.006f;
}
