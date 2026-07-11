using System;
using TUNING;
using UnityEngine;

public abstract class LogicGateBaseConfig : IBuildingConfig
{
	protected BuildingDef CreateBuildingDef(string ID, string anim, int width = 2, int height = 2)
	{
		int num = 10;
		float num2 = 3f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		float num3 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.Anywhere;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, width, height, anim, num, num2, tier, refined_METALS, num3, buildLocationRule, BUILDINGS.DECOR.PENALTY.TIER0, none, 0.2f);
		buildingDef.ViewMode = OverlayModes.Logic.ID;
		buildingDef.ObjectLayer = ObjectLayer.LogicGates;
		buildingDef.SceneLayer = Grid.SceneLayer.LogicGates;
		buildingDef.ThermalConductivity = 0.05f;
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.Entombable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.DragBuild = true;
		LogicGateBase.uiSrcData = Assets.instance.logicModeUIData;
		GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);
		return buildingDef;
	}

	protected abstract LogicGateBase.Op GetLogicOp();

	protected abstract LogicGate.LogicGateDescriptions GetDescriptions();

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
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
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			LogicGate component = game_object.GetComponent<LogicGate>();
			component.SetPortDescriptions(this.GetDescriptions());
		};
	}
}
