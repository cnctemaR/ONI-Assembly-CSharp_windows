using System;
using TUNING;
using UnityEngine;

public class BedConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("Bed", 2, 2, "bedlg_kanim", 200f, 10, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.RAW_MINERALS, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.NONE, null);
		buildingDef.Overheatable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.MinionEffect = "Sleep";
		buildingDef.Slot = Db.Get().OwnableSlots.Bed.Id;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<LoopingSounds>();
		go.AddOrGet<Sleepable>();
		RestRestoreHealth restRestoreHealth = go.AddOrGet<RestRestoreHealth>();
		restRestoreHealth.HitPointsPerDay = 50f;
		restRestoreHealth.CaloriesPerDay = 0f;
		restRestoreHealth.ChanceForNewTraitWhenChangeState = 0.1f;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		go.GetComponent<KAnimControllerBase>().initialAnim = "off";
	}
}
