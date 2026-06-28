using System;
using TUNING;
using UnityEngine;

public class MineralDeoxidizerConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("MineralDeoxidizer", 1, 2, "mineraldeoxidizer_kanim", 100f, 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER1, null);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.ExhaustKilowattsWhenActive = 0.5f;
		buildingDef.OperatingKilowatts = 1f;
		buildingDef.ViewMode = SimViewMode.OxygenMap;
		buildingDef.MaterialCategory = MATERIALS.ALL_METALS;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.Breakable = true;
		buildingDef.Upgradeable = false;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<Prioritizable>();
		Electrolyzer electrolyzer = go.AddOrGet<Electrolyzer>();
		electrolyzer.maxMass = 1.8f;
		electrolyzer.hasMeter = false;
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 1000f;
		storage.disableOnStore = true;
		storage.showInUI = true;
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
		{
			new ElementConverter.ConsumedElement(new Tag("Algae"), 0.55f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[]
		{
			new ElementConverter.OutputElement(0.5f, SimHashes.Oxygen, 303.15f, false, 0f, 1f, false)
		};
		elementConverter.conversionInterval = 1f;
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = new Tag("Algae");
		manualDeliveryKG.capacity = 100f;
		manualDeliveryKG.refillMass = 25f;
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

	private const float OXYGEN_GENERATION_RATE = 0.5f;

	private const float OXYGEN_TEMPERATURE = 303.15f;
}
