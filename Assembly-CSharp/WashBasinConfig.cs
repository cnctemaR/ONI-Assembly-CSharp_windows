using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class WashBasinConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string[] raw_MINERALS = MATERIALS.RAW_MINERALS;
		EffectorValues tier = NOISE_POLLUTION.NOISY.TIER0;
		return BuildingTemplates.CreateBuildingDef("WashBasin", 2, 3, "wash_basin_kanim", 50f, 30, 30f, new float[] { BUILDINGS.CONSTRUCTION_MASS_KG.TIER1[0] }, raw_MINERALS, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.BONUS.TIER1, tier);
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		HandSanitizer handSanitizer = go.AddOrGet<HandSanitizer>();
		handSanitizer.massConsumedPerUse = 5f;
		handSanitizer.consumedElement = SimHashes.Water;
		handSanitizer.outputElement = SimHashes.DirtyWater;
		handSanitizer.diseaseRemovalCount = 120000;
		handSanitizer.maxUses = 40;
		go.AddOrGet<DirectionControl>();
		HandSanitizer.Work work = go.AddOrGet<HandSanitizer.Work>();
		work.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_washbasin_kanim") };
		work.workTime = 10.200001f;
		work.trackUses = true;
		Storage storage = go.AddOrGet<Storage>();
		storage.defaultStoredItemModifers = WashBasinConfig.StoredItemModifiers;
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

	public const int DISEASE_REMOVAL_COUNT = 120000;

	public const float WATER_PER_USE = 5f;

	public const int USES_PER_FLUSH = 40;

	private static readonly List<Storage.StoredItemModifier> StoredItemModifiers = new List<Storage.StoredItemModifier>
	{
		Storage.StoredItemModifier.Hide,
		Storage.StoredItemModifier.Seal
	};
}
