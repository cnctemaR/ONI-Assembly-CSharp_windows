using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class LogicRibbonReaderConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string id = LogicRibbonReaderConfig.ID;
		int num = 2;
		int num2 = 1;
		string text = "logic_ribbon_reader_kanim";
		int num3 = 30;
		float num4 = 30f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.Anywhere;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, num, num2, text, num3, num4, tier, refined_METALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.PENALTY.TIER0, none, 0.2f);
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.InitialOrientation = Orientation.R180;
		buildingDef.ViewMode = OverlayModes.Logic.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.ObjectLayer = ObjectLayer.LogicGate;
		buildingDef.SceneLayer = Grid.SceneLayer.LogicGates;
		buildingDef.AlwaysOperational = true;
		buildingDef.LogicInputPorts = new List<LogicPorts.Port> { LogicPorts.Port.RibbonInputPort(LogicRibbonReader.INPUT_PORT_ID, new CellOffset(0, 0), global::STRINGS.BUILDINGS.PREFABS.LOGICRIBBONREADER.LOGIC_PORT, global::STRINGS.BUILDINGS.PREFABS.LOGICRIBBONREADER.INPUT_PORT_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.LOGICRIBBONREADER.INPUT_PORT_INACTIVE, true, false) };
		buildingDef.LogicOutputPorts = new List<LogicPorts.Port> { LogicPorts.Port.OutputPort(LogicRibbonReader.OUTPUT_PORT_ID, new CellOffset(1, 0), global::STRINGS.BUILDINGS.PREFABS.LOGICRIBBONREADER.LOGIC_PORT_OUTPUT, global::STRINGS.BUILDINGS.PREFABS.LOGICRIBBONREADER.OUTPUT_PORT_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.LOGICRIBBONREADER.OUTPUT_PORT_INACTIVE, true, false) };
		SoundEventVolumeCache.instance.AddVolume("door_internal_kanim", "Open_DoorInternal", NOISE_POLLUTION.NOISY.TIER3);
		SoundEventVolumeCache.instance.AddVolume("door_internal_kanim", "Close_DoorInternal", NOISE_POLLUTION.NOISY.TIER3);
		GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, LogicRibbonReaderConfig.ID);
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicRibbonReader>();
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits, false);
	}

	public static string ID = "LogicRibbonReader";
}
