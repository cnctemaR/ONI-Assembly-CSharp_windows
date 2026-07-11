using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class GasConduitElementSensorConfig : ConduitSensorConfig
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
		BuildingDef buildingDef = base.CreateBuildingDef(GasConduitElementSensorConfig.ID, "gas_element_sensor_kanim", global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, MATERIALS.REFINED_METALS, new List<LogicPorts.Port> { LogicPorts.Port.OutputPort(LogicSwitch.PORT_ID, new CellOffset(0, 0), global::STRINGS.BUILDINGS.PREFABS.GASCONDUITELEMENTSENSOR.LOGIC_PORT, global::STRINGS.BUILDINGS.PREFABS.GASCONDUITELEMENTSENSOR.LOGIC_PORT_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.GASCONDUITELEMENTSENSOR.LOGIC_PORT_INACTIVE, true, false) });
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.GasVentIDs, GasConduitElementSensorConfig.ID);
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		base.DoPostConfigureComplete(go);
		go.AddOrGet<Filterable>().filterElementState = Filterable.ElementState.Gas;
		ConduitElementSensor conduitElementSensor = go.AddOrGet<ConduitElementSensor>();
		conduitElementSensor.manuallyControlled = false;
		conduitElementSensor.conduitType = this.ConduitType;
		conduitElementSensor.defaultState = false;
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits, false);
	}

	public static string ID = "GasConduitElementSensor";
}
