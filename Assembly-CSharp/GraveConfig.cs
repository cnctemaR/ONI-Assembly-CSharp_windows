using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class GraveConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "Grave";
		int num = 1;
		int num2 = 2;
		string text2 = "gravestone_kanim";
		float num3 = 100f;
		int num4 = 30;
		float num5 = 120f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
		string[] raw_MINERALS = MATERIALS.RAW_MINERALS;
		float num6 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, num5, tier, raw_MINERALS, num6, buildLocationRule, BUILDINGS.DECOR.PENALTY.TIER2, none);
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.Relocatable = false;
		buildingDef.MaterialCategory = MATERIALS.RAW_MINERALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.HotKey = global::Action.BuildMenuKeyG;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		Storage storage = go.AddOrGet<Storage>();
		storage.showInUI = true;
		storage.SetDefaultStoredItemModifiers(GraveConfig.StorageModifiers);
		go.AddOrGet<Grave>();
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}

	public const string ID = "Grave";

	private static readonly List<Storage.StoredItemModifier> StorageModifiers = new List<Storage.StoredItemModifier>
	{
		Storage.StoredItemModifier.Hide,
		Storage.StoredItemModifier.Preserve
	};
}
