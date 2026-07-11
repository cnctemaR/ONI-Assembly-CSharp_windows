using System;
using TUNING;
using UnityEngine;

public class LogicWireConfig : BaseLogicWireConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "LogicWire";
		string text2 = "logic_wires_kanim";
		float num = 3f;
		float[] tier_TINY = BUILDINGS.CONSTRUCTION_MASS_KG.TIER_TINY;
		EffectorValues none = NOISE_POLLUTION.NONE;
		return base.CreateBuildingDef(text, text2, num, tier_TINY, BUILDINGS.DECOR.PENALTY.TIER0, none);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		base.DoPostConfigureComplete(LogicWire.BitDepth.OneBit, go);
	}

	public const string ID = "LogicWire";
}
