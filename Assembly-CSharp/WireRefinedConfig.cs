using System;
using TUNING;
using UnityEngine;

public class WireRefinedConfig : BaseWireConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "WireRefined";
		string text2 = "utilities_electric_conduct_kanim";
		float num = 3f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
		float num2 = 0.05f;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = base.CreateBuildingDef(text, text2, num, tier, num2, BUILDINGS.DECOR.NONE, none);
		buildingDef.MaterialCategory = MATERIALS.REFINED_METALS;
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		base.DoPostConfigureComplete(Wire.WattageRating.Max2000, go);
	}

	public const string ID = "WireRefined";
}
