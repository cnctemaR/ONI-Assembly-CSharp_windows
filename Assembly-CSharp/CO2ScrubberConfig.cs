using System;
using TUNING;
using UnityEngine;

public class CO2ScrubberConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("CO2Scrubber", 2, 2, "co2scrubber_kanim", 200f, 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.RAW_METALS, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER1, null);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.OperatingKilowatts = 1f;
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.OutputConduitType = ConduitType.Liquid;
		buildingDef.ViewMode = SimViewMode.OxygenMap;
		buildingDef.MaterialCategory = MATERIALS.RAW_METALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "large";
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(1, 1);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<LoopingSounds>();
		Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
		storage.showInUI = true;
		go.AddOrGet<AirFilter>();
		ElementConsumer elementConsumer = go.AddOrGet<ElementConsumer>();
		elementConsumer.elementToConsume = SimHashes.CarbonDioxide;
		elementConsumer.consumptionRate = 0.3f;
		elementConsumer.consumptionRadius = 3;
		elementConsumer.showInStatusPanel = true;
		elementConsumer.sampleCellOffset = new Vector3(0f, 0f, 0f);
		elementConsumer.isRequired = false;
		elementConsumer.storeOnConsume = true;
		elementConsumer.capacityKG = 5f;
		elementConsumer.showDescriptor = false;
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
		{
			new ElementConverter.ConsumedElement(TagManager.Create(SimHashes.Water), 1f),
			new ElementConverter.ConsumedElement(TagManager.Create(SimHashes.CarbonDioxide), 0.3f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[]
		{
			new ElementConverter.OutputElement(1f, SimHashes.DirtyWater, 313.15f, true, 0f, 0f, false)
		};
		elementConverter.conversionInterval = 1f;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.consumptionRate = 2f;
		conduitConsumer.capacityKG = 20f;
		conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.Water).tag;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Store;
		ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
		conduitDispenser.conduitType = ConduitType.Liquid;
		conduitDispenser.invertElementFilter = true;
		conduitDispenser.elementFilter = new SimHashes[] { SimHashes.Water };
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			PoweredActiveController.Instance instance = new PoweredActiveController.Instance(game_object.GetComponent<KPrefabID>());
			instance.StartSM();
		};
	}

	private const float CO2_CONSUMPTION_RATE = 0.3f;
}
