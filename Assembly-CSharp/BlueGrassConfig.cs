using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class BlueGrassConfig : IEntityConfig
{
	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_DLC_2;
	}

	public GameObject CreatePrefab()
	{
		string text = "BlueGrass";
		string text2 = global::STRINGS.CREATURES.SPECIES.BLUE_GRASS.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.BLUE_GRASS.DESC;
		float num = 2f;
		EffectorValues tier = DECOR.BONUS.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("bluegrass_kanim"), "idle_full", Grid.SceneLayer.BuildingFront, 1, 2, tier, default(EffectorValues), SimHashes.Creature, null, 240f);
		GameObject gameObject2 = gameObject;
		float num2 = 193.15f;
		float num3 = 193.15f;
		float num4 = 273.15f;
		float num5 = 273.15f;
		string text4 = global::STRINGS.CREATURES.SPECIES.BLUE_GRASS.NAME;
		EntityTemplates.ExtendEntityToBasicPlant(gameObject2, num2, num3, num4, num5, new SimHashes[] { SimHashes.CarbonDioxide }, true, 0f, 0f, "OxyRock", true, true, true, true, 2400f, 0f, 2200f, "BlueGrassOriginal", text4);
		ElementConsumer elementConsumer = gameObject.AddOrGet<ElementConsumer>();
		elementConsumer.showInStatusPanel = true;
		elementConsumer.storeOnConsume = false;
		elementConsumer.elementToConsume = SimHashes.CarbonDioxide;
		elementConsumer.configuration = ElementConsumer.Configuration.Element;
		elementConsumer.consumptionRadius = 2;
		elementConsumer.EnableConsumption(true);
		elementConsumer.sampleCellOffset = new Vector3(0f, 0f);
		elementConsumer.consumptionRate = 0.0005f;
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new PlantElementAbsorber.ConsumeInfo[]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = SimHashes.Ice.CreateTag(),
				massConsumptionRate = 0.033333335f
			}
		});
		gameObject.GetComponent<UprootedMonitor>();
		gameObject.AddOrGet<StandardCropPlant>();
		gameObject.AddOrGet<BlueGrass>();
		EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Harvest, "BlueGrassSeed", global::STRINGS.CREATURES.SPECIES.SEEDS.BLUE_GRASS.NAME, global::STRINGS.CREATURES.SPECIES.SEEDS.BLUE_GRASS.DESC, Assets.GetAnim("seed_bluegrass_kanim"), "object", 1, new List<Tag> { GameTags.CropSeed }, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 4, global::STRINGS.CREATURES.SPECIES.BLUE_GRASS.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f, null, "", false, this.GetDlcIds()), "BlueGrass_preview", Assets.GetAnim("bluegrass_kanim"), "place", 1, 1);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "BlueGrass";

	public const string SEED_ID = "BlueGrassSeed";

	public const float CO2_RATE = 0.002f;

	public const float FERTILIZATION_RATE = 20f;
}
