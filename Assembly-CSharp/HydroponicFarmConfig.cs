using System;
using TUNING;
using UnityEngine;

public class HydroponicFarmConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("HydroponicFarm", 1, 1, "farmtilehydroponicrotating_kanim", 100f, 100, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.Tile, BUILDINGS.DECOR.PENALTY.TIER0, null);
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.Overheatable = false;
		buildingDef.Relocatable = false;
		buildingDef.IsFoundation = true;
		buildingDef.UseStructureTemperature = false;
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
		buildingDef.PermittedRotations = PermittedRotations.FlipV;
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
		simCellOccupier.doReplaceElement = true;
		go.AddOrGet<TileTemperature>();
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.consumptionRate = 1f;
		conduitConsumer.capacityKG = 5f;
		conduitConsumer.capacityTag = GameTags.Liquid;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		go.AddOrGet<Storage>();
		PlantablePlot plantablePlot = go.AddOrGet<PlantablePlot>();
		plantablePlot.AddDespoitTag(GameTags.CropSeed);
		plantablePlot.occupyingObjectRelativePosition.y = 1f;
		plantablePlot.SetFertilizationFlags(true, true);
		BuildingTemplates.CreateDefaultStorage(go, false);
		go.AddOrGet<PlanterBox>();
		go.AddOrGet<AnimTileable>();
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<Prioritizable>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		FarmTileConfig.SetUpFarmPlotTags(go);
	}

	public const string ID = "HydroponicFarm";
}
