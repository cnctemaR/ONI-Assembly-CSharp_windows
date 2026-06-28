using System;
using TUNING;
using UnityEngine;

public class WireBridgeConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("WireBridge", 3, 1, "utilityelectricbridge_kanim", 100f, 10, 3f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.PENALTY.TIER0, null);
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.ObjectLayer = ObjectLayer.WireConnectors;
		buildingDef.MaterialCategory = MATERIALS.ALL_METALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 2);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		base.DoPostConfigurePreview(def, go);
		UtilityNetworkLink utilityNetworkLink = this.AddNetworkLink(go);
		utilityNetworkLink.visualizeOnly = true;
		go.AddOrGet<BuildingCellVisualizer>();
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		UtilityNetworkLink utilityNetworkLink = this.AddNetworkLink(go);
		utilityNetworkLink.visualizeOnly = true;
		go.AddOrGet<BuildingCellVisualizer>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		UtilityNetworkLink utilityNetworkLink = this.AddNetworkLink(go);
		utilityNetworkLink.visualizeOnly = false;
		go.AddOrGet<BuildingCellVisualizer>();
		BuildingTemplates.DoPostConfigure(go);
	}

	private UtilityNetworkLink AddNetworkLink(GameObject go)
	{
		UtilityNetworkLink utilityNetworkLink = go.AddOrGet<UtilityNetworkLink>();
		utilityNetworkLink.link1 = new CellOffset(-1, 0);
		utilityNetworkLink.link2 = new CellOffset(1, 0);
		return utilityNetworkLink;
	}
}
