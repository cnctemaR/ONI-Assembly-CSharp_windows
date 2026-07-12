using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class SaltPlantConfig : IEntityConfig
{
	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		string text = "SaltPlant";
		string text2 = global::STRINGS.CREATURES.SPECIES.SALTPLANT.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.SALTPLANT.DESC;
		float num = 2f;
		EffectorValues tier = DECOR.PENALTY.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, Assets.GetAnim("saltplant_kanim"), "idle_empty", Grid.SceneLayer.BuildingFront, 1, 2, tier, default(EffectorValues), SimHashes.Creature, new List<Tag> { GameTags.Hanging }, 258.15f);
		EntityTemplates.MakeHangingOffsets(gameObject, 1, 2);
		GameObject gameObject2 = gameObject;
		float num2 = 198.15f;
		float num3 = 248.15f;
		float num4 = 323.15f;
		float num5 = 393.15f;
		string text4 = SimHashes.Salt.ToString();
		string text5 = global::STRINGS.CREATURES.SPECIES.SALTPLANT.NAME;
		EntityTemplates.ExtendEntityToBasicPlant(gameObject2, num2, num3, num4, num5, new SimHashes[] { SimHashes.ChlorineGas }, true, 0f, 0.025f, text4, true, true, true, true, 2400f, 0f, 740f, "SaltPlantOriginal", text5);
		gameObject.AddOrGet<SaltPlant>();
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new PlantElementAbsorber.ConsumeInfo[]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = SimHashes.Sand.CreateTag(),
				massConsumptionRate = 0.011666667f
			}
		});
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
		gameObject.GetComponent<UprootedMonitor>().monitorCells = new CellOffset[]
		{
			new CellOffset(0, 1)
		};
		gameObject.AddOrGet<StandardCropPlant>();
		EntityTemplates.MakeHangingOffsets(EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(gameObject, SeedProducer.ProductionType.Harvest, "SaltPlantSeed", global::STRINGS.CREATURES.SPECIES.SEEDS.SALTPLANT.NAME, global::STRINGS.CREATURES.SPECIES.SEEDS.SALTPLANT.DESC, Assets.GetAnim("seed_saltplant_kanim"), "object", 1, new List<Tag> { GameTags.CropSeed }, SingleEntityReceptacle.ReceptacleDirection.Bottom, default(Tag), 5, global::STRINGS.CREATURES.SPECIES.SALTPLANT.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.35f, 0.35f, null, "", false), "SaltPlant_preview", Assets.GetAnim("saltplant_kanim"), "place", 1, 2), 1, 2);
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
