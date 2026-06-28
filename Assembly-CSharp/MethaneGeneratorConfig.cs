using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class MethaneGeneratorConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "MethaneGenerator";
		int num = 4;
		int num2 = 3;
		string text2 = "generatormethane_kanim";
		int num3 = 100;
		float num4 = 120f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
		string[] raw_METALS = MATERIALS.RAW_METALS;
		float num5 = 2400f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER5;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, raw_METALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.PENALTY.TIER2, tier2, 0.2f);
		buildingDef.GeneratorWattageRating = 800f;
		buildingDef.GeneratorBaseCapacity = 1000f;
		buildingDef.ExhaustKilowattsWhenActive = 2f;
		buildingDef.SelfHeatKilowattsWhenActive = 2f;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.AudioCategory = "Metal";
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(2, 2);
		buildingDef.PowerOutputOffset = new CellOffset(0, 0);
		buildingDef.InputConduitType = ConduitType.Gas;
		buildingDef.OutputConduitType = ConduitType.Gas;
		return buildingDef;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, MethaneGeneratorConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, MethaneGeneratorConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, MethaneGeneratorConfig.INPUT_PORTS);
		go.AddOrGet<LogicOperationalController>();
		go.GetComponent<KPrefabID>().AddPrefabTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<LoopingSounds>();
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 50f;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Gas;
		conduitConsumer.consumptionRate = 0.59999996f;
		conduitConsumer.capacityTag = GameTagExtensions.Create(SimHashes.Methane);
		conduitConsumer.capacityKG = 0.59999996f;
		conduitConsumer.forceAlwaysSatisfied = true;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		float num = 0.00375f;
		EnergyGenerator energyGenerator = go.AddOrGet<EnergyGenerator>();
		energyGenerator.powerDistributionOrder = 8;
		energyGenerator.ignoreBatteryRefillPercent = true;
		EnergyGenerator.Formula formula = default(EnergyGenerator.Formula);
		formula.inputs = new EnergyGenerator.InputItem[]
		{
			new EnergyGenerator.InputItem(GameTagExtensions.Create(SimHashes.Methane), 0.06f, 0.59999996f)
		};
		float num2 = num * 2f * 18f;
		float num3 = num * 44f;
		float num4 = 0.5f;
		formula.outputs = new EnergyGenerator.OutputItem[]
		{
			new EnergyGenerator.OutputItem(SimHashes.DirtyWater, num2 * num4, false, new CellOffset(1, 1)),
			new EnergyGenerator.OutputItem(SimHashes.CarbonDioxide, num3 * num4, true)
		};
		energyGenerator.formula = formula;
		ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
		conduitDispenser.conduitType = ConduitType.Gas;
		conduitDispenser.invertElementFilter = true;
		conduitDispenser.elementFilter = new SimHashes[]
		{
			SimHashes.Methane,
			SimHashes.Oxygen
		};
		Tinkerable.MakePowerTinkerable(go);
		BuildingTemplates.DoPostConfigure(go);
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			PoweredActiveController.Instance instance = new PoweredActiveController.Instance(game_object.GetComponent<KPrefabID>());
			instance.StartSM();
		};
	}

	public const string ID = "MethaneGenerator";

	public const float METHANE_CONSUMPTION_RATE = 0.06f;

	private const int WIDTH = 4;

	private const int HEIGHT = 3;

	private static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[] { LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(0, 0), UI.LOGIC_PORTS.CONTROL_OPERATIONAL, false) };
}
