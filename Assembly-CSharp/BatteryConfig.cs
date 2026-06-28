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
		float num4 = 30f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num5 = 800f;
		float num6 = 0.25f;
		float num7 = 1f;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = base.CreateBuildingDef(text, num, num2, num3, text2, num4, tier, all_METALS, num5, num6, num7, BUILDINGS.DECOR.PENALTY.TIER1, none);
		buildingDef.Breakable = true;
		SoundEventVolumeCache.instance.AddVolume("batterysm_kanim", "Battery_rattle", NOISE_POLLUTION.NOISY.TIER1);
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Battery battery = go.AddOrGet<Battery>();
		battery.capacity = 10000f;
		battery.joulesLostPerSecond = battery.capacity * 0.1f / 600f;
		base.DoPostConfigureComplete(go);
	}

	public const string ID = "Battery";
}
