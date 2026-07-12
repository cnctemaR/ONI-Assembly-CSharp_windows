using System;
using TUNING;
using UnityEngine;

public class DevHeaterConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "DevHeater";
		int num = 1;
		int num2 = 1;
		string text2 = "dev_generator_kanim";
		int num3 = 100;
		float num4 = 3f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num5 = 9999f;
		BuildLocationRule buildLocationRule = BuildLocationRule.Anywhere;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER5;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, all_METALS, num5, buildLocationRule, BUILDINGS.DECOR.PENALTY.TIER2, tier2, 0.2f);
		buildingDef.RequiresPowerInput = false;
		buildingDef.ViewMode = OverlayModes.Light.ID;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.AudioSize = "large";
		buildingDef.Floodable = false;
		buildingDef.DebugOnly = true;
		buildingDef.Overheatable = false;
		SoundEventVolumeCache.instance.AddVolume("dev_lightgenerator_kanim", "PowerSwitch_on", NOISE_POLLUTION.NOISY.TIER3);
		SoundEventVolumeCache.instance.AddVolume("dev_lightgenerator_kanim", "PowerSwitch_off", NOISE_POLLUTION.NOISY.TIER3);
		return buildingDef;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddTag(GameTags.DevBuilding);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<DirectVolumeHeater>();
	}

	public const string ID = "DevHeater";
}
