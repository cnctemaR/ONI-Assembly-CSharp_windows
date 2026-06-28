using System;
using TUNING;
using UnityEngine;

public class ShowerConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("Shower", 2, 4, "shower_kanim", 400f, 30f, BUILDINGS.CONSTRUCTION_MASS.TIER4, MATERIALS.RAW_METALS, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.BONUS.TIER1, null);
		buildingDef.TemperatureModificationWhenActive = 4f;
		buildingDef.OperatingTemperature = 350f;
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.OutputConduitType = ConduitType.Liquid;
		buildingDef.ViewMode = SimViewMode.LiquidVentMap;
		buildingDef.MaterialCategory = MATERIALS.RAW_METALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<LoopingSounds>();
		Shower shower = go.AddOrGet<Shower>();
		shower.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_shower_kanim") };
		shower.workTime = 45f;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
		conduitDispenser.conduitType = ConduitType.Liquid;
		conduitDispenser.elementFilter = SimHashes.DirtyWater;
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
		{
			new ElementConverter.ConsumedElement(new Tag("Water"), 1f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[]
		{
			new ElementConverter.OutputElement(null, 1f, SimHashes.DirtyWater, 0f, true, 0f, 0f)
		};
		elementConverter.conversionInterval = 1f;
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 5f;
		storage.disableOnStore = true;
	}

	public override void DoPostConfigure(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}
}
