using System;
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
		return base.CreateBuildingDef(GasConduitElementSensorConfig.ID, "gas_element_sensor_kanim", BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, MATERIALS.REFINED_METALS);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		base.DoPostConfigureComplete(go);
		Filterable filterable = go.AddOrGet<Filterable>();
		filterable.filterElementState = Filterable.ElementState.Gas;
		ConduitElementSensor conduitElementSensor = go.AddOrGet<ConduitElementSensor>();
		conduitElementSensor.manuallyControlled = false;
		conduitElementSensor.conduitType = this.ConduitType;
		conduitElementSensor.defaultState = false;
	}

	public static string ID = "GasConduitElementSensor";
}
