using System;
using TUNING;
using UnityEngine;

public class WireConfig : BaseWireConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "Wire";
		string text2 = "utilities_electric_kanim";
		float num = 800f;
		float num2 = 3f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
		float num3 = 0.05f;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = base.CreateBuildingDef(text, text2, num, num2, tier, num3, BUILDINGS.DECOR.PENALTY.TIER0, none);
		buildingDef.HotKey = global::Action.BuildMenuKeyW;
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		base.DoPostConfigureComplete(Wire.WattageRating.Max1000, go);
	}

	public const string ID = "Wire";
}
