using System;
using TUNING;
using UnityEngine;

public class HandSanitizerConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "HandSanitizer";
		int num = 2;
		int num2 = 3;
		string text2 = "handsanitizer_kanim";
		float num3 = 50f;
		int num4 = 30;
		float num5 = 30f;
		string[] array = new string[] { "Metal", "BleachStone" };
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, num5, new float[]
		{
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER2[0],
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER1[0]
		}, array, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.BONUS.TIER1, none);
		SoundEventVolumeCache.instance.AddVolume("handsanitizer_kanim", "HandSanitizer_tongue_out", NOISE_POLLUTION.NOISY.TIER0);
		SoundEventVolumeCache.instance.AddVolume("handsanitizer_kanim", "HandSanitizer_tongue_in", NOISE_POLLUTION.NOISY.TIER0);
		SoundEventVolumeCache.instance.AddVolume("handsanitizer_kanim", "HandSanitizer_tongue_slurp", NOISE_POLLUTION.NOISY.TIER0);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.GetComponent<KPrefabID>().AddPrefabTag(RoomConstraints.ConstraintTags.WashStation);
		HandSanitizer handSanitizer = go.AddOrGet<HandSanitizer>();
		handSanitizer.massConsumedPerUse = 0.07f;
		handSanitizer.consumedElement = SimHashes.BleachStone;
		handSanitizer.diseaseRemovalCount = 480000;
		HandSanitizer.Work work = go.AddOrGet<HandSanitizer.Work>();
		work.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_handsanitizer_kanim") };
		work.workTime = 3.5f;
		work.trackUses = true;
		Storage storage = go.AddOrGet<Storage>();
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		go.AddOrGet<DirectionControl>();
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = GameTagExtensions.Create(SimHashes.BleachStone);
		manualDeliveryKG.capacity = 15f;
		manualDeliveryKG.refillMass = 3f;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}

	public const string ID = "HandSanitizer";

	private const float STORAGE_SIZE = 15f;

	private const float MASS_PER_USE = 0.07f;

	private const int DISEASE_REMOVAL_COUNT = 480000;

	private const float WORK_TIME = 3.5f;

	private const SimHashes CONSUMED_ELEMENT = SimHashes.BleachStone;
}
