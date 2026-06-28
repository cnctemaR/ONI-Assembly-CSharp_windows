using System;
using TUNING;
using UnityEngine;

public class PressureSwitchGasConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(PressureSwitchGasConfig.ID, 1, 1, "switchgaspressure_kanim", 50f, 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.PENALTY.TIER0, null);
		buildingDef.Overheatable = false;
		buildingDef.Floodable = true;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.MaterialCategory = MATERIALS.ALL_METALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		PressureSwitch pressureSwitch = go.AddOrGet<PressureSwitch>();
		pressureSwitch.objectLayer = ObjectLayer.Wire;
		pressureSwitch.rangeMin = 0f;
		pressureSwitch.rangeMax = 2f;
		pressureSwitch.Threshold = 1f;
		pressureSwitch.ActivateAboveThreshold = false;
		pressureSwitch.manuallyControlled = false;
		pressureSwitch.desiredState = Element.State.Gas;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		go.AddComponent<BuildingCellVisualizer>();
	}

	public static string ID = "PressureSwitchGas";
}
