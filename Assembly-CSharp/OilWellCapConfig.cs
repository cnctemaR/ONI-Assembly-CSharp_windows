using System;
using TUNING;
using UnityEngine;

public class OilWellCapConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		EffectorValues tier = NOISE_POLLUTION.NOISY.TIER2;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("OilWellCap", 4, 4, "geyser_oil_cap_kanim", 200f, 100, 120f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.NONE, tier);
		BuildingTemplates.CreateElectricalBuildingDef(buildingDef);
		buildingDef.SceneLayer = Grid.SceneLayer.BuildingFront;
		buildingDef.ViewMode = SimViewMode.LiquidVentMap;
		buildingDef.EnergyConsumptionWhenActive = 240f;
		buildingDef.OperatingKilowatts = 2f;
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.UtilityInputOffset = new CellOffset(0, 1);
		buildingDef.PowerInputOffset = new CellOffset(1, 1);
		buildingDef.OverheatTemperature = 2273.15f;
		buildingDef.Floodable = false;
		buildingDef.AttachableBuildingType = GameTags.OilWell;
		buildingDef.BuildLocationRule = BuildLocationRule.BuildingAttachPoint;
		buildingDef.ObjectLayer = ObjectLayer.AttachableBuilding;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.UpdateComponentRequirement<LoopingSounds>(true);
		Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
		storage.showInUI = true;
		ConduitConsumer conduitConsumer = go.UpdateComponentRequirement<ConduitConsumer>(true);
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.consumptionRate = 1f;
		conduitConsumer.capacityKG = 10f;
		conduitConsumer.capacityTag = GameTags.Liquid;
		ElementConverter elementConverter = go.UpdateComponentRequirement<ElementConverter>(true);
		elementConverter.conversionInterval = 1f;
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
		{
			new ElementConverter.ConsumedElement(new Tag("Water"), 1f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[]
		{
			new ElementConverter.OutputElement(3.3333333f, SimHashes.CrudeOil, 363.15f, false, 2f, 1.5f, false, 0f, byte.MaxValue, 0)
		};
		OilWellCap oilWellCap = go.UpdateComponentRequirement<OilWellCap>(true);
		oilWellCap.gasElement = SimHashes.Methane;
		oilWellCap.gasTemperature = 573.15f;
		oilWellCap.addGasRate = 0.033333335f;
		oilWellCap.maxGasPressure = 80.00001f;
		oilWellCap.releaseGasRate = 0.44444448f;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}

	private const float WATER_INTAKE_RATE = 1f;

	private const float WATER_TO_OIL_RATIO = 3.3333333f;

	private const float LIQUID_STORAGE = 10f;

	private const float GAS_RATE = 0.033333335f;

	private const float OVERPRESSURE_TIME = 2400f;

	private const float PRESSURE_RELEASE_TIME = 180f;

	private const float PRESSURE_RELEASE_RATE = 0.44444448f;

	public const string ID = "OilWellCap";
}
