using System;
using TUNING;
using UnityEngine;

public class GasConduitBridgeConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("GasConduitBridge", 3, 1, "utilitygasbridge_kanim", 100f, 3f, BUILDINGS.CONSTRUCTION_MASS.TIER2, MATERIALS.RAW_MINERALS, 1600f, BuildLocationRule.Tile, BUILDINGS.DECOR.NONE, null);
		buildingDef.InputConduitType = ConduitType.Gas;
		buildingDef.OutputConduitType = ConduitType.Gas;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.ViewMode = SimViewMode.GasVentMap;
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
		conduitJoiner.type = Vent.Transfer.Gas;
		Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
		storage.capacityKg = 1f;
		storage.showInUI = true;
	}

	public override void DoPostConfigure(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}
}
