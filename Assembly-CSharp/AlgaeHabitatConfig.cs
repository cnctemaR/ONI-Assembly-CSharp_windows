using System;
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
		storage.capacityKg = 396f;
		storage.showInUI = true;
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = new Tag("Algae");
		manualDeliveryKG.capacity = 36f;
		manualDeliveryKG.refillMass = 7.2f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;
		ManualDeliveryKG manualDeliveryKG2 = go.AddComponent<ManualDeliveryKG>();
		manualDeliveryKG2.SetStorage(storage);
		manualDeliveryKG2.requestedItemTag = new Tag("Water");
		manualDeliveryKG2.capacity = 360f;
		manualDeliveryKG2.refillMass = 72f;
		manualDeliveryKG2.allowPause = true;
		manualDeliveryKG2.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;
		AlgaeHabitat algaeHabitat = go.AddOrGet<AlgaeHabitat>();
		algaeHabitat.lightBonusMultiplier = 1.1f;
		algaeHabitat.pressureSampleOffset = new CellOffset(0, 1);
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
		{
			new ElementConverter.ConsumedElement(new Tag("Algae"), 0.030000001f),
			new ElementConverter.ConsumedElement(new Tag("Water"), 0.3f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[]
		{
			new ElementConverter.OutputElement(0.040000003f, SimHashes.Oxygen, 303.15f, false, 0f, 1f, false, 1f, byte.MaxValue, 0)
		};
		ElementConsumer elementConsumer = go.AddOrGet<ElementConsumer>();
		elementConsumer.elementToConsume = SimHashes.CarbonDioxide;
		elementConsumer.consumptionRate = 0.0003333333f;
		elementConsumer.consumptionRadius = 3;
		elementConsumer.showInStatusPanel = true;
		elementConsumer.sampleCellOffset = new Vector3(0f, 1f, 0f);
		elementConsumer.isRequired = false;
		ElementConsumer elementConsumer2 = go.AddComponent<PassiveElementConsumer>();
		elementConsumer2.elementToConsume = SimHashes.Water;
		elementConsumer2.consumptionRate = 1.25f;
		elementConsumer2.consumptionRadius = 1;
		elementConsumer2.showDescriptor = false;
		elementConsumer2.storeOnConsume = true;
		elementConsumer2.capacityKG = 360f;
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

	private const float ALGAE_CAPACITY = 36f;

	private const float WATER_CAPACITY = 360f;
}
