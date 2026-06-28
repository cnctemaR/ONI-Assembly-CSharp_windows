using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class AlgaeDistilleryConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("AlgaeDistillery", 3, 4, "algae_distillery_kanim", 100f, 100, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER1, null);
		buildingDef.Overheatable = false;
		buildingDef.RequiresPowerInput = true;
		buildingDef.PowerInputOffset = new CellOffset(1, 0);
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.ExhaustKilowattsWhenActive = 0.5f;
		buildingDef.OperatingKilowatts = 1f;
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
		algaeDistillery.emitTag = new Tag("Algae");
		algaeDistillery.emitMass = 30f;
		ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
		conduitDispenser.conduitType = ConduitType.Liquid;
		conduitDispenser.elementFilter = new SimHashes[] { SimHashes.DirtyWater };
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 1000f;
		storage.disableOnStore = true;
		storage.defaultStoredItemModifers = AlgaeDistilleryConfig.AlgaeDistilleryStoredItemModifiers;
		storage.showInUI = true;
		Tag tag = new Tag("SlimeMold");
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = tag;
		manualDeliveryKG.refillMass = 100f;
		manualDeliveryKG.capacity = 200f;
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
		{
			new ElementConverter.ConsumedElement(tag, 0.6f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[]
		{
			new ElementConverter.OutputElement(0.2f, SimHashes.Algae, 303.15f, true, 0f, 1f, false),
			new ElementConverter.OutputElement(0.4f, SimHashes.DirtyWater, 303.15f, true, 0f, 0f, false)
		};
		elementConverter.conversionInterval = 1f;
		go.AddOrGet<Prioritizable>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			PoweredActiveController.Instance instance = new PoweredActiveController.Instance(game_object.GetComponent<KPrefabID>());
			instance.StartSM();
		};
	}

	public const float INPUT_SLIME_PER_SECOND = 0.6f;

	public const float ALGAE_PER_SECOND = 0.2f;

	public const float DIRTY_WATER_RATIO = 2f;

	public const float OUTPUT_TEMP = 303.15f;

	private static readonly List<Storage.StoredItemModifier> AlgaeDistilleryStoredItemModifiers = new List<Storage.StoredItemModifier>
	{
		Storage.StoredItemModifier.Hide,
		Storage.StoredItemModifier.Seal
	};
}
