using System;
using TUNING;
using UnityEngine;

public class WireHighWattageConfig : BaseWireConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = base.CreateBuildingDef("HighWattageWire", "utilities_electric_insulated_kanim", 800f, 3f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, 0.05f, BUILDINGS.DECOR.PENALTY.TIER5, null);
		buildingDef.BuildLocationRule = BuildLocationRule.NotInTiles;
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		base.DoPostConfigureComplete(Wire.WattageRating.Max20000, go);
	}

	public const string ID = "HighWattageWire";
}
