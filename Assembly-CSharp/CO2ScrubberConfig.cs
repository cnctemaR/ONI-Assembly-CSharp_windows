using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class CO2ScrubberConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "CO2Scrubber";
		int num = 2;
		int num2 = 2;
		string text2 = "co2scrubber_kanim";
		int num3 = 30;
		float num4 = 30f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
		string[] raw_METALS = MATERIALS.RAW_METALS;
		float num5 = 800f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER3;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, raw_METALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.PENALTY.TIER1, tier2, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.SelfHeatKilowattsWhenActive = 1f;
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.OutputConduitType = ConduitType.Liquid;
		buildingDef.ViewMode = OverlayModes.Oxygen.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "large";
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(1, 1);
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
		storage.showInUI = true;
		storage.capacityKg = 30000f;
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		AirFilter airFilter = go.AddOrGet<AirFilter>();
		airFilter.filterTag = GameTagExtensions.Create(SimHashes.Water);
		ElementConsumer elementConsumer = go.AddOrGet<PassiveElementConsumer>();
		elementConsumer.elementToConsume = SimHashes.CarbonDioxide;
		elementConsumer.consumptionRate = 0.6f;
		elementConsumer.capacityKG = 0.6f;
		elementConsumer.consumptionRadius = 3;
		elementConsumer.showInStatusPanel = true;
		elementConsumer.sampleCellOffset = new Vector3(0f, 0f, 0f);
		elementConsumer.isRequired = false;
		elementConsumer.storeOnConsume = true;
		elementConsumer.showDescriptor = false;
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
		{
			new ElementConverter.ConsumedElement(GameTagExtensions.Create(SimHashes.Water), 1f),
			new ElementConverter.ConsumedElement(GameTagExtensions.Create(SimHashes.CarbonDioxide), 0.3f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[]
		{
			new ElementConverter.OutputElement(1f, SimHashes.DirtyWater, 313.15f, true, 0f, 0.5f, false, 1f, byte.MaxValue, 0)
		};
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.consumptionRate = 2f;
		conduitConsumer.capacityKG = 2f;
		conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.Water).tag;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Store;
		conduitConsumer.forceAlwaysSatisfied = true;
		ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
		conduitDispenser.conduitType = ConduitType.Liquid;
		conduitDispenser.invertElementFilter = true;
		conduitDispenser.elementFilter = new SimHashes[] { SimHashes.Water };
		go.AddOrGet<KBatchedAnimController>().randomiseLoopedOffset = true;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, CO2ScrubberConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, CO2ScrubberConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, CO2ScrubberConfig.INPUT_PORTS);
		go.AddOrGet<LogicOperationalController>();
		go.AddOrGetDef<PoweredActiveController.Def>();
	}

	public const string ID = "CO2Scrubber";

	private const float CO2_CONSUMPTION_RATE = 0.3f;

	private const float H2O_CONSUMPTION_RATE = 1f;

	private static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[] { LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(1, 0), UI.LOGIC_PORTS.CONTROL_OPERATIONAL, false) };
}
