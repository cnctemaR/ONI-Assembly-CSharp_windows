using System;
using OverlayModes;
using TUNING;
using UnityEngine;

public class LogicWireBridgeConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "LogicWireBridge";
		int num = 3;
		int num2 = 1;
		string text2 = "logic_bridge_kanim";
		float num3 = 100f;
		int num4 = 30;
		float num5 = 3f;
		float[] tier_TINY = BUILDINGS.CONSTRUCTION_MASS_KG.TIER_TINY;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		float num6 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.Anywhere;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, num5, tier_TINY, refined_METALS, num6, buildLocationRule, BUILDINGS.DECOR.PENALTY.TIER0, none);
		buildingDef.ViewMode = SimViewMode.Logic;
		buildingDef.ObjectLayer = ObjectLayer.LogicGates;
		buildingDef.SceneLayer = Grid.SceneLayer.WireBridges;
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 2);
		buildingDef.HotKey = global::Action.BuildMenuKeyB;
		GeneratedBuildings.RegisterWithOverlay(Logic.HighlightItemIDs, "LogicWireBridge");
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		base.DoPostConfigurePreview(def, go);
		LogicUtilityNetworkLink logicUtilityNetworkLink = this.AddNetworkLink(go);
		logicUtilityNetworkLink.visualizeOnly = true;
		go.AddOrGet<BuildingCellVisualizer>();
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		LogicUtilityNetworkLink logicUtilityNetworkLink = this.AddNetworkLink(go);
		logicUtilityNetworkLink.visualizeOnly = true;
		go.AddOrGet<BuildingCellVisualizer>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		LogicUtilityNetworkLink logicUtilityNetworkLink = this.AddNetworkLink(go);
		logicUtilityNetworkLink.visualizeOnly = false;
		go.AddOrGet<BuildingCellVisualizer>();
		BuildingTemplates.DoPostConfigure(go);
	}

	private LogicUtilityNetworkLink AddNetworkLink(GameObject go)
	{
		LogicUtilityNetworkLink logicUtilityNetworkLink = go.AddOrGet<LogicUtilityNetworkLink>();
		logicUtilityNetworkLink.link1 = new CellOffset(-1, 0);
		logicUtilityNetworkLink.link2 = new CellOffset(1, 0);
		return logicUtilityNetworkLink;
	}

	public const string ID = "LogicWireBridge";
}
