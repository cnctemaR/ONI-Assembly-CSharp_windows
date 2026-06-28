using System;
using TUNING;
using UnityEngine;

public class WireBridgeConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("WireBridge", 3, 1, "utilityelectricbridge_kanim", 100f, 3f, BUILDINGS.CONSTRUCTION_MASS.TIER0, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.Tile, BUILDINGS.DECOR.PENALTY.TIER0, null);
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.ObjectLayer = ObjectLayer.WireConnectors;
		buildingDef.MaterialCategory = MATERIALS.ALL_METALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.PermittedRotations = Rotatable.PermittedRotations.R360;
		buildingDef.DisableWhenInactive = true;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 2);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		UtilityNetworkLink utilityNetworkLink = go.AddOrGet<UtilityNetworkLink>();
		utilityNetworkLink.link = new CellOffset(0, 2);
	}

	public override void DoPostConfigure(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}
}
