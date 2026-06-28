using System;
using TUNING;
using UnityEngine;

public class LadderConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "Ladder";
		int num = 1;
		int num2 = 1;
		string text2 = "ladder_kanim";
		float num3 = 100f;
		int num4 = 10;
		float num5 = 10f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
		string[] all_MINERALS = MATERIALS.ALL_MINERALS;
		float num6 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.Anywhere;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, num5, tier, all_MINERALS, num6, buildLocationRule, BUILDINGS.DECOR.PENALTY.TIER0, none);
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.Entombable = false;
		buildingDef.Relocatable = false;
		buildingDef.MaterialCategory = MATERIALS.ALL_MINERALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.DragBuild = true;
		buildingDef.HotKey = global::Action.BuildMenuKeyA;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		Ladder ladder = go.AddOrGet<Ladder>();
		ladder.upwardsMovementSpeedMultiplier = 1f;
		ladder.downwardsMovementSpeedMultiplier = 1f;
		go.AddOrGet<AnimTileable>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}

	public const string ID = "Ladder";
}
