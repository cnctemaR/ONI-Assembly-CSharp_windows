using System;
using TUNING;
using UnityEngine;

public class WashBasinConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "WashBasin";
		int num = 2;
		int num2 = 3;
		string text2 = "wash_basin_kanim";
		float num3 = 50f;
		int num4 = 30;
		float num5 = 30f;
		string[] raw_MINERALS = MATERIALS.RAW_MINERALS;
		EffectorValues tier = NOISE_POLLUTION.NOISY.TIER0;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, num5, BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, raw_MINERALS, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.BONUS.TIER1, tier);
		buildingDef.HotKey = global::Action.BuildMenuKeyB;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.GetComponent<KPrefabID>().AddPrefabTag(RoomConstraints.ConstraintTags.WashStation);
		HandSanitizer handSanitizer = go.AddOrGet<HandSanitizer>();
		handSanitizer.massConsumedPerUse = 5f;
		handSanitizer.consumedElement = SimHashes.Water;
		handSanitizer.outputElement = SimHashes.DirtyWater;
		handSanitizer.diseaseRemovalCount = 120000;
		handSanitizer.maxUses = 40;
		go.AddOrGet<DirectionControl>();
		HandSanitizer.Work work = go.AddOrGet<HandSanitizer.Work>();
		work.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_washbasin_kanim") };
		work.workTime = 5f;
		work.trackUses = true;
		Storage storage = go.AddOrGet<Storage>();
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = GameTagExtensions.Create(SimHashes.Water);
		manualDeliveryKG.minimumMass = 5f;
		manualDeliveryKG.capacity = 200f;
		manualDeliveryKG.refillMass = 40f;
		go.UpdateComponentRequirement<LoopingSounds>(true);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}

	public const string ID = "WashBasin";

	public const int DISEASE_REMOVAL_COUNT = 120000;

	public const float WATER_PER_USE = 5f;

	public const int USES_PER_FLUSH = 40;

	public const float WORK_TIME = 5f;
}
