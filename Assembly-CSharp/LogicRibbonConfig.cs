using System;
using TUNING;
using UnityEngine;

public class LogicRibbonConfig : BaseLogicWireConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "LogicRibbon";
		string text2 = "logic_ribbon_kanim";
		float num = 10f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
		EffectorValues none = NOISE_POLLUTION.NONE;
		return base.CreateBuildingDef(text, text2, num, tier, BUILDINGS.DECOR.PENALTY.TIER0, none);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		base.DoPostConfigureComplete(LogicWire.BitDepth.FourBit, go);
	}

	public const string ID = "LogicRibbon";
}
