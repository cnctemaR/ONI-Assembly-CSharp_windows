using System;
using TUNING;
using UnityEngine;

public class GasConduitTemperatureSensorConfig : ConduitSensorConfig
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
		return base.CreateBuildingDef(GasConduitTemperatureSensorConfig.ID, "gas_temperature_sensor_kanim", BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, MATERIALS.REFINED_METALS);
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

	public static string ID = "GasConduitTemperatureSensor";
}
