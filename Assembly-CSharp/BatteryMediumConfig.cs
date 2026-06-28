using System;
using TUNING;
using UnityEngine;

public class BatteryMediumConfig : BaseBatteryConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		return base.CreateBuildingDef("BatteryMedium", 2, 2, 30, "batterymed_kanim", 1200f, 60f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 800f, 0.25f, 1f, BUILDINGS.DECOR.PENALTY.TIER2);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Battery battery = go.AddOrGet<Battery>();
		battery.capacity = 40000f;
		base.DoPostConfigureComplete(go);
	}
}
