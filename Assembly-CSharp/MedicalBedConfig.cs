using System;
using TUNING;
using UnityEngine;

public class MedicalBedConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("MedicalBed", 2, 3, "bed_medical_kanim", 200f, 100, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.RAW_MINERALS, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.BONUS.TIER2, null);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 240f;
		buildingDef.ExhaustKilowattsWhenActive = 0.25f;
		buildingDef.OperatingKilowatts = 0.5f;
		buildingDef.AudioCategory = "Metal";
		buildingDef.MinionEffect = "Sleep";
		buildingDef.Slot = Db.Get().OwnableSlots.Clinic.Id;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<LoopingSounds>();
		Clinic clinic = go.AddOrGet<Clinic>();
		clinic.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_medical_bed_kanim") };
		RestRestoreHealth restRestoreHealth = go.AddOrGet<RestRestoreHealth>();
		restRestoreHealth.HitPointsPerDay = 150f;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		go.GetComponent<KAnimControllerBase>().initialAnim = "off";
		go.GetComponent<KPrefabID>().AddTag(TagManager.Create("Bed", null));
	}
}
