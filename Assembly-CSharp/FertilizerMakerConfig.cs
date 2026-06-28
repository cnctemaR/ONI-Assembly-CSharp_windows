using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class FertilizerMakerConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		EffectorValues tier = NOISE_POLLUTION.NOISY.TIER5;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("FertilizerMaker", 4, 3, "fertilizer_maker_kanim", 100f, 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER2, tier);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.ExhaustKilowattsWhenActive = 0.25f;
		buildingDef.OperatingKilowatts = 0.5f;
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
		go.GetComponent<KPrefabID>().AddPrefabTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
		storage.defaultStoredItemModifers = FertilizerMakerConfig.StoredItemModifiers;
		go.AddOrGet<WaterPurifier>();
		ElementDropper elementDropper = go.AddComponent<ElementDropper>();
		elementDropper.emitMass = FertilizerMakerConfig.FERTILIZER_PER_LOAD;
		elementDropper.emitTag = new Tag("Fertilizer");
		elementDropper.emitOffset = new Vector3(0f, 1f, 0f);
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.conversionInterval = 1f;
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
		{
			new ElementConverter.ConsumedElement(new Tag("DirtyWater"), FertilizerMakerConfig.WATER_PER_CYCLE / 600f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[]
		{
			new ElementConverter.OutputElement(FertilizerMakerConfig.FERTILIZER_PER_CYCLE / 600f, SimHashes.Fertilizer, 323.15f, true, 0f, 0.5f, false, 1f, byte.MaxValue, 0)
		};
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.consumptionRate = 10f;
		conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.DirtyWater).tag;
		conduitConsumer.capacityKG = FertilizerMakerConfig.WATER_PER_CYCLE * 5f;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		conduitConsumer.forceAlwaysSatisfied = true;
		BuildingElementEmitter buildingElementEmitter = go.AddOrGet<BuildingElementEmitter>();
		buildingElementEmitter.emitRate = 0.02f;
		buildingElementEmitter.temperature = 303f;
		buildingElementEmitter.element = SimHashes.Methane;
		buildingElementEmitter.modifierOffset = new Vector2(2f, 2f);
		Prioritizable.AddRef(go);
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

	public const float METHANE_EMIT_RATE = 0.02f;

	private static float _NUM_PLANTS_PER_DUPE = (float)Math.Ceiling(2.880000114440918);

	private static float _NUM_DUPES = 6f;

	private static float _PLANTS_FED = FertilizerMakerConfig._NUM_DUPES * FertilizerMakerConfig._NUM_PLANTS_PER_DUPE;

	private static float FERTILIZER_PER_LOAD = 4f;

	private static float FERTILIZER_PER_CYCLE = FertilizerMakerConfig._PLANTS_FED * FertilizerMakerConfig.FERTILIZER_PER_LOAD;

	private static float WATER_PER_CYCLE = FertilizerMakerConfig.FERTILIZER_PER_CYCLE / 0.8f;

	private static readonly List<Storage.StoredItemModifier> StoredItemModifiers = new List<Storage.StoredItemModifier>
	{
		Storage.StoredItemModifier.Hide,
		Storage.StoredItemModifier.Seal
	};
}
