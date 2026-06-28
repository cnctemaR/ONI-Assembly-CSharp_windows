using System;
using TUNING;
using UnityEngine;

public class BatteryConfig : BaseBatteryConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = base.CreateBuildingDef("Battery", 1, 2, 30, "batterysm_kanim", 200f, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 800f, 0.25f, 1f, BUILDINGS.DECOR.PENALTY.TIER1);
		buildingDef.Breakable = true;
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Battery battery = go.AddOrGet<Battery>();
		battery.capacity = 10000f;
		base.DoPostConfigureComplete(go);
	}
}
