using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class BatterySmartConfig : BaseBatteryConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "BatterySmart";
		int num = 2;
		int num2 = 2;
		int num3 = 30;
		string text2 = "smartbattery_kanim";
		float num4 = 60f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		float num5 = 800f;
		float num6 = 0f;
		float num7 = 0.5f;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER1;
		BuildingDef buildingDef = base.CreateBuildingDef(text, num, num2, num3, text2, num4, tier, refined_METALS, num5, num6, num7, global::TUNING.BUILDINGS.DECOR.PENALTY.TIER2, tier2);
		SoundEventVolumeCache.instance.AddVolume("batterymed_kanim", "Battery_med_rattle", NOISE_POLLUTION.NOISY.TIER2);
		return buildingDef;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, null, BatterySmartConfig.OUTPUT_PORTS);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, null, BatterySmartConfig.OUTPUT_PORTS);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BatterySmart batterySmart = go.AddOrGet<BatterySmart>();
		batterySmart.capacity = 20000f;
		batterySmart.joulesLostPerSecond = batterySmart.capacity * 0.02f / 600f;
		batterySmart.powerSortOrder = 1000;
		GeneratedBuildings.RegisterLogicPorts(go, null, BatterySmartConfig.OUTPUT_PORTS);
		base.DoPostConfigureComplete(go);
	}

	public const string ID = "BatterySmart";

	private static readonly LogicPorts.Port[] OUTPUT_PORTS = new LogicPorts.Port[] { LogicPorts.Port.OutputPort(BatterySmart.PORT_ID, new CellOffset(0, 0), global::STRINGS.BUILDINGS.PREFABS.BATTERYSMART.LOGIC_PORT, global::STRINGS.BUILDINGS.PREFABS.BATTERYSMART.LOGIC_PORT_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.BATTERYSMART.LOGIC_PORT_INACTIVE, true, false) };
}
