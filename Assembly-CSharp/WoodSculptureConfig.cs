using System;
using TUNING;
using UnityEngine;

public class WoodSculptureConfig : IBuildingConfig
{
	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_DLC_2;
	}

	public override BuildingDef CreateBuildingDef()
	{
		string text = "WoodSculpture";
		int num = 1;
		int num2 = 1;
		string text2 = "sculpture_wood_kanim";
		int num3 = 10;
		float num4 = 120f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] woods = MATERIALS.WOODS;
		float num5 = 800f;
		BuildLocationRule buildLocationRule = BuildLocationRule.Anywhere;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, woods, num5, buildLocationRule, new EffectorValues
		{
			amount = 4,
			radius = 4
		}, none, 0.2f);
		buildingDef.SceneLayer = Grid.SceneLayer.InteriorWall;
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
		go.GetComponent<KPrefabID>().AddTag(GameTags.Decoration, false);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddComponent<LongRangeSculpture>().defaultAnimName = "slab";
	}

	public const string ID = "WoodSculpture";
}
