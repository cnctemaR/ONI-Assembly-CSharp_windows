using System;
using TUNING;
using UnityEngine;

public class SwitchConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(SwitchConfig.ID, 1, 1, "switchpower_kanim", 100f, 10, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.NONE, null);
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.MaterialCategory = MATERIALS.ALL_METALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
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
