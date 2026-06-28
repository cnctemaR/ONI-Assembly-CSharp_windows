using System;
using TUNING;
using UnityEngine;

public class BatteryMediumConfig : BaseBatteryConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "BatteryMedium";
		int num = 2;
		int num2 = 2;
		int num3 = 30;
		string text2 = "batterymed_kanim";
		float num4 = 1200f;
		float num5 = 60f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num6 = 800f;
		float num7 = 0.25f;
		float num8 = 1f;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER1;
		BuildingDef buildingDef = base.CreateBuildingDef(text, num, num2, num3, text2, num4, num5, tier, all_METALS, num6, num7, num8, BUILDINGS.DECOR.PENALTY.TIER2, tier2);
		buildingDef.HotKey = global::Action.BuildMenuKeyB;
		SoundEventVolumeCache.instance.AddVolume("batterymed_kanim", "Battery_med_rattle", NOISE_POLLUTION.NOISY.TIER2);
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Battery battery = go.AddOrGet<Battery>();
		battery.capacity = 40000f;
		base.DoPostConfigureComplete(go);
	}

	public const string ID = "BatteryMedium";
}
