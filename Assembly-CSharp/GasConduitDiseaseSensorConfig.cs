using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class GasConduitDiseaseSensorConfig : ConduitSensorConfig
{
	protected override ConduitType ConduitType
	{
		get
		{
			return ConduitType.Gas;
		}
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = base.CreateBuildingDef(GasConduitDiseaseSensorConfig.ID, "gas_germs_sensor_kanim", new float[]
		{
			global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0[0],
			global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER1[0]
		}, new string[] { "RefinedMetal", "Plastic" }, new List<LogicPorts.Port> { LogicPorts.Port.OutputPort(LogicSwitch.PORT_ID, new CellOffset(0, 0), global::STRINGS.BUILDINGS.PREFABS.GASCONDUITDISEASESENSOR.LOGIC_PORT, global::STRINGS.BUILDINGS.PREFABS.GASCONDUITDISEASESENSOR.LOGIC_PORT_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.GASCONDUITDISEASESENSOR.LOGIC_PORT_INACTIVE, true, false) });
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, GasConduitDiseaseSensorConfig.ID);
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

	public static string ID = "GasConduitDiseaseSensor";
}
