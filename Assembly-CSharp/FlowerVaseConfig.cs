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
		float num3 = 50f;
		int num4 = 10;
		float num5 = 10f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER1;
		string[] farmable = MATERIALS.FARMABLE;
		float num6 = 800f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, num5, tier, farmable, num6, buildLocationRule, BUILDINGS.DECOR.NONE, none);
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.ViewMode = SimViewMode.Decor;
		buildingDef.MaterialCategory = MATERIALS.RAW_MINERALS;
		buildingDef.AudioCategory = "Glass";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<Storage>();
		Prioritizable.AddRef(go);
		PlantablePlot plantablePlot = go.AddOrGet<PlantablePlot>();
		plantablePlot.AddDespoitTag(GameTags.DecorSeed);
		go.AddOrGet<FlowerVase>();
		go.AddOrGet<AnimTileable>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}

	public const string ID = "FlowerVase";
}
