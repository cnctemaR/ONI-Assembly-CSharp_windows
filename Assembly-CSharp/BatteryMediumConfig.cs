using System;
using TUNING;

public class BatteryMediumConfig : BaseBatteryConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		return base.CreateBuildingDef("BatteryMedium", 2, 2, "batterymed_kanim", 1200f, 60f, BUILDINGS.CONSTRUCTION_MASS.TIER4, MATERIALS.ALL_METALS, 800f, 200f, 40000f, 4f, 400f, BUILDINGS.DECOR.PENALTY.TIER2);
	}
}
