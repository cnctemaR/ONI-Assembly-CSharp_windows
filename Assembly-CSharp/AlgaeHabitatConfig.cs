using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class AlgaeHabitatConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "AlgaeHabitat";
		int num = 1;
		int num2 = 2;
		string text2 = "algaefarm_kanim";
		int num3 = 30;
		float num4 = 30f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] farmable = MATERIALS.FARMABLE;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER0;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, farmable, num5, buildLocationRule, BUILDINGS.DECOR.PENALTY.TIER1, tier2, 0.2f);
		buildingDef.Floodable = false;
		buildingDef.ViewMode = SimViewMode.OxygenMap;
		buildingDef.MaterialCategory = MATERIALS.FARMABLE;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
		SoundEventVolumeCache.instance.AddVolume("algaefarm_kanim", "AlgaeHabitat_bubbles", NOISE_POLLUTION.NOISY.TIER0);
		SoundEventVolumeCache.instance.AddVolume("algaefarm_kanim", "AlgaeHabitat_algae_in", NOISE_POLLUTION.NOISY.TIER0);
		SoundEventVolumeCache.instance.AddVolume("algaefarm_kanim", "AlgaeHabitat_algae_out", NOISE_POLLUTION.NOISY.TIER0);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Storage storage = go.AddOrGet<Storage>();
		storage.showInUI = true;
		Storage storage2 = go.AddComponent<Storage>();
		storage2.capacityKg = 360f;
		storage2.showInUI = true;
		storage2.SetDefaultStoredItemModifiers(AlgaeHabitatConfig.PollutedWaterStorageModifiers);
		storage2.allowItemRemoval = false;
		storage2.storageFilters = AlgaeHabitatConfig.pollutedWaterFilter;
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = new Tag("Algae");
		manualDeliveryKG.capacity = 90f;
		manualDeliveryKG.refillMass = 18f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.OperateFetch.IdHash;
		ManualDeliveryKG manualDeliveryKG2 = go.AddComponent<ManualDeliveryKG>();
		manualDeliveryKG2.SetStorage(storage);
		manualDeliveryKG2.requestedItemTag = new Tag("Water");
		manualDeliveryKG2.capacity = 360f;
		manualDeliveryKG2.refillMass = 72f;
		manualDeliveryKG2.allowPause = true;
		manualDeliveryKG2.choreTypeIDHash = Db.Get().ChoreTypes.OperateFetch.IdHash;
		KAnimFile[] array = new KAnimFile[] { Assets.GetAnim("anim_interacts_outhouse_kanim") };
		AlgaeHabitatEmpty algaeHabitatEmpty = go.AddOrGet<AlgaeHabitatEmpty>();
		algaeHabitatEmpty.workTime = 5f;
		algaeHabitatEmpty.overrideAnims = array;
		algaeHabitatEmpty.workLayer = Grid.SceneLayer.BuildingFront;
		AlgaeHabitat algaeHabitat = go.AddOrGet<AlgaeHabitat>();
		algaeHabitat.lightBonusMultiplier = 1.1f;
		algaeHabitat.pressureSampleOffset = new CellOffset(0, 1);
		ElementConverter elementConverter = go.AddComponent<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
		{
			new ElementConverter.ConsumedElement(new Tag("Algae"), 0.030000001f),
			new ElementConverter.ConsumedElement(new Tag("Water"), 0.3f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[]
		{
			new ElementConverter.OutputElement(0.040000003f, SimHashes.Oxygen, 303.15f, false, 0f, 1f, false, 1f, byte.MaxValue, 0)
		};
		ElementConverter elementConverter2 = go.AddComponent<ElementConverter>();
		new ElementConverter.OutputElement(0.29033333f, SimHashes.DirtyWater, 303.15f, true, 0f, 1f, false, 1f, byte.MaxValue, 0);
		elementConverter2.outputElements = new ElementConverter.OutputElement[]
		{
			new ElementConverter.OutputElement(0.29033333f, SimHashes.DirtyWater, 303.15f, true, 0f, 1f, false, 1f, byte.MaxValue, 0)
		};
		ElementConsumer elementConsumer = go.AddOrGet<ElementConsumer>();
		elementConsumer.elementToConsume = SimHashes.CarbonDioxide;
		elementConsumer.consumptionRate = 0.0003333333f;
		elementConsumer.consumptionRadius = 3;
		elementConsumer.showInStatusPanel = true;
		elementConsumer.sampleCellOffset = new Vector3(0f, 1f, 0f);
		elementConsumer.isRequired = false;
		PassiveElementConsumer passiveElementConsumer = go.AddComponent<PassiveElementConsumer>();
		passiveElementConsumer.elementToConsume = SimHashes.Water;
		passiveElementConsumer.consumptionRate = 1.2f;
		passiveElementConsumer.consumptionRadius = 1;
		passiveElementConsumer.showDescriptor = false;
		passiveElementConsumer.storeOnConsume = true;
		passiveElementConsumer.capacityKG = 360f;
		passiveElementConsumer.showInStatusPanel = false;
		go.AddOrGet<AnimTileable>();
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}

	public const string ID = "AlgaeHabitat";

	private const float ALGAE_RATE = 0.030000001f;

	private const float WATER_RATE = 0.3f;

	private const float OXYGEN_RATE = 0.040000003f;

	private const float CO2_RATE = 0.0003333333f;

	private const float ALGAE_CAPACITY = 90f;

	private const float WATER_CAPACITY = 360f;

	private static readonly List<Storage.StoredItemModifier> PollutedWaterStorageModifiers = new List<Storage.StoredItemModifier> { Storage.StoredItemModifier.Seal };

	public static List<Tag> pollutedWaterFilter = new List<Tag> { ElementLoader.FindElementByHash(SimHashes.DirtyWater).tag };
}
