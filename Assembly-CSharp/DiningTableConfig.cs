using System;
using TUNING;
using UnityEngine;

public class DiningTableConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("DiningTable", 1, 1, "diningtable_kanim", 25f, 10, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.NONE, null);
		buildingDef.WorkTime = 20f;
		buildingDef.Overheatable = false;
		buildingDef.MaterialCategory = MATERIALS.ALL_METALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.Slot = "MessStation";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<LoopingSounds>();
		go.AddOrGet<MessStation>();
		Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
		storage.showInUI = true;
		RequiresRegion requiresRegion = go.AddComponent<RequiresRegion>();
		requiresRegion.RequiredRegions.Add(TagManager.Create("MessHallRegion", null));
		go.AddOrGet<AnimTileable>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		go.GetComponent<KAnimControllerBase>().initialAnim = "off";
	}
}
