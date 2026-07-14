using System;
using TUNING;
using UnityEngine;

public class WireRubberConfig : BaseWireConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "WireRubber";
		string text2 = "utilities_electric_rubber_kanim";
		float num = 3f;
		float[] array = new float[]
		{
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER0[0],
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER_SMALL[0]
		};
		float num2 = 0.05f;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = base.CreateBuildingDef(text, text2, num, array, num2, BUILDINGS.DECOR.NONE, none);
		buildingDef.MaterialCategory = new string[] { "RefinedMetal", "Rubber&Plastic" };
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		base.DoPostConfigureComplete(Wire.WattageRating.Max4000, go);
	}

	public const string ID = "WireRubber";
}
