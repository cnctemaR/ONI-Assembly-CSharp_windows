using System;
using TUNING;
using UnityEngine;

public class BedConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string id = BedConfig.ID;
		int num = 2;
		int num2 = 2;
		string text = "bedlg_kanim";
		float num3 = 200f;
		int num4 = 10;
		float num5 = 10f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] raw_MINERALS = MATERIALS.RAW_MINERALS;
		float num6 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, num, num2, text, num3, num4, num5, tier, raw_MINERALS, num6, buildLocationRule, BUILDINGS.DECOR.NONE, none);
		buildingDef.Overheatable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.HotKey = global::Action.BuildMenuKeyC;
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
		bed.effects = new string[] { "BedStamina" };
		bed.workLayer = Grid.SceneLayer.BuildingFront;
		Sleepable sleepable = go.AddOrGet<Sleepable>();
		sleepable.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_sleep_bed_kanim") };
		sleepable.workLayer = Grid.SceneLayer.BuildingFront;
	}

	public static string ID = "Bed";
}
