using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class WireHighWattageConfig : BaseWireConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "HighWattageWire";
		string text2 = "utilities_electric_insulated_kanim";
		float num = 3f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
		float num2 = 0.05f;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = base.CreateBuildingDef(text, text2, num, tier, num2, global::TUNING.BUILDINGS.DECOR.PENALTY.TIER5, none);
		buildingDef.BuildLocationRule = BuildLocationRule.NotInTiles;
		buildingDef.AddSearchTerms(SEARCH_TERMS.POWER);
		buildingDef.AddSearchTerms(SEARCH_TERMS.WIRE);
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		base.DoPostConfigureComplete(Wire.WattageRating.Max20000, go);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
	}

	public const string ID = "HighWattageWire";
}
