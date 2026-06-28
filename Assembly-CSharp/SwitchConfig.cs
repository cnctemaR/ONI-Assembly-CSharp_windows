using System;
using TUNING;
using UnityEngine;

public class SwitchConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string id = SwitchConfig.ID;
		int num = 1;
		int num2 = 1;
		string text = "switchpower_kanim";
		float num3 = 100f;
		int num4 = 10;
		float num5 = 30f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num6 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.Anywhere;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, num, num2, text, num3, num4, num5, tier, all_METALS, num6, buildLocationRule, BUILDINGS.DECOR.NONE, none);
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.MaterialCategory = MATERIALS.ALL_METALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		buildingDef.HotKey = global::Action.BuildMenuKeyS;
		SoundEventVolumeCache.instance.AddVolume("switchpower_kanim", "PowerSwitch_on", NOISE_POLLUTION.NOISY.TIER3);
		SoundEventVolumeCache.instance.AddVolume("switchpower_kanim", "PowerSwitch_off", NOISE_POLLUTION.NOISY.TIER3);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		CircuitSwitch circuitSwitch = go.AddOrGet<CircuitSwitch>();
		circuitSwitch.objectLayer = ObjectLayer.Wire;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		go.AddComponent<BuildingCellVisualizer>();
	}

	public static string ID = "Switch";
}
