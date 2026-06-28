using System;
using TUNING;
using UnityEngine;

public class WireRefinedHighWattageConfig : BaseWireConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "WireRefinedHighWattage";
		string text2 = "utilities_electric_conduct_hiwatt_kanim";
		float num = 800f;
		float num2 = 3f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
		float num3 = 0.05f;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = base.CreateBuildingDef(text, text2, num, num2, tier, num3, BUILDINGS.DECOR.PENALTY.TIER3, none);
		buildingDef.MaterialCategory = MATERIALS.REFINED_METALS;
		buildingDef.BuildLocationRule = BuildLocationRule.NotInTiles;
		buildingDef.HotKey = global::Action.BuildMenuKeyV;
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		base.DoPostConfigureComplete(Wire.WattageRating.Max20000, go);
	}

	public const string ID = "WireRefinedHighWattage";
}
