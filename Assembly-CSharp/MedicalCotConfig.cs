using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class MedicalCotConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "MedicalCot";
		int num = 3;
		int num2 = 2;
		string text2 = "medical_cot_kanim";
		int num3 = 10;
		float num4 = 10f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] raw_MINERALS = MATERIALS.RAW_MINERALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, raw_MINERALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.NONE, none, 0.2f);
		buildingDef.Overheatable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AddSearchTerms(SEARCH_TERMS.MEDICINE);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.Clinic, false);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<KAnimControllerBase>().initialAnim = "off";
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.BedType, false);
		Clinic clinic = go.AddOrGet<Clinic>();
		clinic.doctorVisitInterval = 300f;
		clinic.workerInjuredAnims = new KAnimFile[] { Assets.GetAnim("anim_healing_bed_kanim") };
		clinic.workerDiseasedAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_med_cot_sick_kanim") };
		clinic.workLayer = Grid.SceneLayer.BuildingFront;
		string text = "MedicalCot";
		string text2 = "MedicalCotDoctored";
		clinic.healthEffect = text;
		clinic.doctoredHealthEffect = text2;
		clinic.diseaseEffect = text;
		clinic.doctoredDiseaseEffect = text2;
		clinic.doctoredPlaceholderEffect = "DoctoredOffCotEffect";
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.Hospital.Id;
		roomTracker.requirement = RoomTracker.Requirement.CustomRecommended;
		roomTracker.customStatusItemID = Db.Get().BuildingStatusItems.ClinicOutsideHospital.Id;
		Sleepable sleepable = go.AddOrGet<Sleepable>();
		sleepable.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_med_cot_sick_kanim") };
		sleepable.isNormalBed = false;
		DoctorChoreWorkable doctorChoreWorkable = go.AddOrGet<DoctorChoreWorkable>();
		doctorChoreWorkable.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_med_cot_doctor_kanim") };
		doctorChoreWorkable.workTime = 45f;
		go.AddOrGet<Ownable>().slotID = Db.Get().AssignableSlots.Clinic.Id;
	}

	public const string ID = "MedicalCot";
}
