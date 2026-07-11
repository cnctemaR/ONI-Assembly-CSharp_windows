using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class CanvasConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "Canvas";
		int num = 2;
		int num2 = 2;
		string text2 = "painting_kanim";
		int num3 = 30;
		float num4 = 120f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] raw_MINERALS = MATERIALS.RAW_MINERALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.Anywhere;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, raw_MINERALS, num5, buildLocationRule, new EffectorValues
		{
			amount = 5,
			radius = 6
		}, none, 0.2f);
		buildingDef.Floodable = false;
		buildingDef.SceneLayer = Grid.SceneLayer.Paintings;
		buildingDef.Overheatable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.ViewMode = SimViewMode.Decor;
		buildingDef.DefaultAnimState = "off";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<BuildingComplete>().isArtable = true;
		go.GetComponent<KPrefabID>().AddTag(GameTags.Decoration);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		Artable artable = go.AddComponent<Painting>();
		artable.requiredRolePerk = RoleManager.rolePerks.CanArt.id;
		artable.stages.Add(new Artable.Stage("Default", global::STRINGS.BUILDINGS.PREFABS.CANVAS.NAME, "off", 0, 0, false, Artable.Status.Ready));
		artable.stages.Add(new Artable.Stage("Bad", global::STRINGS.BUILDINGS.PREFABS.CANVAS.POORQUALITYNAME, "art_a", 0, 5, false, Artable.Status.Ugly));
		artable.stages.Add(new Artable.Stage("Average", global::STRINGS.BUILDINGS.PREFABS.CANVAS.AVERAGEQUALITYNAME, "art_b", 2, 10, false, Artable.Status.Okay));
		artable.stages.Add(new Artable.Stage("Good", global::STRINGS.BUILDINGS.PREFABS.CANVAS.EXCELLENTQUALITYNAME, "art_c", 4, 15, true, Artable.Status.Great));
		artable.stages.Add(new Artable.Stage("Good2", global::STRINGS.BUILDINGS.PREFABS.CANVAS.EXCELLENTQUALITYNAME, "art_d", 4, 15, true, Artable.Status.Great));
		artable.stages.Add(new Artable.Stage("Good3", global::STRINGS.BUILDINGS.PREFABS.CANVAS.EXCELLENTQUALITYNAME, "art_e", 4, 15, true, Artable.Status.Great));
	}

	public const string ID = "Canvas";
}
