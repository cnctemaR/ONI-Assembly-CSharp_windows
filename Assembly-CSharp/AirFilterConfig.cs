using System;
using TUNING;
using UnityEngine;

public class AirFilterConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "AirFilter";
		int num = 1;
		int num2 = 1;
		string text2 = "co2filter_kanim";
		float num3 = 200f;
		int num4 = 30;
		float num5 = 30f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
		string[] raw_MINERALS = MATERIALS.RAW_MINERALS;
		float num6 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER0;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, num5, tier, raw_MINERALS, num6, buildLocationRule, BUILDINGS.DECOR.NONE, tier2);
		buildingDef.Overheatable = false;
		buildingDef.ViewMode = SimViewMode.OxygenMap;
		buildingDef.MaterialCategory = MATERIALS.RAW_MINERALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<LoopingSounds>();
		Prioritizable.AddRef(go);
		Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
		storage.showInUI = true;
		storage.capacityKg = 200f;
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		ElementConsumer elementConsumer = go.AddOrGet<ElementConsumer>();
		elementConsumer.elementToConsume = SimHashes.ContaminatedOxygen;
		elementConsumer.consumptionRate = 0.1f;
		elementConsumer.consumptionRadius = 3;
		elementConsumer.showInStatusPanel = true;
		elementConsumer.sampleCellOffset = new Vector3(0f, 0f, 0f);
		elementConsumer.isRequired = false;
		elementConsumer.storeOnConsume = true;
		ElementDropper elementDropper = go.AddComponent<ElementDropper>();
		elementDropper.emitMass = 10f;
		elementDropper.emitTag = new Tag("Clay");
		elementDropper.emitOffset = new Vector3(0f, 1f, 0f);
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
		{
			new ElementConverter.ConsumedElement(new Tag("Filter"), 0.13333334f),
			new ElementConverter.ConsumedElement(new Tag("ContaminatedOxygen"), 0.1f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[]
		{
			new ElementConverter.OutputElement(0.14333335f, SimHashes.Clay, 0f, true, 0f, 0.5f, false, 0.25f, byte.MaxValue, 0),
			new ElementConverter.OutputElement(0.089999996f, SimHashes.Oxygen, 0f, false, 0f, 1f, false, 0.75f, byte.MaxValue, 0)
		};
		elementConverter.conversionInterval = 1f;
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = new Tag("Filter");
		manualDeliveryKG.capacity = 80.00001f;
		manualDeliveryKG.refillMass = 8.000001f;
		AirFilter airFilter = go.AddOrGet<AirFilter>();
		airFilter.filterTag = new Tag("Filter");
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			ActiveController.Instance instance = new ActiveController.Instance(game_object.GetComponent<KPrefabID>());
			instance.StartSM();
		};
	}

	public const string ID = "AirFilter";

	public const float DIRTY_AIR_CONSUMPTION_RATE = 0.1f;

	private const float SAND_CONSUMPTION_RATE = 0.13333334f;

	private const float REFILL_RATE = 600f;

	private const float SAND_STORAGE_AMOUNT = 80.00001f;

	private const float CLAY_PER_LOAD = 10f;
}
