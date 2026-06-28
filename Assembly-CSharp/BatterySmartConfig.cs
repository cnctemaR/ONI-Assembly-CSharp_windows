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
		string text2 = "batterymed_kanim";
		float num4 = 1200f;
		float num5 = 60f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		float num6 = 800f;
		float num7 = 0.25f;
		float num8 = 1f;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER1;
		BuildingDef buildingDef = base.CreateBuildingDef(text, num, num2, num3, text2, num4, num5, tier, refined_METALS, num6, num7, num8, global::TUNING.BUILDINGS.DECOR.PENALTY.TIER2, tier2);
		buildingDef.HotKey = global::Action.BuildMenuKeyS;
		buildingDef.Deprecated = true;
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
		batterySmart.capacity = 40000f;
		GeneratedBuildings.RegisterLogicPorts(go, null, BatterySmartConfig.OUTPUT_PORTS);
		base.DoPostConfigureComplete(go);
	}

	public const string ID = "BatterySmart";

	private static readonly LogicPorts.Port[] OUTPUT_PORTS = new LogicPorts.Port[]
	{
		new LogicPorts.Port(BatterySmart.PORT_ID, new CellOffset(0, 0), global::STRINGS.BUILDINGS.PREFABS.BATTERYSMART.LOGIC_PORT_DESC, true)
	};
}
