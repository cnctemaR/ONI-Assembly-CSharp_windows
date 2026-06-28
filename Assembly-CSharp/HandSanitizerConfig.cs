using System;
using TUNING;
using UnityEngine;

public class HandSanitizerConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string[] array = new string[] { "Metal", "BleachStone" };
		return BuildingTemplates.CreateBuildingDef("HandSanitizer", 2, 2, "handsanitizer_kanim", 50f, 30f, new float[]
		{
			BUILDINGS.CONSTRUCTION_MASS.TIER2[0],
			BUILDINGS.CONSTRUCTION_MASS.TIER1[0]
		}, array, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.BONUS.TIER1, null);
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		HandSanitizer handSanitizer = go.AddOrGet<HandSanitizer>();
		handSanitizer.massConsumedPerUse = 1f;
		HandSanitizer.Workable workable = go.AddOrGet<HandSanitizer.Workable>();
		workable.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_handsanitizer_kanim") };
		workable.workTime = 5.4f;
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.requestedItemTag = TagManager.Create(SimHashes.BleachStone);
		manualDeliveryKG.capacity = 100f;
		manualDeliveryKG.refillMass = 10f;
		go.AddOrGet<Storage>();
	}

	public override void DoPostConfigure(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}
}
