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
		float num3 = 200f;
		int num4 = 100;
		float num5 = 10f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		float num6 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER1;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, num5, tier, refined_METALS, num6, buildLocationRule, BUILDINGS.DECOR.BONUS.TIER2, tier2);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 240f;
		buildingDef.ExhaustKilowattsWhenActive = 0.25f;
		buildingDef.OperatingKilowatts = 0.5f;
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
		Sleepable sleepable = go.AddOrGet<Sleepable>();
		sleepable.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_medical_bed_kanim") };
		DoctorChore doctorChore = go.AddOrGet<DoctorChore>();
		doctorChore.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_med_cot_doctor_kanim") };
		doctorChore.workTime = 25f;
	}

	public const string ID = "MedicalBed";
}
