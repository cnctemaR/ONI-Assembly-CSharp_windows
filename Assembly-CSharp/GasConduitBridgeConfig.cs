using System;
using TUNING;
using UnityEngine;

public class GasConduitBridgeConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("GasConduitBridge", 3, 1, "utilitygasbridge_kanim", 50f, 10, 3f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, MATERIALS.RAW_MINERALS, 1600f, BuildLocationRule.Tile, BUILDINGS.DECOR.NONE, null);
		buildingDef.ObjectLayer = ObjectLayer.GasConduitConnection;
		buildingDef.InputConduitType = ConduitType.Gas;
		buildingDef.OutputConduitType = ConduitType.Gas;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.ViewMode = SimViewMode.GasVentMap;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(1, 0);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		GeneratedBuildings.MakeBuildableAnywhere(go);
		ConduitBridge conduitBridge = go.AddOrGet<ConduitBridge>();
		conduitBridge.type = ConduitType.Gas;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		global::UnityEngine.Object.DestroyImmediate(go.GetComponent<RequireInputs>());
		global::UnityEngine.Object.DestroyImmediate(go.GetComponent<ConduitConsumer>());
		global::UnityEngine.Object.DestroyImmediate(go.GetComponent<ConduitDispenser>());
		BuildingTemplates.DoPostConfigure(go);
	}

	private const ConduitType CONDUIT_TYPE = ConduitType.Gas;
}
