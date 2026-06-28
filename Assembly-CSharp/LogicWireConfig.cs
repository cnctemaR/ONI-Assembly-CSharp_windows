using System;
using OverlayModes;
using TUNING;
using UnityEngine;

public class LogicWireConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "LogicWire";
		int num = 1;
		int num2 = 1;
		string text2 = "logic_wires_kanim";
		float num3 = 800f;
		int num4 = 10;
		float num5 = 3f;
		float[] tier_TINY = BUILDINGS.CONSTRUCTION_MASS_KG.TIER_TINY;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		float num6 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.Anywhere;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, num5, tier_TINY, refined_METALS, num6, buildLocationRule, BUILDINGS.DECOR.PENALTY.TIER0, none);
		buildingDef.ViewMode = SimViewMode.Logic;
		buildingDef.ObjectLayer = ObjectLayer.LogicWires;
		buildingDef.TileLayer = ObjectLayer.LogicWiresTiling;
		buildingDef.SceneLayer = Grid.SceneLayer.Wires;
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.Entombable = false;
		buildingDef.Relocatable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.isKAnimTile = true;
		buildingDef.isUtility = true;
		buildingDef.DragBuild = true;
		buildingDef.HotKey = global::Action.BuildMenuKeyW;
		GeneratedBuildings.RegisterWithOverlay(Logic.HighlightItemIDs, "LogicWire");
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		GeneratedBuildings.MakeBuildableAnywhere(go);
		KAnimGraphTileVisualizer kanimGraphTileVisualizer = go.AddOrGet<KAnimGraphTileVisualizer>();
		kanimGraphTileVisualizer.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Logic;
		kanimGraphTileVisualizer.isPhysicalBuilding = true;
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		go.GetComponent<Constructable>().isDiggingRequired = false;
		KAnimGraphTileVisualizer kanimGraphTileVisualizer = go.AddOrGet<KAnimGraphTileVisualizer>();
		kanimGraphTileVisualizer.connectionSource = KAnimGraphTileVisualizer.ConnectionSource.Logic;
		kanimGraphTileVisualizer.isPhysicalBuilding = false;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicWire>();
	}

	public const string ID = "LogicWire";
}
