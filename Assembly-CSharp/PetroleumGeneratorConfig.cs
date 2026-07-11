using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class PetroleumGeneratorConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "PetroleumGenerator";
		int num = 3;
		int num2 = 4;
		string text2 = "generatorpetrol_kanim";
		int num3 = 100;
		float num4 = 480f;
		string[] array = new string[] { "Metal", "Plastic" };
		EffectorValues tier = NOISE_POLLUTION.NOISY.TIER5;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, new float[]
		{
			global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5[0],
			global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2[0]
		}, array, 2400f, BuildLocationRule.OnFloor, global::TUNING.BUILDINGS.DECOR.PENALTY.TIER2, tier, 0.2f);
		buildingDef.GeneratorWattageRating = 2000f;
		buildingDef.GeneratorBaseCapacity = 2000f;
		buildingDef.ExhaustKilowattsWhenActive = 4f;
		buildingDef.SelfHeatKilowattsWhenActive = 16f;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
		buildingDef.PowerOutputOffset = new CellOffset(1, 0);
		buildingDef.InputConduitType = ConduitType.Liquid;
		return buildingDef;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, PetroleumGeneratorConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, PetroleumGeneratorConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, PetroleumGeneratorConfig.INPUT_PORTS);
		go.AddOrGet<LogicOperationalController>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<LoopingSounds>();
		go.AddOrGet<Storage>();
		BuildingDef def = go.GetComponent<Building>().Def;
		float num = 20f;
		go.AddOrGet<LoopingSounds>();
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = def.InputConduitType;
		conduitConsumer.consumptionRate = 10f;
		conduitConsumer.capacityTag = SimHashes.Petroleum.CreateTag();
		conduitConsumer.capacityKG = num;
		conduitConsumer.forceAlwaysSatisfied = true;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		EnergyGenerator energyGenerator = go.AddOrGet<EnergyGenerator>();
		energyGenerator.powerDistributionOrder = 8;
		energyGenerator.ignoreBatteryRefillPercent = true;
		energyGenerator.hasMeter = true;
		energyGenerator.formula = new EnergyGenerator.Formula
		{
			inputs = new EnergyGenerator.InputItem[]
			{
				new EnergyGenerator.InputItem(SimHashes.Petroleum.CreateTag(), 2f, num)
			},
			outputs = new EnergyGenerator.OutputItem[]
			{
				new EnergyGenerator.OutputItem(SimHashes.CarbonDioxide, 0.5f, false, new CellOffset(0, 3), 0f),
				new EnergyGenerator.OutputItem(SimHashes.DirtyWater, 0.75f, false, new CellOffset(1, 1), 0f)
			}
		};
		Tinkerable.MakePowerTinkerable(go);
		go.AddOrGetDef<PoweredActiveController.Def>();
	}

	public const string ID = "PetroleumGenerator";

	public const float CONSUMPTION_RATE = 2f;

	private const SimHashes INPUT_ELEMENT = SimHashes.Petroleum;

	private const SimHashes EXHAUST_ELEMENT_GAS = SimHashes.CarbonDioxide;

	private const SimHashes EXHAUST_ELEMENT_LIQUID = SimHashes.DirtyWater;

	public const float EFFICIENCY_RATE = 0.5f;

	public const float EXHAUST_GAS_RATE = 0.5f;

	public const float EXHAUST_LIQUID_RATE = 0.75f;

	private const int WIDTH = 3;

	private const int HEIGHT = 4;

	private static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[] { LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(0, 0), UI.LOGIC_PORTS.CONTROL_OPERATIONAL, false) };
}
