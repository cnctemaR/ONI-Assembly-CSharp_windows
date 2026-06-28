using System;
using TUNING;
using UnityEngine;

public class AirConditionerConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("AirConditioner", 2, 2, "airconditioner_kanim", 200f, 100, 120f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.NONE, null);
		BuildingTemplates.CreateElectricalBuildingDef(buildingDef);
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.EnergyConsumptionWhenActive = 240f;
		buildingDef.OperatingKilowatts = 0f;
		buildingDef.InputConduitType = ConduitType.Gas;
		buildingDef.OutputConduitType = ConduitType.Gas;
		buildingDef.PowerInputOffset = new CellOffset(1, 0);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<LoopingSounds>();
		AirConditioner airConditioner = go.AddOrGet<AirConditioner>();
		airConditioner.temperatureDelta = -14f;
		airConditioner.maxEnvironmentDelta = -50f;
		Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
		storage.showInUI = true;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Gas;
		conduitConsumer.consumptionRate = 1f;
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
