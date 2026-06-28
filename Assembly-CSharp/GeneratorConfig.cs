using System;
using TUNING;
using UnityEngine;

public class GeneratorConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("Generator", 3, 3, "generatorphos_kanim", 400f, 100, 120f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER5, MATERIALS.ALL_METALS, 2400f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER2, null);
		buildingDef.GeneratorWattageRating = 600f;
		buildingDef.GeneratorBaseCapacity = 20000f;
		buildingDef.ExhaustKilowattsWhenActive = 8f;
		buildingDef.OperatingKilowatts = 1f;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.MaterialCategory = MATERIALS.ALL_METALS;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.AudioSize = "large";
		buildingDef.Upgradeable = false;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		EnergyGenerator energyGenerator = go.AddOrGet<EnergyGenerator>();
		energyGenerator.formula = EnergyGenerator.CreateSimpleFormula(SimHashes.Carbon, 1f, 500f, SimHashes.Void, 0f, true);
		energyGenerator.BatteryRefillPercent = 0.5f;
		energyGenerator.powerDistributionOrder = 9;
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 500f;
		storage.disableOnStore = true;
		go.AddOrGet<LoopingSounds>();
		go.AddOrGet<Prioritizable>();
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = new Tag("Coal");
		manualDeliveryKG.capacity = storage.capacityKg;
		manualDeliveryKG.refillMass = 100f;
		BuildingElementEmitter buildingElementEmitter = go.AddOrGet<BuildingElementEmitter>();
		buildingElementEmitter.emitRate = 0.02f;
		buildingElementEmitter.temperature = 310f;
		buildingElementEmitter.element = SimHashes.CarbonDioxide;
		buildingElementEmitter.modifierOffset = new Vector2(1f, 2f);
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
}
