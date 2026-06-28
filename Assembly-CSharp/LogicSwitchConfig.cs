using System;
using OverlayModes;
using STRINGS;
using TUNING;
using UnityEngine;

public class LogicSwitchConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string id = LogicSwitchConfig.ID;
		int num = 1;
		int num2 = 1;
		string text = "switchdupecontrolledpower_kanim";
		float num3 = 100f;
		int num4 = 10;
		float num5 = 30f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		float num6 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.Anywhere;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, num, num2, text, num3, num4, num5, tier, refined_METALS, num6, buildLocationRule, global::TUNING.BUILDINGS.DECOR.NONE, none);
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.ViewMode = SimViewMode.Logic;
		buildingDef.AudioCategory = "Metal";
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		SoundEventVolumeCache.instance.AddVolume("switchpower_kanim", "PowerSwitch_on", NOISE_POLLUTION.NOISY.TIER3);
		SoundEventVolumeCache.instance.AddVolume("switchpower_kanim", "PowerSwitch_off", NOISE_POLLUTION.NOISY.TIER3);
		GeneratedBuildings.RegisterWithOverlay(Logic.HighlightItemIDs, LogicSwitchConfig.ID);
		return buildingDef;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, LogicSwitchConfig.OUTPUT_PORT);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, LogicSwitchConfig.OUTPUT_PORT);
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, LogicSwitchConfig.OUTPUT_PORT);
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		go.AddOrGet<LogicSwitch>();
		BuildingTemplates.DoPostConfigure(go);
	}

	public static string ID = "LogicSwitch";

	public static readonly LogicPorts.Port OUTPUT_PORT = new LogicPorts.Port(LogicSwitch.PORT_ID, new CellOffset(0, 0), global::STRINGS.BUILDINGS.PREFABS.LOGICSWITCH.LOGIC_PORT_DESC, true);
}
