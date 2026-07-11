using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class CommandModuleConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "CommandModule";
		int num = 5;
		int num2 = 5;
		string text2 = "rocket_command_module_kanim";
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
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		RocketModule rocketModule = go.AddOrGet<RocketModule>();
		rocketModule.SetBGKAnim(Assets.GetAnim("rocket_command_module_bg_kanim"));
		LaunchConditionManager launchConditionManager = go.AddOrGet<LaunchConditionManager>();
		launchConditionManager.triggerPort = "TriggerLaunch";
		launchConditionManager.statusPort = "LaunchReady";
		Storage storage = go.AddOrGet<Storage>();
		storage.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>
		{
			Storage.StoredItemModifier.Hide,
			Storage.StoredItemModifier.Seal,
			Storage.StoredItemModifier.Insulate
		});
		go.AddOrGet<CommandModule>();
		go.AddOrGet<CommandModuleWorkable>();
		go.AddOrGet<MinionStorage>();
		go.AddOrGet<ArtifactFinder>();
		go.AddOrGet<LaunchableRocket>();
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, CommandModuleConfig.INPUT_PORTS, CommandModuleConfig.OUTPUT_PORTS);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, CommandModuleConfig.INPUT_PORTS, CommandModuleConfig.OUTPUT_PORTS);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, CommandModuleConfig.INPUT_PORTS, CommandModuleConfig.OUTPUT_PORTS);
		Ownable ownable = go.AddOrGet<Ownable>();
		ownable.slotID = Db.Get().AssignableSlots.RocketCommandModule.Id;
		ownable.canBePublic = false;
		EntityTemplates.ExtendBuildingToRocketModule(go);
	}

	public const string ID = "CommandModule";

	private const string TRIGGER_LAUNCH_PORT_ID = "TriggerLaunch";

	private const string LAUNCH_READY_PORT_ID = "LaunchReady";

	private static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[] { LogicPorts.Port.InputPort("TriggerLaunch", new CellOffset(0, 1), global::STRINGS.BUILDINGS.PREFABS.COMMANDMODULE.LOGIC_PORT_LAUNCH, global::STRINGS.BUILDINGS.PREFABS.COMMANDMODULE.LOGIC_PORT_LAUNCH_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.COMMANDMODULE.LOGIC_PORT_LAUNCH_INACTIVE, false) };

	private static readonly LogicPorts.Port[] OUTPUT_PORTS = new LogicPorts.Port[] { LogicPorts.Port.OutputPort("LaunchReady", new CellOffset(0, 2), global::STRINGS.BUILDINGS.PREFABS.COMMANDMODULE.LOGIC_PORT_READY, global::STRINGS.BUILDINGS.PREFABS.COMMANDMODULE.LOGIC_PORT_READY_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.COMMANDMODULE.LOGIC_PORT_READY_INACTIVE, false) };
}
