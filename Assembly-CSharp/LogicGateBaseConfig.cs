using System;
using OverlayModes;
using TUNING;
using UnityEngine;

public abstract class LogicGateBaseConfig : IBuildingConfig
{
	protected BuildingDef CreateBuildingDef(string ID, string anim, int width = 2, int height = 2)
	{
		float num = 800f;
		int num2 = 10;
		float num3 = 3f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		float num4 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.Anywhere;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, width, height, anim, num, num2, num3, tier, refined_METALS, num4, buildLocationRule, BUILDINGS.DECOR.PENALTY.TIER0, none);
		buildingDef.ViewMode = SimViewMode.Logic;
		buildingDef.ObjectLayer = ObjectLayer.LogicGates;
		buildingDef.SceneLayer = Grid.SceneLayer.WireBridges;
		buildingDef.Insulation = 0.05f;
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.Entombable = false;
		buildingDef.Relocatable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.DragBuild = true;
		LogicGateBase.uiSrcData = Assets.instance.logicModeUIData;
		GeneratedBuildings.RegisterWithOverlay(Logic.HighlightItemIDs, ID);
		return buildingDef;
	}

	protected abstract LogicGateBase.Op GetLogicOp();

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		GeneratedBuildings.MakeBuildableAnywhere(go);
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		base.DoPostConfigurePreview(def, go);
		MoveableLogicGateVisualizer moveableLogicGateVisualizer = go.AddComponent<MoveableLogicGateVisualizer>();
		moveableLogicGateVisualizer.op = this.GetLogicOp();
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		LogicGateVisualizer logicGateVisualizer = go.AddComponent<LogicGateVisualizer>();
		logicGateVisualizer.op = this.GetLogicOp();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		LogicGate logicGate = go.AddComponent<LogicGate>();
		logicGate.op = this.GetLogicOp();
	}
}
