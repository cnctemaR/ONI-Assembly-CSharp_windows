using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class BedConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("Bed", 2, 2, "bedlg_kanim", 200f, 10f, global::TUNING.BUILDINGS.CONSTRUCTION_MASS.TIER3, MATERIALS.RAW_MINERALS, 1600f, BuildLocationRule.OnFloor, global::TUNING.BUILDINGS.DECOR.NONE, null);
		buildingDef.AudioCategory = "Metal";
		buildingDef.MinionEffect = "Sleep";
		buildingDef.Slot = Db.Get().OwnableSlots.Bed.Id;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		TagManager.SetProperName("Bed", TAGS.BED);
		go.AddOrGet<LoopingSounds>();
		go.AddOrGet<Sleepable>();
		RestRestoreHealth restRestoreHealth = go.AddOrGet<RestRestoreHealth>();
		restRestoreHealth.HitPointsPerDay = 50f;
		restRestoreHealth.ChanceForNewTraitWhenChangeState = 0.1f;
	}

	public override void DoPostConfigure(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		go.GetComponent<KAnimControllerBase>().initialAnim = "off";
	}
}
