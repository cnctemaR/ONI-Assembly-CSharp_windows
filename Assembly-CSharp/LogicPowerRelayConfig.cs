using System;
using OverlayModes;
using STRINGS;
using TUNING;
using UnityEngine;

public class LogicPowerRelayConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string id = LogicPowerRelayConfig.ID;
		int num = 1;
		int num2 = 1;
		string text = "switchpowershutoff_kanim";
		float num3 = 100f;
		int num4 = 10;
		float num5 = 30f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num6 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.Anywhere;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, num, num2, text, num3, num4, num5, tier, all_METALS, num6, buildLocationRule, global::TUNING.BUILDINGS.DECOR.NONE, none);
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.AudioCategory = "Metal";
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		SoundEventVolumeCache.instance.AddVolume("switchpower_kanim", "PowerSwitch_on", NOISE_POLLUTION.NOISY.TIER3);
		SoundEventVolumeCache.instance.AddVolume("switchpower_kanim", "PowerSwitch_off", NOISE_POLLUTION.NOISY.TIER3);
		GeneratedBuildings.RegisterWithOverlay(Logic.HighlightItemIDs, LogicPowerRelayConfig.ID);
		return buildingDef;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, LogicPowerRelayConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, LogicPowerRelayConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		global::UnityEngine.Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
		GeneratedBuildings.RegisterLogicPorts(go, LogicPowerRelayConfig.INPUT_PORTS);
		go.AddOrGet<LogicOperationalController>();
		OperationalControlledSwitch operationalControlledSwitch = go.AddOrGet<OperationalControlledSwitch>();
		operationalControlledSwitch.objectLayer = ObjectLayer.Wire;
	}

	public static string ID = "LogicPowerRelay";

	public static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[]
	{
		new LogicPorts.Port(LogicOperationalController.PORT_ID, new CellOffset(0, 0), global::STRINGS.BUILDINGS.PREFABS.LOGICPOWERRELAY.LOGIC_PORT_DESC, true)
	};
}
