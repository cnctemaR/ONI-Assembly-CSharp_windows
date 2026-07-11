using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class SolidConduitElementSensorConfig : ConduitSensorConfig
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
		BuildingDef buildingDef = base.CreateBuildingDef(SolidConduitElementSensorConfig.ID, "conveyor_element_sensor_kanim", global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, MATERIALS.REFINED_METALS, new List<LogicPorts.Port> { LogicPorts.Port.OutputPort(LogicSwitch.PORT_ID, new CellOffset(0, 0), global::STRINGS.BUILDINGS.PREFABS.SOLIDCONDUITELEMENTSENSOR.LOGIC_PORT, global::STRINGS.BUILDINGS.PREFABS.SOLIDCONDUITELEMENTSENSOR.LOGIC_PORT_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.SOLIDCONDUITELEMENTSENSOR.LOGIC_PORT_INACTIVE, true, false) });
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, SolidConduitElementSensorConfig.ID);
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		base.DoPostConfigureComplete(go);
		go.AddOrGet<Filterable>().filterElementState = Filterable.ElementState.Solid;
		ConduitElementSensor conduitElementSensor = go.AddOrGet<ConduitElementSensor>();
		conduitElementSensor.manuallyControlled = false;
		conduitElementSensor.conduitType = this.ConduitType;
		conduitElementSensor.defaultState = false;
	}

	public static string ID = "SolidConduitElementSensor";
}
