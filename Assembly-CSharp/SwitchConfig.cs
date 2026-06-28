using System;
using TUNING;
using UnityEngine;

public class SwitchConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("Switch", 1, 2, "switchpower_kanim", 100f, 30f, BUILDINGS.CONSTRUCTION_MASS.TIER2, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.NONE, null);
		buildingDef.Floodable = false;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.MaterialCategory = MATERIALS.ALL_METALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		Switch @switch = go.AddOrGet<Switch>();
		@switch.objectLayer = ObjectLayer.Wire;
	}

	public override void DoPostConfigure(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}
}
