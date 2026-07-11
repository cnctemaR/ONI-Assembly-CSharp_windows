using System;
using TUNING;
using UnityEngine;

public class LogicDiseaseSensorConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string id = LogicDiseaseSensorConfig.ID;
		int num = 1;
		int num2 = 1;
		string text = "diseasesensor_kanim";
		int num3 = 30;
		float num4 = 30f;
		float[] array = new float[]
		{
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER0[0],
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER1[0]
		};
		string[] array2 = new string[] { "RefinedMetal", "Plastic" };
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.Anywhere;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, num, num2, text, num3, num4, array, array2, num5, buildLocationRule, BUILDINGS.DECOR.PENALTY.TIER0, none, 0.2f);
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.ViewMode = OverlayModes.Logic.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		SoundEventVolumeCache.instance.AddVolume("diseasesensor_kanim", "PowerSwitch_on", NOISE_POLLUTION.NOISY.TIER3);
		SoundEventVolumeCache.instance.AddVolume("diseasesensor_kanim", "PowerSwitch_off", NOISE_POLLUTION.NOISY.TIER3);
		GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, LogicDiseaseSensorConfig.ID);
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
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		GeneratedBuildings.RegisterLogicPorts(go, LogicSwitchConfig.OUTPUT_PORT);
		LogicDiseaseSensor logicDiseaseSensor = go.AddOrGet<LogicDiseaseSensor>();
		logicDiseaseSensor.Threshold = 0f;
		logicDiseaseSensor.ActivateAboveThreshold = true;
		logicDiseaseSensor.manuallyControlled = false;
	}

	public static string ID = "LogicDiseaseSensor";
}
