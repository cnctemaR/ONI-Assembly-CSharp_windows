using System;
using TUNING;
using UnityEngine;

public class FlushToiletConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("FlushToilet", 2, 3, "toiletflush_kanim", 400f, 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.RAW_METALS, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER1, null);
		buildingDef.Overheatable = false;
		buildingDef.ExhaustKilowattsWhenActive = 0.25f;
		buildingDef.OperatingKilowatts = 0f;
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.OutputConduitType = ConduitType.Liquid;
		buildingDef.ViewMode = SimViewMode.LiquidVentMap;
		buildingDef.MaterialCategory = MATERIALS.RAW_METALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(1, 1);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<LoopingSounds>();
		ToiletWorkableUse toiletWorkableUse = go.AddOrGet<ToiletWorkableUse>();
		toiletWorkableUse.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_toiletflush_kanim") };
		FlushToilet flushToilet = go.AddOrGet<FlushToilet>();
		flushToilet.massConsumedPerUse = 5f;
		flushToilet.massEmittedPerUse = 11.7f;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.Water).tag;
		conduitConsumer.capacityKG = 5f;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Store;
		ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
		conduitDispenser.conduitType = ConduitType.Liquid;
		conduitDispenser.invertElementFilter = true;
		conduitDispenser.elementFilter = new SimHashes[] { SimHashes.Water };
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 25f;
		storage.disableOnStore = true;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}

	private const float WATER_USAGE = 5f;
}
