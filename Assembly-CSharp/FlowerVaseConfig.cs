using System;
using TUNING;
using UnityEngine;

public class FlowerVaseConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("FlowerVase", 1, 2, "flowervase_kanim", 50f, 10f, BUILDINGS.CONSTRUCTION_MASS.TIER1, MATERIALS.FARMABLE, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.BONUS.TIER1, null);
		buildingDef.Floodable = false;
		buildingDef.ViewMode = SimViewMode.Decor;
		buildingDef.MaterialCategory = MATERIALS.RAW_MINERALS;
		buildingDef.AudioCategory = "Glass";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<Storage>();
		go.AddOrGet<Prioritizable>();
		PlantablePlot plantablePlot = go.AddOrGet<PlantablePlot>();
		plantablePlot.AddDespoitTag(GameTags.DecorSeed);
		go.AddOrGet<FlowerVase>();
		go.AddOrGet<AnimTileable>();
	}

	public override void DoPostConfigure(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}
}
