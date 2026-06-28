using System;
using TUNING;
using UnityEngine;

public class MedicalCotConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("MedicalCot", 3, 2, "medical_cot_kanim", 200f, 10, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.RAW_MINERALS, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.NONE, null);
		buildingDef.Overheatable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.MinionEffect = "Sleep";
		buildingDef.Slot = Db.Get().OwnableSlots.Clinic.Id;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<LoopingSounds>();
		Clinic clinic = go.AddOrGet<Clinic>();
		clinic.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_healing_bed_kanim") };
		RestRestoreHealth restRestoreHealth = go.AddOrGet<RestRestoreHealth>();
		restRestoreHealth.HitPointsPerDay = 50f;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		go.GetComponent<KAnimControllerBase>().initialAnim = "off";
		go.GetComponent<KPrefabID>().AddTag(TagManager.Create("Bed", null));
	}
}
