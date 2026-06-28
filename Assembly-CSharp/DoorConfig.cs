using System;
using TUNING;
using UnityEngine;

public class DoorConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("Door", 1, 2, "door_internal_kanim", 100f, 30, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.NONE, null);
		buildingDef.Entombable = false;
		buildingDef.IsFoundation = true;
		buildingDef.MaterialCategory = MATERIALS.ALL_METALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PermittedRotations = PermittedRotations.R90;
		buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		Door door = go.AddOrGet<Door>();
		door.unpoweredAnimSpeed = 1f;
		door.doorType = Door.DoorType.Internal;
		Workable workable = go.AddOrGet<Workable>();
		workable.workTime = 3f;
		go.AddOrGet<BoxCollider2D>();
		go.AddOrGet<Prioritizable>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}
}
