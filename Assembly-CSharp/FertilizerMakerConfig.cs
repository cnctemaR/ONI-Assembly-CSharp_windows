using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class FertilizerMakerConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "FertilizerMaker";
		int num = 4;
		int num2 = 3;
		string text2 = "fertilizer_maker_kanim";
		int num3 = 30;
		float num4 = 30f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num5 = 800f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER5;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, all_METALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.PENALTY.TIER2, tier2, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.ExhaustKilowattsWhenActive = 0.25f;
		buildingDef.SelfHeatKilowattsWhenActive = 0.5f;
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.ViewMode = SimViewMode.LiquidVentMap;
		buildingDef.MaterialCategory = MATERIALS.ALL_METALS;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.PowerInputOffset = new CellOffset(1, 0);
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddPrefabTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		go.AddOrGet<WaterPurifier>();
		ElementDropper elementDropper = go.AddComponent<ElementDropper>();
		elementDropper.emitMass = 4f;
		elementDropper.emitTag = new Tag("Fertilizer");
		elementDropper.emitOffset = new Vector3(0f, 1f, 0f);
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
		{
			new ElementConverter.ConsumedElement(new Tag("DirtyWater"), 0.15f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[]
		{
			new ElementConverter.OutputElement(0.12f, SimHashes.Fertilizer, 323.15f, true, 0f, 0.5f, false, 1f, byte.MaxValue, 0)
		};
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.consumptionRate = 10f;
		conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.DirtyWater).tag;
		conduitConsumer.capacityKG = 0.75f;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		conduitConsumer.forceAlwaysSatisfied = true;
		BuildingElementEmitter buildingElementEmitter = go.AddOrGet<BuildingElementEmitter>();
		buildingElementEmitter.emitRate = 0.02f;
		buildingElementEmitter.temperature = 303f;
		buildingElementEmitter.element = SimHashes.Methane;
		buildingElementEmitter.modifierOffset = new Vector2(2f, 2f);
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, FertilizerMakerConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, FertilizerMakerConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		GeneratedBuildings.RegisterLogicPorts(go, FertilizerMakerConfig.INPUT_PORTS);
		go.AddOrGet<LogicOperationalController>();
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			PoweredActiveController.Instance instance = new PoweredActiveController.Instance(game_object.GetComponent<KPrefabID>());
			instance.StartSM();
		};
	}

	public const string ID = "FertilizerMaker";

	public const float METHANE_EMIT_RATE = 0.02f;

	private const float _NUM_PLANTS_PER_DUPE = 3f;

	private const float _NUM_DUPES = 6f;

	private const float _PLANTS_FED = 18f;

	private const float FERTILIZER_PER_LOAD = 4f;

	private const float FERTILIZER_PER_CYCLE = 72f;

	private const float WATER_PER_CYCLE = 90f;

	private static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[] { LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(-1, 0), UI.LOGIC_PORTS.CONTROL_OPERATIONAL, false) };
}
