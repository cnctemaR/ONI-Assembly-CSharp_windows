using System;
using TUNING;
using UnityEngine;

public class GraveConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("Grave", 1, 2, "gravestone_kanim", 100f, 30, 120f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER5, MATERIALS.RAW_MINERALS, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER2, null);
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.Relocatable = false;
		buildingDef.MaterialCategory = MATERIALS.RAW_MINERALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.BaseTimeUntilRepair = -1f;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		Storage storage = go.AddOrGet<Storage>();
		storage.disableOnStore = true;
		storage.showInUI = true;
		go.AddOrGet<Grave>();
		go.AddOrGet<Prioritizable>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}
}
