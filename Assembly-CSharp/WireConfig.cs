using System;
using TUNING;
using UnityEngine;

public class WireConfig : BaseWireConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "Wire";
		string text2 = "utilities_electric_kanim";
		float num = 3f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
		float num2 = 0.05f;
		EffectorValues none = NOISE_POLLUTION.NONE;
		return base.CreateBuildingDef(text, text2, num, tier, num2, BUILDINGS.DECOR.PENALTY.TIER0, none);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		base.DoPostConfigureComplete(Wire.WattageRating.Max1000, go);
	}

	public const string ID = "Wire";
}
