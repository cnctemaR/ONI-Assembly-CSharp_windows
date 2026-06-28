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
		float num3 = 400f;
		int num4 = 100;
		float num5 = 480f;
		string[] array = new string[] { "Metal", "Plastic" };
		EffectorValues tier = NOISE_POLLUTION.NOISY.TIER5;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, num5, new float[]
		{
			global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5[0],
			global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2[0]
		}, array, 2400f, BuildLocationRule.OnFloor, global::TUNING.BUILDINGS.DECOR.PENALTY.TIER2, tier);
		buildingDef.GeneratorWattageRating = 2000f;
		buildingDef.GeneratorBaseCapacity = 2000f;
		buildingDef.ExhaustKilowattsWhenActive = 4f;
		buildingDef.OperatingKilowatts = 16f;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.AudioCategory = "Metal";
		buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
		buildingDef.PowerOutputOffset = new CellOffset(1, 0);
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.HotKey = global::Action.BuildMenuKeyT;
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
		go.GetComponent<KPrefabID>().AddPrefabTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<LoopingSounds>();
		go.AddOrGet<Storage>();
		BuildingDef def = go.GetComponent<Building>().Def;
		float num = 30f;
		go.UpdateComponentRequirement<LoopingSounds>(true);
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
				new EnergyGenerator.InputItem(SimHashes.Petroleum.CreateTag(), 3f, num)
			},
			outputs = new EnergyGenerator.OutputItem[]
			{
				new EnergyGenerator.OutputItem(SimHashes.CarbonDioxide, 0.5f, false, new CellOffset(0, 3)),
				new EnergyGenerator.OutputItem(SimHashes.DirtyWater, 1.25f, false, new CellOffset(1, 1))
			}
		};
		BuildingTemplates.DoPostConfigure(go);
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			PoweredActiveController.Instance instance = new PoweredActiveController.Instance(game_object.GetComponent<KPrefabID>());
			instance.StartSM();
		};
	}

	public const string ID = "PetroleumGenerator";

	public const float CONSUMPTION_RATE = 3f;

	private const SimHashes INPUT_ELEMENT = SimHashes.Petroleum;

	private const SimHashes EXHAUST_ELEMENT_GAS = SimHashes.CarbonDioxide;

	private const SimHashes EXHAUST_ELEMENT_LIQUID = SimHashes.DirtyWater;

	public const float EFFICIENCY_RATE = 0.5f;

	public const float EXHAUST_GAS_RATE = 0.5f;

	public const float EXHAUST_LIQUID_RATE = 1.25f;

	private const int WIDTH = 3;

	private const int HEIGHT = 4;

	private static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[]
	{
		new LogicPorts.Port(LogicOperationalController.PORT_ID, new CellOffset(0, 0), UI.LOGIC_PORTS.CONTROL_OPERATIONAL, false)
	};
}
