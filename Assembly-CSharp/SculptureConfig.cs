using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class SculptureConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "Sculpture";
		int num = 1;
		int num2 = 3;
		string text2 = "sculpture_kanim";
		int num3 = 30;
		float num4 = 120f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] raw_MINERALS = MATERIALS.RAW_MINERALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, raw_MINERALS, num5, buildLocationRule, new EffectorValues
		{
			amount = 5,
			radius = 8
		}, none, 0.2f);
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.ViewMode = SimViewMode.Decor;
		buildingDef.DefaultAnimState = "slab";
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
		Artable artable = go.AddComponent<Sculpture>();
		artable.requiredRolePerk = RoleManager.rolePerks.CanArt.id;
		artable.stages.Add(new Artable.Stage("Default", global::STRINGS.BUILDINGS.PREFABS.SCULPTURE.NAME, "slab", 0, 0, false, Artable.Status.Ready));
		artable.stages.Add(new Artable.Stage("Bad", global::STRINGS.BUILDINGS.PREFABS.SCULPTURE.POORQUALITYNAME, "crap", 0, 5, false, Artable.Status.Ugly));
		artable.stages.Add(new Artable.Stage("Average", global::STRINGS.BUILDINGS.PREFABS.SCULPTURE.AVERAGEQUALITYNAME, "idle", 2, 15, true, Artable.Status.Okay));
	}

	public const string ID = "Sculpture";
}
