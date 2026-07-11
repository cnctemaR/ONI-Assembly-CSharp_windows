using System;
using TUNING;
using UnityEngine;

public class CometDetectorConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string id = CometDetectorConfig.ID;
		int num = 2;
		int num2 = 4;
		string text = "meteor_detector_kanim";
		int num3 = 30;
		float num4 = 30f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, num, num2, text, num3, num4, tier, refined_METALS, num5, buildLocationRule, BUILDINGS.DECOR.PENALTY.TIER0, none, 0.2f);
		buildingDef.Overheatable = false;
		buildingDef.Floodable = true;
		buildingDef.Entombable = true;
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.ViewMode = OverlayModes.Logic.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		SoundEventVolumeCache.instance.AddVolume("world_element_sensor_kanim", "PowerSwitch_on", NOISE_POLLUTION.NOISY.TIER3);
		SoundEventVolumeCache.instance.AddVolume("world_element_sensor_kanim", "PowerSwitch_off", NOISE_POLLUTION.NOISY.TIER3);
		GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, CometDetectorConfig.ID);
		return buildingDef;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, LogicSwitchConfig.OUTPUT_PORT);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, LogicSwitchConfig.OUTPUT_PORT);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		GeneratedBuildings.RegisterLogicPorts(go, LogicSwitchConfig.OUTPUT_PORT);
		go.AddOrGetDef<CometDetector.Def>();
	}

	public static string ID = "CometDetector";
}
