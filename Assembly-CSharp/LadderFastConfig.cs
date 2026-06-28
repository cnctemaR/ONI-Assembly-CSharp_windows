using System;
using TUNING;
using UnityEngine;

public class LadderFastConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "LadderFast";
		int num = 1;
		int num2 = 1;
		string text2 = "ladder_plastic_kanim";
		float num3 = 100f;
		int num4 = 10;
		float num5 = 10f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
		string[] plastics = MATERIALS.PLASTICS;
		float num6 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.Anywhere;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, num5, tier, plastics, num6, buildLocationRule, BUILDINGS.DECOR.PENALTY.TIER0, none);
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.Entombable = false;
		buildingDef.Relocatable = false;
		buildingDef.AudioCategory = "Plastic";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.DragBuild = true;
		buildingDef.HotKey = global::Action.BuildMenuKeyD;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		Ladder ladder = go.AddOrGet<Ladder>();
		ladder.upwardsMovementSpeedMultiplier = 1.2f;
		ladder.downwardsMovementSpeedMultiplier = 1.2f;
		go.AddOrGet<AnimTileable>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}

	public const string ID = "LadderFast";
}
