using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class MarbleSculptureConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "MarbleSculpture";
		int num = 2;
		int num2 = 3;
		string text2 = "sculpture_marble_kanim";
		int num3 = 10;
		float num4 = 120f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] precious_ROCKS = MATERIALS.PRECIOUS_ROCKS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, precious_ROCKS, num5, buildLocationRule, new EffectorValues
		{
			amount = 20,
			radius = 8
		}, none, 0.2f);
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.ViewMode = OverlayModes.Decor.ID;
		buildingDef.DefaultAnimState = "slab";
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<BuildingComplete>().isArtable = true;
		go.GetComponent<KPrefabID>().AddTag(GameTags.Decoration);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Artable artable = go.AddComponent<Sculpture>();
		artable.stages.Add(new Artable.Stage("Default", global::STRINGS.BUILDINGS.PREFABS.MARBLESCULPTURE.NAME, "slab", 0, false, Artable.Status.Ready));
		artable.stages.Add(new Artable.Stage("Bad", global::STRINGS.BUILDINGS.PREFABS.MARBLESCULPTURE.POORQUALITYNAME, "crap_1", 5, false, Artable.Status.Ugly));
		artable.stages.Add(new Artable.Stage("Average", global::STRINGS.BUILDINGS.PREFABS.MARBLESCULPTURE.AVERAGEQUALITYNAME, "good_1", 10, false, Artable.Status.Okay));
		artable.stages.Add(new Artable.Stage("Good1", global::STRINGS.BUILDINGS.PREFABS.MARBLESCULPTURE.EXCELLENTQUALITYNAME, "amazing_1", 15, true, Artable.Status.Okay));
		artable.stages.Add(new Artable.Stage("Good2", global::STRINGS.BUILDINGS.PREFABS.MARBLESCULPTURE.EXCELLENTQUALITYNAME, "amazing_2", 15, true, Artable.Status.Okay));
		artable.stages.Add(new Artable.Stage("Good3", global::STRINGS.BUILDINGS.PREFABS.MARBLESCULPTURE.EXCELLENTQUALITYNAME, "amazing_3", 15, true, Artable.Status.Okay));
	}

	public const string ID = "MarbleSculpture";
}
