using System;
using TUNING;
using UnityEngine;

public class PlanterBoxConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("PlanterBox", 1, 1, "planterbox_kanim", 400f, 10, 3f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.FARMABLE, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER1, null);
		buildingDef.ForegroundLayer = Grid.SceneLayer.BuildingBack;
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.MaterialCategory = MATERIALS.FARMABLE;
		buildingDef.AudioCategory = "Glass";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<Storage>();
		PlantablePlot plantablePlot = go.AddOrGet<PlantablePlot>();
		plantablePlot.AddDespoitTag(GameTags.CropSeed);
		plantablePlot.SetFertilizationFlags(false, false);
		BuildingTemplates.CreateDefaultStorage(go, false);
		go.AddOrGet<PlanterBox>();
		go.AddOrGet<AnimTileable>();
		go.AddOrGet<Prioritizable>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}
}
