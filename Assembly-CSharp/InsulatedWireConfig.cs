using System;
using TUNING;

public class InsulatedWireConfig : BaseWireConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		return base.CreateBuildingDef("InsulatedWire", "utilities_electric_insulated_kanim", 800f, 120f, BUILDINGS.CONSTRUCTION_MASS.TIER3, 0.05f, BUILDINGS.DECOR.PENALTY.TIER0, null);
	}
}
