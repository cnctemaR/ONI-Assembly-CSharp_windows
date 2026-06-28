using System;
using TUNING;
using UnityEngine;

public class BatteryMediumConfig : BaseBatteryConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		EffectorValues tier = NOISE_POLLUTION.NOISY.TIER1;
		BuildingDef buildingDef = base.CreateBuildingDef("BatteryMedium", 2, 2, 30, "batterymed_kanim", 1200f, 60f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 800f, 0.25f, 1f, BUILDINGS.DECOR.PENALTY.TIER2, tier);
		SoundEventVolumeCache.instance.AddVolume("batterymed_kanim", "Battery_med_rattle", NOISE_POLLUTION.NOISY.TIER2);
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Battery battery = go.AddOrGet<Battery>();
		battery.capacity = 40000f;
		base.DoPostConfigureComplete(go);
	}
}
