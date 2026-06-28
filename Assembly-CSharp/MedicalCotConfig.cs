using System;
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
		float num3 = 200f;
		int num4 = 10;
		float num5 = 10f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] raw_MINERALS = MATERIALS.RAW_MINERALS;
		float num6 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, num5, tier, raw_MINERALS, num6, buildLocationRule, BUILDINGS.DECOR.NONE, none);
		buildingDef.HotKey = global::Action.BuildMenuKeyB;
		buildingDef.Overheatable = false;
		buildingDef.AudioCategory = "Metal";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddPrefabTag(RoomConstraints.ConstraintTags.Clinic);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		go.GetComponent<KAnimControllerBase>().initialAnim = "off";
		go.GetComponent<KPrefabID>().AddTag(TagManager.Create("Bed", null));
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
		Sleepable sleepable = go.AddOrGet<Sleepable>();
		sleepable.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_med_cot_sick_kanim") };
		DoctorChore doctorChore = go.AddOrGet<DoctorChore>();
		doctorChore.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_med_cot_doctor_kanim") };
		doctorChore.workTime = 45f;
	}

	public const string ID = "MedicalCot";
}
