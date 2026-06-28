using System;
using TUNING;

public class WireConfig : BaseWireConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		return base.CreateBuildingDef("Wire", "utilities_electric_kanim", 800f, 3f, BUILDINGS.CONSTRUCTION_MASS.TIER0, 0.05f, BUILDINGS.DECOR.PENALTY.TIER0, null);
	}
}
