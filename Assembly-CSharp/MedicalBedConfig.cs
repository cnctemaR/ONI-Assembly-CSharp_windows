using System;
using TUNING;
using UnityEngine;

public class MedicalBedConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "MedicalBed";
		int num = 2;
		int num2 = 3;
		string text2 = "bed_medical_kanim";
		int num3 = 100;
		float num4 = 10f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER1;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, refined_METALS, num5, buildLocationRule, BUILDINGS.DECOR.BONUS.TIER2, tier2, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 240f;
		buildingDef.ExhaustKilowattsWhenActive = 0.25f;
		buildingDef.SelfHeatKilowattsWhenActive = 0.5f;
		buildingDef.AudioCategory = "Metal";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.Clinic);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		go.GetComponent<KAnimControllerBase>().initialAnim = "off";
		go.GetComponent<KPrefabID>().AddTag(TagManager.Create("Bed"));
		Clinic clinic = go.AddOrGet<Clinic>();
		clinic.workerInjuredAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_medical_bed_kanim") };
		clinic.workerDiseasedAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_medical_bed_kanim") };
		clinic.workLayer = Grid.SceneLayer.BuildingFront;
		clinic.doctorVisitInterval = 450f;
		clinic.workLayer = Grid.SceneLayer.BuildingFront;
		string text = "Rejuvenator";
		string text2 = "RejuvenatorDoctored";
		clinic.diseaseEffect = text;
		clinic.doctoredDiseaseEffect = text2;
		clinic.doctoredPlaceholderEffect = "DoctoredOffRejuvenatorEffect";
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.Hospital.Id;
		roomTracker.requirement = RoomTracker.Requirement.CustomRecommended;
		roomTracker.customStatusItemID = Db.Get().BuildingStatusItems.ClinicOutsideHospital.Id;
		Sleepable sleepable = go.AddOrGet<Sleepable>();
		sleepable.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_medical_bed_kanim") };
		DoctorChoreWorkable doctorChoreWorkable = go.AddOrGet<DoctorChoreWorkable>();
		doctorChoreWorkable.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_med_cot_doctor_kanim") };
		doctorChoreWorkable.workTime = 25f;
		Ownable ownable = go.AddOrGet<Ownable>();
		ownable.slotID = Db.Get().AssignableSlots.Clinic.Id;
	}

	public const string ID = "MedicalBed";
}
