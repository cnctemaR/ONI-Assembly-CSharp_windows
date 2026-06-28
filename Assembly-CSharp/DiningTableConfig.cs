using System;
using TUNING;
using UnityEngine;

public class DiningTableConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "DiningTable";
		int num = 1;
		int num2 = 1;
		string text2 = "diningtable_kanim";
		float num3 = 25f;
		int num4 = 10;
		float num5 = 10f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num6 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, num5, tier, all_METALS, num6, buildLocationRule, BUILDINGS.DECOR.BONUS.TIER1, none);
		buildingDef.WorkTime = 20f;
		buildingDef.Overheatable = false;
		buildingDef.MaterialCategory = MATERIALS.ALL_METALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.HotKey = global::Action.BuildMenuKeyT;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddPrefabTag(RoomConstraints.ConstraintTags.MessTable);
		go.AddOrGet<MessStation>();
		Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
		storage.showInUI = true;
		RequiresRegion requiresRegion = go.AddComponent<RequiresRegion>();
		requiresRegion.RequiredRegions.Add(TagManager.Create("MessHallRegion", null));
		go.AddOrGet<AnimTileable>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		go.GetComponent<KAnimControllerBase>().initialAnim = "off";
		Ownable ownable = go.AddOrGet<Ownable>();
		ownable.slot = Db.Get().OwnableSlots.MessStation;
	}

	public const string ID = "DiningTable";
}
