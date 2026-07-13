using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class FilterPlantConfig : IEntityConfig, IHasDlcRestrictions
{
	public string[] GetRequiredDlcIds()
	{
		return DlcManager.EXPANSION1;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public GameObject CreatePrefab()
	{
		string text = "FilterPlant";
		string text2 = global::STRINGS.CREATURES.SPECIES.FILTERPLANT.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.FILTERPLANT.DESC;
		float num = 2f;
		EffectorValues tier = DECOR.PENALTY.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("cactus_kanim"), "idle_empty", Grid.SceneLayer.BuildingFront, 1, 2, tier, default(EffectorValues), SimHashes.Creature, null, 348.15f);
		GameObject gameObject2 = gameObject;
		float num2 = 253.15f;
		float num3 = 293.15f;
		float num4 = 383.15f;
		float num5 = 443.15f;
		string text4 = SimHashes.Water.ToString();
		string text5 = global::STRINGS.CREATURES.SPECIES.FILTERPLANT.NAME;
		EntityTemplates.ExtendEntityToBasicPlant(gameObject2, num2, num3, num4, num5, new SimHashes[] { SimHashes.Oxygen }, true, 0f, 0.025f, text4, true, true, true, true, 2400f, 0f, 2200f, "FilterPlantOriginal", text5);
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new PlantElementAbsorber.ConsumeInfo[]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = SimHashes.Sand.CreateTag(),
				massConsumptionRate = 0.008333334f
			}
		});
		EntityTemplates.ExtendPlantToIrrigated(gameObject, new PlantElementAbsorber.ConsumeInfo[]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = GameTags.DirtyWater,
				massConsumptionRate = 0.108333334f
			}
		});
		gameObject.AddOrGet<StandardCropPlant>();
		gameObject.AddOrGet<SaltPlant>();
		ElementConsumer elementConsumer = gameObject.AddOrGet<ElementConsumer>();
		elementConsumer.showInStatusPanel = true;
		elementConsumer.showDescriptor = true;
		elementConsumer.storeOnConsume = false;
		elementConsumer.elementToConsume = SimHashes.Oxygen;
		elementConsumer.configuration = ElementConsumer.Configuration.Element;
		elementConsumer.consumptionRadius = 4;
		elementConsumer.sampleCellOffset = new Vector3(0f, 0f);
		elementConsumer.consumptionRate = 0.008333334f;
		GameObject gameObject3 = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Harvest;
		string text6 = "FilterPlantSeed";
		string text7 = global::STRINGS.CREATURES.SPECIES.SEEDS.FILTERPLANT.NAME;
		string text8 = global::STRINGS.CREATURES.SPECIES.SEEDS.FILTERPLANT.DESC;
		KAnimFile anim = Assets.GetAnim("seed_cactus_kanim");
		string text9 = "object";
		int num6 = 1;
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.CropSeed);
		SingleEntityReceptacle.ReceptacleDirection receptacleDirection = SingleEntityReceptacle.ReceptacleDirection.Top;
		text5 = global::STRINGS.CREATURES.SPECIES.FILTERPLANT.DOMESTICATEDDESC;
		EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(gameObject3, this, productionType, text6, text7, text8, anim, text9, num6, list, receptacleDirection, default(Tag), 21, text5, EntityTemplates.CollisionShape.CIRCLE, 0.35f, 0.35f, null, "", false), "FilterPlant_preview", Assets.GetAnim("cactus_kanim"), "place", 1, 2);
		gameObject.AddTag(GameTags.DeprecatedContent);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "FilterPlant";

	public const string SEED_ID = "FilterPlantSeed";

	public const float SAND_CONSUMPTION_RATE = 0.008333334f;

	public const float WATER_CONSUMPTION_RATE = 0.108333334f;

	public const float OXYGEN_CONSUMPTION_RATE = 0.008333334f;
}
