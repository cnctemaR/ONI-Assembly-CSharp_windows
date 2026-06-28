using System;
using TUNING;
using UnityEngine;

public class AquafarmConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("Aquafarm", 1, 1, "farmtile_kanim", 100f, 100, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.Tile, BUILDINGS.DECOR.PENALTY.TIER0, null);
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.Overheatable = false;
		buildingDef.Relocatable = false;
		buildingDef.IsFoundation = true;
		buildingDef.TileLayer = ObjectLayer.FoundationTile;
		buildingDef.ReplacementLayer = ObjectLayer.ReplacementTile;
		buildingDef.MaterialCategory = MATERIALS.ALL_METALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
		buildingDef.ConstructionOffsetFilter = new CellOffset[]
		{
			new CellOffset(0, -1)
		};
		buildingDef.isSolidTile = true;
		buildingDef.UseStructureTemperature = false;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<Operational>();
		SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
		simCellOccupier.doReplaceElement = true;
		go.AddOrGet<TileTemperature>();
		go.AddOrGet<Storage>();
		PlantablePlot plantablePlot = go.AddOrGet<PlantablePlot>();
		plantablePlot.AddDespoitTag(GameTags.WaterSeed);
		plantablePlot.occupyingObjectRelativePosition.y = 1.1f;
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
