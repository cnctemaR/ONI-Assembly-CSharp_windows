using System;
using TUNING;
using UnityEngine;

public class PressureDoorConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("PressureDoor", 1, 2, "door_external_kanim", 100f, 30, 60f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.PENALTY.TIER1, null);
		buildingDef.Overheatable = false;
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.Entombable = false;
		buildingDef.IsFoundation = true;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.TileLayer = ObjectLayer.FoundationTile;
		buildingDef.MaterialCategory = MATERIALS.ALL_METALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PermittedRotations = PermittedRotations.R90;
		buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		Door door = go.UpdateComponentRequirement<Door>(true);
		door.hasComplexUserControls = true;
		door.unpoweredAnimSpeed = 1f;
		go.UpdateComponentRequirement<AccessControl>(true);
		go.UpdateComponentRequirement<BoxCollider2D>(true);
		go.UpdateComponentRequirement<Prioritizable>(true);
		Workable workable = go.AddOrGet<Workable>();
		workable.workTime = 5f;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}
}
