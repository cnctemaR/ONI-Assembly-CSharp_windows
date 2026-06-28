using System;
using TUNING;
using UnityEngine;

public class BottleEmptierConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "BottleEmptier";
		int num = 1;
		int num2 = 3;
		string text2 = "liquidator_kanim";
		int num3 = 30;
		float num4 = 10f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] raw_MINERALS = MATERIALS.RAW_MINERALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, raw_MINERALS, num5, buildLocationRule, BUILDINGS.DECOR.PENALTY.TIER1, none, 0.2f);
		buildingDef.Floodable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.Overheatable = false;
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Prioritizable.AddRef(go);
		Storage storage = go.AddOrGet<Storage>();
		storage.storageFilters = STORAGEFILTERS.LIQUIDS;
		storage.showInUI = true;
		storage.showDescriptor = true;
		storage.capacityKg = 200f;
		go.AddOrGet<TreeFilterable>();
		go.AddOrGet<UserMenu>();
		BottleEmptier bottleEmptier = go.AddOrGet<BottleEmptier>();
		bottleEmptier.noFilterTint = BottleEmptierConfig.NoFilterTint;
		bottleEmptier.filterTint = BottleEmptierConfig.FilterTint;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}

	private static readonly Color NoFilterTint = new Color(0.5019608f, 0.5019608f, 0.5019608f, 1f);

	private static readonly Color FilterTint = new Color(1f, 1f, 1f, 1f);

	public const string ID = "BottleEmptier";
}
