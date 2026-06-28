using System;
using TUNING;
using UnityEngine;

public class FarmTileConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("FarmTile", 1, 1, "farmtilerotating_kanim", 100f, 100, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.FARMABLE, 1600f, BuildLocationRule.Tile, BUILDINGS.DECOR.NONE, null);
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.Relocatable = false;
		buildingDef.Overheatable = false;
		buildingDef.IsFoundation = true;
		buildingDef.TileLayer = ObjectLayer.FoundationTile;
		buildingDef.ReplacementLayer = ObjectLayer.ReplacementTile;
		buildingDef.MaterialCategory = MATERIALS.FARMABLE;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
		buildingDef.ConstructionOffsetFilter = new CellOffset[]
		{
			new CellOffset(0, -1)
		};
		buildingDef.PermittedRotations = PermittedRotations.FlipV;
		buildingDef.isSolidTile = false;
		buildingDef.DragBuild = true;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		GeneratedBuildings.MakeBuildableAnywhere(go);
		SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
		simCellOccupier.doReplaceElement = true;
		go.AddOrGet<TileTemperature>();
		BuildingTemplates.CreateDefaultStorage(go, false);
		PlantablePlot plantablePlot = go.AddOrGet<PlantablePlot>();
		plantablePlot.occupyingObjectRelativePosition = new Vector3(0f, 1f, 0f);
		plantablePlot.AddDespoitTag(GameTags.CropSeed);
		plantablePlot.SetFertilizationFlags(true, false);
		go.AddOrGet<AnimTileable>();
		go.AddOrGet<Prioritizable>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		FarmTileConfig.SetUpFarmPlotTags(go);
	}

	public static void SetUpFarmPlotTags(GameObject go)
	{
		KPrefabID component = go.GetComponent<KPrefabID>();
		component.prefabSpawnFn += delegate(GameObject inst)
		{
			Rotatable component2 = inst.GetComponent<Rotatable>();
			PlantablePlot component3 = inst.GetComponent<PlantablePlot>();
			switch (component2.GetOrientation())
			{
			case Orientation.Neutral:
			case Orientation.FlipH:
				component3.SetReceptacleDirection(SingleEntityReceptacle.ReceptacleDirection.Top);
				break;
			case Orientation.R90:
			case Orientation.R270:
				component3.SetReceptacleDirection(SingleEntityReceptacle.ReceptacleDirection.Side);
				break;
			case Orientation.R180:
			case Orientation.FlipV:
				component3.SetReceptacleDirection(SingleEntityReceptacle.ReceptacleDirection.Bottom);
				break;
			}
		};
	}

	public const string ID = "FarmTile";
}
