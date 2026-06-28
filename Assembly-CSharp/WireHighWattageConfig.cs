using System;
using TUNING;
using UnityEngine;

public class WireHighWattageConfig : BaseWireConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "HighWattageWire";
		string text2 = "utilities_electric_insulated_kanim";
		float num = 800f;
		float num2 = 3f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
		float num3 = 0.05f;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = base.CreateBuildingDef(text, text2, num, num2, tier, num3, BUILDINGS.DECOR.PENALTY.TIER5, none);
		buildingDef.BuildLocationRule = BuildLocationRule.NotInTiles;
		buildingDef.HotKey = global::Action.BuildMenuKeyT;
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		base.DoPostConfigureComplete(Wire.WattageRating.Max20000, go);
	}

	public const string ID = "HighWattageWire";
}
