using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class RoboPilotCommandModuleConfig : IBuildingConfig
{
	public override string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC3;
	}

	public override string[] GetForbiddenDlcIds()
	{
		return DlcManager.EXPANSION1;
	}

	public override BuildingDef CreateBuildingDef()
	{
		string text = "RoboPilotCommandModule";
		int num = 5;
		int num2 = 5;
		string text2 = "robo_command_capsule_kanim";
		int num3 = 1000;
		float num4 = 60f;
		float[] command_MODULE_MASS = global::TUNING.BUILDINGS.ROCKETRY_MASS_KG.COMMAND_MODULE_MASS;
		string[] array = new string[] { SimHashes.Steel.ToString() };
		float num5 = 9999f;
		BuildLocationRule buildLocationRule = BuildLocationRule.BuildingAttachPoint;
		EffectorValues tier = NOISE_POLLUTION.NOISY.TIER2;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, command_MODULE_MASS, array, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.NONE, tier, 0.2f);
		BuildingTemplates.CreateRocketBuildingDef(buildingDef);
		buildingDef.SceneLayer = Grid.SceneLayer.BuildingFront;
		buildingDef.OverheatTemperature = 2273.15f;
		buildingDef.Floodable = false;
		buildingDef.AttachmentSlotTag = GameTags.Rocket;
		buildingDef.ObjectLayer = ObjectLayer.Building;
		buildingDef.RequiresPowerInput = false;
		buildingDef.attachablePosition = new CellOffset(0, 0);
		buildingDef.CanMove = true;
		buildingDef.LogicInputPorts = new List<LogicPorts.Port> { LogicPorts.Port.InputPort("TriggerLaunch", new CellOffset(0, 1), global::STRINGS.BUILDINGS.PREFABS.COMMANDMODULE.LOGIC_PORT_LAUNCH, global::STRINGS.BUILDINGS.PREFABS.COMMANDMODULE.LOGIC_PORT_LAUNCH_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.COMMANDMODULE.LOGIC_PORT_LAUNCH_INACTIVE, false, false) };
		buildingDef.LogicOutputPorts = new List<LogicPorts.Port> { LogicPorts.Port.OutputPort("LaunchReady", new CellOffset(0, 2), global::STRINGS.BUILDINGS.PREFABS.COMMANDMODULE.LOGIC_PORT_READY, global::STRINGS.BUILDINGS.PREFABS.COMMANDMODULE.LOGIC_PORT_READY_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.COMMANDMODULE.LOGIC_PORT_READY_INACTIVE, false, false) };
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
		LaunchConditionManager launchConditionManager = go.AddOrGet<LaunchConditionManager>();
		launchConditionManager.triggerPort = "TriggerLaunch";
		launchConditionManager.statusPort = "LaunchReady";
		RoboPilotModule roboPilotModule = go.AddOrGet<RoboPilotModule>();
		roboPilotModule.consumeDataBanksOnLand = true;
		roboPilotModule.dataBankConsumption = 1;
		Storage storage = go.AddComponent<Storage>();
		storage.showInUI = true;
		storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
		storage.capacityKg = 100f;
		ManualDeliveryKG manualDeliveryKG = go.AddComponent<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
		manualDeliveryKG.capacity = storage.capacityKg;
		manualDeliveryKG.refillMass = 20f;
		manualDeliveryKG.requestedItemTag = DatabankHelper.TAG;
		manualDeliveryKG.MinimumMass = 1f;
		go.AddOrGet<CommandModule>().robotPilotControlled = true;
		go.AddOrGet<RobotCommandConditions>();
		go.AddOrGet<LaunchableRocket>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.ExtendBuildingToRocketModule(go, "rocket_command_module_bg_kanim", false);
	}

	public const string ID = "RoboPilotCommandModule";

	public static float DATABANKCONSUMPTION = 2f;

	public static float DATABANKRANGE = 10000f / RoboPilotCommandModuleConfig.DATABANKCONSUMPTION;

	private const string TRIGGER_LAUNCH_PORT_ID = "TriggerLaunch";

	private const string LAUNCH_READY_PORT_ID = "LaunchReady";
}
