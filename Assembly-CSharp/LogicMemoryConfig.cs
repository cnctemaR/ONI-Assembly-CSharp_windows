using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class LogicMemoryConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string id = LogicMemoryConfig.ID;
		int num = 2;
		int num2 = 2;
		string text = "logic_memory_kanim";
		int num3 = 10;
		float num4 = 30f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.Anywhere;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, num, num2, text, num3, num4, tier, refined_METALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.NONE, none, 0.2f);
		buildingDef.Deprecated = false;
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.ViewMode = OverlayModes.Logic.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.SceneLayer = Grid.SceneLayer.LogicGates;
		buildingDef.ObjectLayer = ObjectLayer.LogicGates;
		SoundEventVolumeCache.instance.AddVolume("logic_memory_kanim", "PowerMemory_on", NOISE_POLLUTION.NOISY.TIER3);
		SoundEventVolumeCache.instance.AddVolume("logic_memory_kanim", "PowerMemory_off", NOISE_POLLUTION.NOISY.TIER3);
		GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, LogicMemoryConfig.ID);
		return buildingDef;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, LogicMemoryConfig.INPUT_PORTS, LogicMemoryConfig.OUTPUT_PORTS);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, LogicMemoryConfig.INPUT_PORTS, LogicMemoryConfig.OUTPUT_PORTS);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		go.AddOrGet<LogicMemory>();
		GeneratedBuildings.RegisterLogicPorts(go, LogicMemoryConfig.INPUT_PORTS, LogicMemoryConfig.OUTPUT_PORTS);
	}

	public static string ID = "LogicMemory";

	private static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[]
	{
		new LogicPorts.Port(LogicMemory.SET_PORT_ID, new CellOffset(0, 0), global::STRINGS.BUILDINGS.PREFABS.LOGICMEMORY.SET_PORT, global::STRINGS.BUILDINGS.PREFABS.LOGICMEMORY.SET_PORT_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.LOGICMEMORY.SET_PORT_INACTIVE, true, LogicPortSpriteType.Input),
		new LogicPorts.Port(LogicMemory.RESET_PORT_ID, new CellOffset(1, 0), global::STRINGS.BUILDINGS.PREFABS.LOGICMEMORY.RESET_PORT, global::STRINGS.BUILDINGS.PREFABS.LOGICMEMORY.RESET_PORT_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.LOGICMEMORY.RESET_PORT_INACTIVE, true, LogicPortSpriteType.ResetUpdate)
	};

	private static readonly LogicPorts.Port[] OUTPUT_PORTS = new LogicPorts.Port[]
	{
		new LogicPorts.Port(LogicMemory.READ_PORT_ID, new CellOffset(0, 1), global::STRINGS.BUILDINGS.PREFABS.LOGICMEMORY.READ_PORT, global::STRINGS.BUILDINGS.PREFABS.LOGICMEMORY.READ_PORT_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.LOGICMEMORY.READ_PORT_INACTIVE, true, LogicPortSpriteType.Output)
	};
}
