using System;
using TUNING;
using UnityEngine;

public class OuthouseConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("Outhouse", 2, 2, "outhouse_kanim", 200f, 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.RAW_MINERALS, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER4, null);
		buildingDef.Overheatable = false;
		buildingDef.ExhaustKilowattsWhenActive = 0.25f;
		buildingDef.MaterialCategory = MATERIALS.RAW_MINERALS;
		buildingDef.AudioCategory = "Metal";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<LoopingSounds>();
		Toilet toilet = go.AddOrGet<Toilet>();
		toilet.maxFlushes = 15;
		toilet.solidWastePerUse = new Toilet.SpawnInfo(SimHashes.ToxicSand, 6.7f, 0f);
		toilet.gasWasteWhenFull = new Toilet.SpawnInfo(SimHashes.ContaminatedOxygen, 0.1f, 15f);
		KAnimFile[] array = new KAnimFile[] { Assets.GetAnim("anim_interacts_outhouse_kanim") };
		ToiletWorkableUse toiletWorkableUse = go.AddOrGet<ToiletWorkableUse>();
		toiletWorkableUse.overrideAnims = array;
		ToiletWorkableClean toiletWorkableClean = go.AddOrGet<ToiletWorkableClean>();
		toiletWorkableClean.workTime = 90f;
		toiletWorkableClean.overrideAnims = array;
		Storage storage = go.AddOrGet<Storage>();
		storage.disableOnStore = true;
		storage.showInUI = true;
		Storage storage2 = go.AddComponent<Storage>();
		storage2.capacityKg = 20000f;
		storage2.disableOnStore = true;
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
}
