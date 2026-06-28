using System;
using TUNING;
using UnityEngine;

public class WireConfig : BaseWireConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		EffectorValues none = NOISE_POLLUTION.NONE;
		return base.CreateBuildingDef("Wire", "utilities_electric_kanim", 800f, 3f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, 0.05f, BUILDINGS.DECOR.PENALTY.TIER0, none);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		base.DoPostConfigureComplete(Wire.WattageRating.Max1000, go);
	}

	public const string ID = "Wire";
}
