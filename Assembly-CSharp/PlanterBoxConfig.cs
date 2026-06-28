using System;
using TUNING;
using UnityEngine;

public class PlanterBoxConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "PlanterBox";
		int num = 1;
		int num2 = 1;
		string text2 = "planterbox_kanim";
		float num3 = 400f;
		int num4 = 10;
		float num5 = 3f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
		string[] farmable = MATERIALS.FARMABLE;
		float num6 = 800f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, num5, tier, farmable, num6, buildLocationRule, BUILDINGS.DECOR.PENALTY.TIER1, none);
		buildingDef.ForegroundLayer = Grid.SceneLayer.BuildingBack;
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.MaterialCategory = MATERIALS.FARMABLE;
		buildingDef.AudioCategory = "Glass";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		Storage storage = go.AddOrGet<Storage>();
		PlantablePlot plantablePlot = go.AddOrGet<PlantablePlot>();
		plantablePlot.AddDespoitTag(GameTags.CropSeed);
		plantablePlot.SetFertilizationFlags(true, false);
		BuildingTemplates.CreateDefaultStorage(go, false);
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<PlanterBox>();
		go.AddOrGet<AnimTileable>();
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}

	public const string ID = "PlanterBox";
}
