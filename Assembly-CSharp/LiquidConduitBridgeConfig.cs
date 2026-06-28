using System;
using TUNING;
using UnityEngine;

public class LiquidConduitBridgeConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "LiquidConduitBridge";
		int num = 3;
		int num2 = 1;
		string text2 = "utilityliquidbridge_kanim";
		float num3 = 100f;
		int num4 = 10;
		float num5 = 3f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
		string[] raw_MINERALS = MATERIALS.RAW_MINERALS;
		float num6 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.Conduit;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, num5, tier, raw_MINERALS, num6, buildLocationRule, BUILDINGS.DECOR.NONE, none);
		buildingDef.ObjectLayer = ObjectLayer.LiquidConduitConnection;
		buildingDef.SceneLayer = Grid.SceneLayer.LiquidConduitBridges;
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.OutputConduitType = ConduitType.Liquid;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.ViewMode = SimViewMode.LiquidVentMap;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(1, 0);
		buildingDef.HotKey = global::Action.BuildMenuKeyB;
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, "LiquidConduitBridge");
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		GeneratedBuildings.MakeBuildableAnywhere(go);
		ConduitBridge conduitBridge = go.AddOrGet<ConduitBridge>();
		conduitBridge.type = ConduitType.Liquid;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		global::UnityEngine.Object.DestroyImmediate(go.GetComponent<RequireInputs>());
		global::UnityEngine.Object.DestroyImmediate(go.GetComponent<ConduitConsumer>());
		global::UnityEngine.Object.DestroyImmediate(go.GetComponent<ConduitDispenser>());
		BuildingTemplates.DoPostConfigure(go);
	}

	public const string ID = "LiquidConduitBridge";

	private const ConduitType CONDUIT_TYPE = ConduitType.Liquid;
}
