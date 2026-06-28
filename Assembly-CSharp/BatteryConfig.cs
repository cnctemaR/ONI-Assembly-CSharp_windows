using System;
using TUNING;

public class BatteryConfig : BaseBatteryConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = base.CreateBuildingDef("Battery", 1, 2, "batterysm_kanim", 200f, 30f, BUILDINGS.CONSTRUCTION_MASS.TIER3, MATERIALS.ALL_METALS, 800f, 100f, 10000f, 2f, 350f, BUILDINGS.DECOR.PENALTY.TIER1);
		buildingDef.Breakable = true;
		return buildingDef;
	}
}
