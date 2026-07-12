using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class LogicDuplicantSensorConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("LogicDuplicantSensor", 1, 1, "presence_sensor_kanim", 30, 30f, global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.OnFoundationRotatable, global::TUNING.BUILDINGS.DECOR.PENALTY.TIER0, NOISE_POLLUTION.NOISY.TIER0, 0.2f);
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.Entombable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.ViewMode = OverlayModes.Logic.ID;
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.AlwaysOperational = true;
		buildingDef.LogicOutputPorts = new List<LogicPorts.Port> { LogicPorts.Port.OutputPort(LogicSwitch.PORT_ID, new CellOffset(0, 0), global::STRINGS.BUILDINGS.PREFABS.LOGICDUPLICANTSENSOR.LOGIC_PORT, global::STRINGS.BUILDINGS.PREFABS.LOGICDUPLICANTSENSOR.LOGIC_PORT_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.LOGICDUPLICANTSENSOR.LOGIC_PORT_INACTIVE, true, false) };
		GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, "LogicDuplicantSensor");
		return buildingDef;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		LogicDuplicantSensorConfig.AddVisualizer(go, true);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		LogicDuplicantSensor logicDuplicantSensor = go.AddOrGet<LogicDuplicantSensor>();
		logicDuplicantSensor.defaultState = false;
		logicDuplicantSensor.manuallyControlled = false;
		logicDuplicantSensor.pickupRange = 4;
		LogicDuplicantSensorConfig.AddVisualizer(go, false);
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits, false);
	}

	private static void AddVisualizer(GameObject prefab, bool movable)
	{
		RangeVisualizer rangeVisualizer = prefab.AddOrGet<RangeVisualizer>();
		rangeVisualizer.OriginOffset = new Vector2I(0, 0);
		rangeVisualizer.RangeMin.x = -2;
		rangeVisualizer.RangeMin.y = 0;
		rangeVisualizer.RangeMax.x = 2;
		rangeVisualizer.RangeMax.y = 4;
		rangeVisualizer.BlockingTileVisible = true;
	}

	public const string ID = "LogicDuplicantSensor";

	private const int RANGE = 4;
}
