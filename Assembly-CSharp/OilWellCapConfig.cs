using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class OilWellCapConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "OilWellCap";
		int num = 4;
		int num2 = 4;
		string text2 = "geyser_oil_cap_kanim";
		int num3 = 100;
		float num4 = 120f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER2;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, refined_METALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.NONE, tier2, 0.2f);
		BuildingTemplates.CreateElectricalBuildingDef(buildingDef);
		buildingDef.SceneLayer = Grid.SceneLayer.BuildingFront;
		buildingDef.ViewMode = SimViewMode.LiquidVentMap;
		buildingDef.EnergyConsumptionWhenActive = 240f;
		buildingDef.SelfHeatKilowattsWhenActive = 2f;
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.UtilityInputOffset = new CellOffset(0, 1);
		buildingDef.PowerInputOffset = new CellOffset(1, 1);
		buildingDef.OverheatTemperature = 2273.15f;
		buildingDef.Floodable = false;
		buildingDef.AttachmentSlotTag = GameTags.OilWell;
		buildingDef.BuildLocationRule = BuildLocationRule.BuildingAttachPoint;
		buildingDef.ObjectLayer = ObjectLayer.AttachableBuilding;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
		storage.showInUI = true;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.consumptionRate = 1f;
		conduitConsumer.capacityKG = 10f;
		conduitConsumer.capacityTag = GameTags.Liquid;
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
		{
			new ElementConverter.ConsumedElement(new Tag("Water"), 1f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[]
		{
			new ElementConverter.OutputElement(3.3333333f, SimHashes.CrudeOil, 363.15f, false, 2f, 1.5f, false, 0f, byte.MaxValue, 0)
		};
		OilWellCap oilWellCap = go.AddOrGet<OilWellCap>();
		oilWellCap.gasElement = SimHashes.Methane;
		oilWellCap.gasTemperature = 573.15f;
		oilWellCap.addGasRate = 0.033333335f;
		oilWellCap.maxGasPressure = 80.00001f;
		oilWellCap.releaseGasRate = 0.44444448f;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, OilWellCapConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, OilWellCapConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		GeneratedBuildings.RegisterLogicPorts(go, OilWellCapConfig.INPUT_PORTS);
		go.AddOrGet<LogicOperationalController>();
	}

	private const float WATER_INTAKE_RATE = 1f;

	private const float WATER_TO_OIL_RATIO = 3.3333333f;

	private const float LIQUID_STORAGE = 10f;

	private const float GAS_RATE = 0.033333335f;

	private const float OVERPRESSURE_TIME = 2400f;

	private const float PRESSURE_RELEASE_TIME = 180f;

	private const float PRESSURE_RELEASE_RATE = 0.44444448f;

	public const string ID = "OilWellCap";

	private static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[] { LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(0, 0), UI.LOGIC_PORTS.CONTROL_OPERATIONAL, false) };
}
