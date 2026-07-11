using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class LogicWireBridgeConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "LogicWireBridge";
		int num = 3;
		int num2 = 1;
		string text2 = "logic_bridge_kanim";
		int num3 = 30;
		float num4 = 3f;
		float[] tier_TINY = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER_TINY;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.LogicBridge;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier_TINY, refined_METALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.PENALTY.TIER0, none, 0.2f);
		buildingDef.ViewMode = OverlayModes.Logic.ID;
		buildingDef.ObjectLayer = ObjectLayer.LogicGates;
		buildingDef.SceneLayer = Grid.SceneLayer.LogicGates;
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 2);
		GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, "LogicWireBridge");
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		base.DoPostConfigurePreview(def, go);
		this.AddNetworkLink(go).visualizeOnly = true;
		go.AddOrGet<BuildingCellVisualizer>();
		GeneratedBuildings.RegisterLogicPorts(go, LogicWireBridgeConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		this.AddNetworkLink(go).visualizeOnly = true;
		go.AddOrGet<BuildingCellVisualizer>();
		GeneratedBuildings.RegisterLogicPorts(go, LogicWireBridgeConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		this.AddNetworkLink(go).visualizeOnly = false;
		go.AddOrGet<BuildingCellVisualizer>();
		GeneratedBuildings.RegisterLogicPorts(go, LogicWireBridgeConfig.INPUT_PORTS);
	}

	private LogicUtilityNetworkLink AddNetworkLink(GameObject go)
	{
		LogicUtilityNetworkLink logicUtilityNetworkLink = go.AddOrGet<LogicUtilityNetworkLink>();
		logicUtilityNetworkLink.link1 = new CellOffset(-1, 0);
		logicUtilityNetworkLink.link2 = new CellOffset(1, 0);
		return logicUtilityNetworkLink;
	}

	public const string ID = "LogicWireBridge";

	public static readonly HashedString BRIDGE_LOGIC_IO_ID = new HashedString("BRIDGE_LOGIC_IO");

	public static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[]
	{
		LogicPorts.Port.InputPort(LogicWireBridgeConfig.BRIDGE_LOGIC_IO_ID, new CellOffset(-1, 0), global::STRINGS.BUILDINGS.PREFABS.LOGICWIREBRIDGE.LOGIC_PORT, global::STRINGS.BUILDINGS.PREFABS.LOGICWIREBRIDGE.LOGIC_PORT_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.LOGICWIREBRIDGE.LOGIC_PORT_INACTIVE, false, false),
		LogicPorts.Port.InputPort(LogicWireBridgeConfig.BRIDGE_LOGIC_IO_ID, new CellOffset(1, 0), global::STRINGS.BUILDINGS.PREFABS.LOGICWIREBRIDGE.LOGIC_PORT, global::STRINGS.BUILDINGS.PREFABS.LOGICWIREBRIDGE.LOGIC_PORT_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.LOGICWIREBRIDGE.LOGIC_PORT_INACTIVE, false, false)
	};
}
