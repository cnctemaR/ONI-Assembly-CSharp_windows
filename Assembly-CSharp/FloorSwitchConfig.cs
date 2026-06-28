using System;
using OverlayModes;
using TUNING;
using UnityEngine;

public class FloorSwitchConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "FloorSwitch";
		int num = 1;
		int num2 = 1;
		string text2 = "pressureswitch_kanim";
		float num3 = 100f;
		int num4 = 30;
		float num5 = 30f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER1;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		float num6 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.Tile;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, num5, tier, refined_METALS, num6, buildLocationRule, BUILDINGS.DECOR.PENALTY.TIER0, none);
		buildingDef.IsFoundation = true;
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.Relocatable = false;
		buildingDef.ViewMode = SimViewMode.Logic;
		buildingDef.TileLayer = ObjectLayer.FoundationTile;
		buildingDef.ReplacementLayer = ObjectLayer.ReplacementTile;
		buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
		buildingDef.ConstructionOffsetFilter = new CellOffset[]
		{
			new CellOffset(0, -1)
		};
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		GeneratedBuildings.RegisterWithOverlay(Logic.HighlightItemIDs, "FloorSwitch");
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		GeneratedBuildings.MakeBuildableAnywhere(go);
		SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
		simCellOccupier.doReplaceElement = true;
		simCellOccupier.movementSpeedMultiplier = DUPLICANTSTATS.MOVEMENT.BONUS_2;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, LogicSwitchConfig.OUTPUT_PORT);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, LogicSwitchConfig.OUTPUT_PORT);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, LogicSwitchConfig.OUTPUT_PORT);
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		LogicMassSensor logicMassSensor = go.AddOrGet<LogicMassSensor>();
		logicMassSensor.rangeMin = 0f;
		logicMassSensor.rangeMax = 2000f;
		logicMassSensor.Threshold = 10f;
		logicMassSensor.ActivateAboveThreshold = true;
		logicMassSensor.manuallyControlled = false;
	}

	public const string ID = "FloorSwitch";
}
