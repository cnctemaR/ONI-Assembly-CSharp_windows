using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class LiquidConduitDiseaseSensorConfig : ConduitSensorConfig
{
	protected override ConduitType ConduitType
	{
		get
		{
			return ConduitType.Liquid;
		}
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = base.CreateBuildingDef(LiquidConduitDiseaseSensorConfig.ID, "liquid_germs_sensor_kanim", new float[]
		{
			global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0[0],
			global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER1[0]
		}, new string[] { "RefinedMetal", "Plastic" }, new List<LogicPorts.Port> { LogicPorts.Port.OutputPort(LogicSwitch.PORT_ID, new CellOffset(0, 0), global::STRINGS.BUILDINGS.PREFABS.LIQUIDCONDUITDISEASESENSOR.LOGIC_PORT, global::STRINGS.BUILDINGS.PREFABS.LIQUIDCONDUITDISEASESENSOR.LOGIC_PORT_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.LIQUIDCONDUITDISEASESENSOR.LOGIC_PORT_INACTIVE, true, false) });
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, LiquidConduitDiseaseSensorConfig.ID);
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		base.DoPostConfigureComplete(go);
		ConduitDiseaseSensor conduitDiseaseSensor = go.AddComponent<ConduitDiseaseSensor>();
		conduitDiseaseSensor.conduitType = this.ConduitType;
		conduitDiseaseSensor.Threshold = 0f;
		conduitDiseaseSensor.ActivateAboveThreshold = true;
		conduitDiseaseSensor.manuallyControlled = false;
		conduitDiseaseSensor.defaultState = false;
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits, false);
	}

	public static string ID = "LiquidConduitDiseaseSensor";
}
