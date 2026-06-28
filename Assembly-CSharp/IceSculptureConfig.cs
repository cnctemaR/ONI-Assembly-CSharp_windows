using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class IceSculptureConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "IceSculpture";
		int num = 2;
		int num2 = 2;
		string text2 = "icesculpture_kanim";
		int num3 = 10;
		float num4 = 120f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] array = new string[] { "Ice" };
		float num5 = 273.15f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, array, num5, buildLocationRule, new EffectorValues
		{
			amount = 10,
			radius = 10
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
		go.GetComponent<KPrefabID>().AddPrefabTag(GameTags.Decoration);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		Artable artable = go.AddComponent<Sculpture>();
		artable.requiredRolePerk = RoleManager.rolePerks.CanArt.id;
		artable.stages.Add(new Artable.Stage("Default", global::STRINGS.BUILDINGS.PREFABS.ICESCULPTURE.NAME, "slab", 0, 0, false, Artable.Status.Ready));
		artable.stages.Add(new Artable.Stage("Bad", global::STRINGS.BUILDINGS.PREFABS.ICESCULPTURE.POORQUALITYNAME, "crap", 0, 5, false, Artable.Status.Ugly));
		artable.stages.Add(new Artable.Stage("Average", global::STRINGS.BUILDINGS.PREFABS.ICESCULPTURE.AVERAGEQUALITYNAME, "idle", 2, 15, true, Artable.Status.Okay));
	}

	public const string ID = "IceSculpture";
}
