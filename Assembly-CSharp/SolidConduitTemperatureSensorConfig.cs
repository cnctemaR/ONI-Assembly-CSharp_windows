using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class SolidConduitTemperatureSensorConfig : ConduitSensorConfig
{
	protected override ConduitType ConduitType
	{
		get
		{
			return ConduitType.Solid;
		}
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = base.CreateBuildingDef(SolidConduitTemperatureSensorConfig.ID, "conveyor_temperature_sensor_kanim", global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, MATERIALS.REFINED_METALS, new List<LogicPorts.Port> { LogicPorts.Port.OutputPort(LogicSwitch.PORT_ID, new CellOffset(0, 0), global::STRINGS.BUILDINGS.PREFABS.SOLIDCONDUITTEMPERATURESENSOR.LOGIC_PORT, global::STRINGS.BUILDINGS.PREFABS.SOLIDCONDUITTEMPERATURESENSOR.LOGIC_PORT_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.SOLIDCONDUITTEMPERATURESENSOR.LOGIC_PORT_INACTIVE, true, false) });
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, SolidConduitTemperatureSensorConfig.ID);
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		base.DoPostConfigureComplete(go);
		ConduitTemperatureSensor conduitTemperatureSensor = go.AddComponent<ConduitTemperatureSensor>();
		conduitTemperatureSensor.conduitType = this.ConduitType;
		conduitTemperatureSensor.Threshold = 280f;
		conduitTemperatureSensor.ActivateAboveThreshold = true;
		conduitTemperatureSensor.manuallyControlled = false;
		conduitTemperatureSensor.rangeMin = 0f;
		conduitTemperatureSensor.rangeMax = 9999f;
		conduitTemperatureSensor.defaultState = false;
	}

	public static string ID = "SolidConduitTemperatureSensor";
}
