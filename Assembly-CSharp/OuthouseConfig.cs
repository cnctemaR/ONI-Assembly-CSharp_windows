using System;
using TUNING;
using UnityEngine;

public class OuthouseConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "Outhouse";
		int num = 2;
		int num2 = 3;
		string text2 = "outhouse_kanim";
		float num3 = 200f;
		int num4 = 30;
		float num5 = 30f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] raw_MINERALS = MATERIALS.RAW_MINERALS;
		float num6 = 800f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, num5, tier, raw_MINERALS, num6, buildLocationRule, BUILDINGS.DECOR.PENALTY.TIER4, none);
		buildingDef.Overheatable = false;
		buildingDef.ExhaustKilowattsWhenActive = 0.25f;
		buildingDef.DiseaseCellVisName = "FoodPoisoning";
		buildingDef.MaterialCategory = MATERIALS.RAW_MINERALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.HotKey = global::Action.BuildMenuKeyT;
		SoundEventVolumeCache.instance.AddVolume("outhouse_kanim", "Latrine_door_open", NOISE_POLLUTION.NOISY.TIER1);
		SoundEventVolumeCache.instance.AddVolume("outhouse_kanim", "Latrine_door_close", NOISE_POLLUTION.NOISY.TIER1);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddPrefabTag(RoomConstraints.ConstraintTags.Toilet);
		Toilet toilet = go.AddOrGet<Toilet>();
		toilet.maxFlushes = 15;
		toilet.solidWastePerUse = new Toilet.SpawnInfo(SimHashes.ToxicSand, 6.7f, 0f);
		toilet.gasWasteWhenFull = new Toilet.SpawnInfo(SimHashes.ContaminatedOxygen, 0.1f, 15f);
		toilet.diseaseId = "FoodPoisoning";
		toilet.diseasePerFlush = 100000;
		toilet.diseaseOnDupePerFlush = 100000;
		KAnimFile[] array = new KAnimFile[] { Assets.GetAnim("anim_interacts_outhouse_kanim") };
		ToiletWorkableUse toiletWorkableUse = go.AddOrGet<ToiletWorkableUse>();
		toiletWorkableUse.overrideAnims = array;
		toiletWorkableUse.workLayer = Grid.SceneLayer.BuildingFront;
		toiletWorkableUse.canBePublic = true;
		ToiletWorkableClean toiletWorkableClean = go.AddOrGet<ToiletWorkableClean>();
		toiletWorkableClean.workTime = 90f;
		toiletWorkableClean.overrideAnims = array;
		Storage storage = go.AddOrGet<Storage>();
		storage.showInUI = true;
		Storage storage2 = go.AddComponent<Storage>();
		storage2.capacityKg = 20000f;
		storage2.showInUI = true;
		storage2.allowItemRemoval = true;
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = new Tag("Dirt");
		manualDeliveryKG.capacity = 200f;
		manualDeliveryKG.refillMass = 25f;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}

	public const string ID = "Outhouse";
}
