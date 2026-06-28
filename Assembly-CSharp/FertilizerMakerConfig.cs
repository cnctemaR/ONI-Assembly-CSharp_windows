using System;
using TUNING;
using UnityEngine;

public class FertilizerMakerConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("FertilizerMaker", 4, 3, "fertilizer_maker_kanim", 100f, 30f, BUILDINGS.CONSTRUCTION_MASS.TIER3, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER2, null);
		buildingDef.ExplosionSize = Overheatable.ExplosionSize.Small;
		buildingDef.RequiresPower = true;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.TemperatureModificationWhenActive = 4f;
		buildingDef.OperatingTemperature = 400f;
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.ViewMode = SimViewMode.LiquidVentMap;
		buildingDef.MaterialCategory = MATERIALS.ALL_METALS;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.PowerInputOffset = new CellOffset(1, 0);
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		BuildingTemplates.CreateDefaultStorage(go, false);
		go.AddOrGet<WaterPurifier>();
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
		{
			new ElementConverter.ConsumedElement(new Tag("DirtyWater"), 40f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[]
		{
			new ElementConverter.OutputElement(null, 20f, SimHashes.Fertilizer, 323.15f, false, 0f, 1f)
		};
		elementConverter.conversionInterval = 30f;
		ManualDeliveryKG manualDeliveryKG = go.AddComponent<ManualDeliveryKG>();
		manualDeliveryKG.requestedItemTag = new Tag("DirtyWater");
		manualDeliveryKG.capacity = 400f;
		manualDeliveryKG.refillMass = 50f;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.consumptionRate = 10f;
		go.AddOrGet<Prioritizable>();
	}

	public override void DoPostConfigure(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			PoweredActiveController.Instance instance = new PoweredActiveController.Instance(game_object.GetComponent<KPrefabID>());
			instance.StartSM();
		};
	}

	private const float FERTILIZER_PER_CYCLE = 400f;

	private const float LOADS_PER_CYCLE = 20f;

	private const float FERTILIZER_PER_LOAD = 20f;

	private const float WATER_PER_LOAD = 40f;
}
