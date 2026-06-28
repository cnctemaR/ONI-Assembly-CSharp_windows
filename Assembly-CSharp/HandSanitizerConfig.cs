using System;
using TUNING;
using UnityEngine;

public class HandSanitizerConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string[] array = new string[] { "Metal", "BleachStone" };
		return BuildingTemplates.CreateBuildingDef("HandSanitizer", 2, 2, "handsanitizer_kanim", 50f, 30, 30f, new float[]
		{
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER2[0],
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER1[0]
		}, array, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.BONUS.TIER1, null);
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		HandSanitizer handSanitizer = go.AddOrGet<HandSanitizer>();
		handSanitizer.massConsumedPerUse = 1f;
		HandSanitizer.Workable workable = go.AddOrGet<HandSanitizer.Workable>();
		workable.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_handsanitizer_kanim") };
		workable.workTime = 5.4f;
		Storage storage = go.AddOrGet<Storage>();
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = TagManager.Create(SimHashes.BleachStone);
		manualDeliveryKG.capacity = 100f;
		manualDeliveryKG.refillMass = 10f;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}
}
