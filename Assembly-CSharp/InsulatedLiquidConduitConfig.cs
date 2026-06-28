using System;
using TUNING;
using UnityEngine;

public class InsulatedLiquidConduitConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("InsulatedLiquidConduit", 1, 1, "utilities_liquid_insulated_kanim", 400f, 30, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.PLUMBABLE, 1600f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.PENALTY.TIER0, null);
		buildingDef.Insulation = 0.05f;
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.Entombable = false;
		buildingDef.Relocatable = false;
		buildingDef.ViewMode = SimViewMode.LiquidVentMap;
		buildingDef.ObjectLayer = ObjectLayer.LiquidConduit;
		buildingDef.TileLayer = ObjectLayer.LiquidConduitTile;
		buildingDef.ReplacementLayer = ObjectLayer.ReplacementLiquidConduit;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
		buildingDef.SceneLayer = Grid.SceneLayer.LiquidConduits;
		buildingDef.isKAnimTile = true;
		buildingDef.isUtility = true;
		buildingDef.OverlayAnim = Assets.GetAnim("utilities_liquid_insulated_kanim");
		buildingDef.DragBuild = true;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		Conduit conduit = go.AddOrGet<Conduit>();
		conduit.type = ConduitType.Liquid;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<Building>().Def.BuildingUnderConstruction.GetComponent<Constructable>().isDiggingRequired = false;
		KAnimGraphTileVisualizer kanimGraphTileVisualizer = go.AddComponent<KAnimGraphTileVisualizer>();
		kanimGraphTileVisualizer.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Liquid;
		kanimGraphTileVisualizer.isPhysicalBuilding = true;
		BuildingTemplates.DoPostConfigure(go);
		LiquidConduitConfig.CommonConduitPostConfigureComplete(go);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		KAnimGraphTileVisualizer kanimGraphTileVisualizer = go.AddComponent<KAnimGraphTileVisualizer>();
		kanimGraphTileVisualizer.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Liquid;
		kanimGraphTileVisualizer.isPhysicalBuilding = false;
	}
}
