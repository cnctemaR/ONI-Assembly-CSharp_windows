using System;
using TUNING;
using UnityEngine;

public class AlgaeDistilleryConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("AlgaeDistillery", 3, 4, "algae_distillery_kanim", 100f, 30f, BUILDINGS.CONSTRUCTION_MASS.TIER3, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER1, null);
		buildingDef.ExplosionSize = Overheatable.ExplosionSize.Small;
		buildingDef.RequiresPower = true;
		buildingDef.PowerInputOffset = new CellOffset(1, 0);
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.TemperatureModificationWhenActive = 8f;
		buildingDef.OperatingTemperature = 500f;
		buildingDef.MaterialCategory = MATERIALS.ALL_METALS;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.Upgradeable = false;
		buildingDef.ViewMode = SimViewMode.LiquidVentMap;
		buildingDef.OutputConduitType = ConduitType.Liquid;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		AlgaeDistillery algaeDistillery = go.AddOrGet<AlgaeDistillery>();
		algaeDistillery.hasMeter = true;
		ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
		conduitDispenser.conduitType = ConduitType.Liquid;
		conduitDispenser.elementFilter = SimHashes.DirtyWater;
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.requestedItemTag = new Tag("SlimeMold");
		manualDeliveryKG.refillMass = 100f;
		manualDeliveryKG.capacity = 200f;
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 1000f;
		storage.disableOnStore = true;
		storage.showInUI = true;
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
		{
			new ElementConverter.ConsumedElement(new Tag("SlimeMold"), 30.000002f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[]
		{
			new ElementConverter.OutputElement(null, 10f, SimHashes.Algae, 303.15f, false, 0f, 1f),
			new ElementConverter.OutputElement(null, 10f, SimHashes.DirtyWater, 303.15f, true, 0f, 0f)
		};
		elementConverter.conversionInterval = 100f;
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

	public const float CONVERSION_INTERVAL = 100f;

	public const float INPUT_SLIME_PER_SECOND = 0.3f;

	public const float SLIME_INPUT_MASS = 30.000002f;

	public const float ALGAE_PER_SECOND = 0.1f;

	public const float DIRTY_WATER_RATIO = 1f;

	public const float ALGAE_OUTPUT_MASS = 10f;

	public const float OUTPUT_TEMP = 303.15f;
}
