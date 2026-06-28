using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class AirConditionerConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "AirConditioner";
		int num = 2;
		int num2 = 2;
		string text2 = "airconditioner_kanim";
		float num3 = 200f;
		int num4 = 100;
		float num5 = 120f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num6 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER2;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, num5, tier, all_METALS, num6, buildLocationRule, global::TUNING.BUILDINGS.DECOR.NONE, tier2);
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

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, AirConditionerConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, AirConditionerConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		GeneratedBuildings.RegisterLogicPorts(go, AirConditionerConfig.INPUT_PORTS);
		go.AddOrGet<LogicOperationalController>();
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			PoweredActiveController.Instance instance = new PoweredActiveController.Instance(game_object.GetComponent<KPrefabID>());
			instance.StartSM();
		};
	}

	public const string ID = "AirConditioner";

	private static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[]
	{
		new LogicPorts.Port(LogicOperationalController.PORT_ID, new CellOffset(0, 1), UI.LOGIC_PORTS.CONTROL_OPERATIONAL, false)
	};
}
