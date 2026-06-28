using System;
using TUNING;
using UnityEngine;

public class BatteryConfig : BaseBatteryConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "Battery";
		int num = 1;
		int num2 = 2;
		int num3 = 30;
		string text2 = "batterysm_kanim";
		float num4 = 200f;
		float num5 = 30f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num6 = 800f;
		float num7 = 0.25f;
		float num8 = 1f;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = base.CreateBuildingDef(text, num, num2, num3, text2, num4, num5, tier, all_METALS, num6, num7, num8, BUILDINGS.DECOR.PENALTY.TIER1, none);
		buildingDef.Breakable = true;
		buildingDef.HotKey = global::Action.BuildMenuKeyT;
		SoundEventVolumeCache.instance.AddVolume("batterysm_kanim", "Battery_rattle", NOISE_POLLUTION.NOISY.TIER1);
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Battery battery = go.AddOrGet<Battery>();
		battery.capacity = 10000f;
		base.DoPostConfigureComplete(go);
	}

	public const string ID = "Battery";
}
