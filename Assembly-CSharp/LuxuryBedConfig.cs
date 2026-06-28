using System;
using TUNING;
using UnityEngine;

public class LuxuryBedConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string id = LuxuryBedConfig.ID;
		int num = 4;
		int num2 = 2;
		string text = "elegantbed_kanim";
		float num3 = 200f;
		int num4 = 10;
		float num5 = 10f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] plastics = MATERIALS.PLASTICS;
		float num6 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, num, num2, text, num3, num4, num5, tier, plastics, num6, buildLocationRule, BUILDINGS.DECOR.BONUS.TIER2, none);
		buildingDef.Overheatable = false;
		buildingDef.AudioCategory = "Metal";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddPrefabTag(RoomConstraints.ConstraintTags.Bed);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		go.GetComponent<KAnimControllerBase>().initialAnim = "off";
		Bed bed = go.AddOrGet<Bed>();
		bed.effects = new string[] { "LuxuryBedStamina" };
		bed.workLayer = Grid.SceneLayer.BuildingFront;
		Sleepable sleepable = go.AddOrGet<Sleepable>();
		sleepable.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_sleep_bed_kanim") };
		sleepable.workLayer = Grid.SceneLayer.BuildingFront;
	}

	public static string ID = "LuxuryBed";
}
