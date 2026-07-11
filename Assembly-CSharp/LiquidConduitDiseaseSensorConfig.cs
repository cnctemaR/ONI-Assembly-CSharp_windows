using System;
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
		return base.CreateBuildingDef(LiquidConduitDiseaseSensorConfig.ID, "liquid_germs_sensor_kanim", new float[]
		{
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER0[0],
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER1[0]
		}, new string[] { "RefinedMetal", "Plastic" });
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
	}

	public static string ID = "LiquidConduitDiseaseSensor";
}
