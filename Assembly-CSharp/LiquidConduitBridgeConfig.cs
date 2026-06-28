using System;
using TUNING;
using UnityEngine;

public class LiquidConduitBridgeConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("LiquidConduitBridge", 3, 1, "utilityliquidbridge_kanim", 100f, 3f, BUILDINGS.CONSTRUCTION_MASS.TIER2, MATERIALS.RAW_MINERALS, 1600f, BuildLocationRule.Tile, BUILDINGS.DECOR.NONE, null);
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.OutputConduitType = ConduitType.Liquid;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.ViewMode = SimViewMode.LiquidVentMap;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.PermittedRotations = Rotatable.PermittedRotations.R360;
		buildingDef.DisableWhenInactive = true;
		buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		ConduitJoiner conduitJoiner = go.AddOrGet<ConduitJoiner>();
		conduitJoiner.type = Vent.Transfer.Liquid;
		Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
		storage.capacityKg = 10f;
		storage.showInUI = true;
	}

	public override void DoPostConfigure(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}
}
