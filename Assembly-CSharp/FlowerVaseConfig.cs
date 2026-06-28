using System;
using TUNING;
using UnityEngine;

public class FlowerVaseConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "FlowerVase";
		int num = 1;
		int num2 = 1;
		string text2 = "flowervase_kanim";
		int num3 = 10;
		float num4 = 10f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER1;
		string[] farmable = MATERIALS.FARMABLE;
		float num5 = 800f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, farmable, num5, buildLocationRule, BUILDINGS.DECOR.NONE, none, 0.2f);
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.ViewMode = SimViewMode.Decor;
		buildingDef.MaterialCategory = MATERIALS.RAW_MINERALS;
		buildingDef.AudioCategory = "Glass";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<Storage>();
		Prioritizable.AddRef(go);
		PlantablePlot plantablePlot = go.AddOrGet<PlantablePlot>();
		plantablePlot.AddDepositTag(GameTags.DecorSeed);
		go.AddOrGet<FlowerVase>();
		go.AddOrGet<AnimTileable>();
		go.GetComponent<KPrefabID>().AddPrefabTag(GameTags.Decoration);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}

	public const string ID = "FlowerVase";
}
