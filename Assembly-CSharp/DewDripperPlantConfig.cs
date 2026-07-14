using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class DewDripperPlantConfig : IEntityConfig, IHasDlcRestrictions
{
	public string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC4;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public GameObject CreatePrefab()
	{
		string text = "DewDripperPlant";
		string text2 = global::STRINGS.CREATURES.SPECIES.DEWDRIPPERPLANT.NAME;
		string text3 = global::STRINGS.CREATURES.SPECIES.DEWDRIPPERPLANT.DESC;
		float num = 1f;
		EffectorValues tier = DECOR.BONUS.TIER0;
		KAnimFile anim = Assets.GetAnim("brackwood_kanim");
		string text4 = "idle_empty";
		Grid.SceneLayer sceneLayer = Grid.SceneLayer.BuildingFront;
		int num2 = 1;
		int num3 = 2;
		EffectorValues effectorValues = tier;
		List<Tag> list = new List<Tag> { GameTags.Hanging };
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(text, text2, text3, num, anim, text4, sceneLayer, num2, num3, effectorValues, default(EffectorValues), SimHashes.Creature, list, 253.15f);
		EntityTemplates.MakeHangingOffsets(gameObject, 1, 2);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 218.15f, 238.15f, 278.15f, 308.15f, null, true, 0f, 0.25f, DewDripConfig.ID, true, false, true, false, true, 2400f, 0f, 4600f, "DewDripperPlantOriginal", global::STRINGS.CREATURES.SPECIES.DEWDRIPPERPLANT.NAME);
		PressureVulnerable pressureVulnerable = gameObject.AddOrGet<PressureVulnerable>();
		pressureVulnerable.pressureWarning_High = 2f;
		pressureVulnerable.pressureLethal_High = 10f;
		gameObject.GetComponent<UprootedMonitor>().monitorCells = new CellOffset[]
		{
			new CellOffset(0, 1)
		};
		gameObject.AddOrGet<PlantFiberProducer>().amount = 2f;
		gameObject.AddOrGet<StandardCropPlant>();
		gameObject.AddOrGet<LoopingSounds>();
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new PlantElementAbsorber.ConsumeInfo[]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = SimHashes.BrineIce.CreateTag(),
				massConsumptionRate = 0.016666668f
			}
		});
		GameObject gameObject2 = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Harvest;
		string text5 = "DewDripperPlantSeed";
		string text6 = global::STRINGS.CREATURES.SPECIES.SEEDS.DEWDRIPPERPLANT.NAME;
		string text7 = global::STRINGS.CREATURES.SPECIES.SEEDS.DEWDRIPPERPLANT.DESC;
		KAnimFile anim2 = Assets.GetAnim("seed_brackwood_kanim");
		string text8 = "object";
		int num4 = 1;
		List<Tag> list2 = new List<Tag>();
		list2.Add(GameTags.CropSeed);
		SingleEntityReceptacle.ReceptacleDirection receptacleDirection = SingleEntityReceptacle.ReceptacleDirection.Bottom;
		string text9 = global::STRINGS.CREATURES.SPECIES.DEWDRIPPERPLANT.DOMESTICATEDDESC;
		EntityTemplates.MakeHangingOffsets(EntityTemplates.CreateAndRegisterPreviewForPlant(EntityTemplates.CreateAndRegisterSeedForPlant(gameObject2, this, productionType, text5, text6, text7, anim2, text8, num4, list2, receptacleDirection, default(Tag), 5, text9, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f, null, "", false), "DewDripperPlant_preview", Assets.GetAnim("brackwood_kanim"), "place", 1, 2), 1, 2);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "DewDripperPlant";

	public const string SEED_ID = "DewDripperPlantSeed";

	public const float GROWTH_TIME = 1200f;

	public const float FERTILIZER_RATE = 0.016666668f;

	public const float PLANT_FIBER_PRODUCED_PER_CYCLE = 2f;
}
