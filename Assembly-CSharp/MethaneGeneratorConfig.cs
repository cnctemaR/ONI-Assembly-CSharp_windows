using System;
using TUNING;
using UnityEngine;

public class MethaneGeneratorConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("MethaneGenerator", 4, 3, "generatormethane_kanim", 400f, 100, 120f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER5, MATERIALS.RAW_METALS, 2400f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER2, null);
		buildingDef.GeneratorWattageRating = 800f;
		buildingDef.GeneratorBaseCapacity = 1000f;
		buildingDef.ExhaustKilowattsWhenActive = 2f;
		buildingDef.OperatingKilowatts = 2f;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.AudioCategory = "Metal";
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(2, 2);
		buildingDef.PowerOutputOffset = new CellOffset(0, 0);
		buildingDef.InputConduitType = ConduitType.Gas;
		buildingDef.OutputConduitType = ConduitType.Gas;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<LoopingSounds>();
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 50f;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Gas;
		conduitConsumer.consumptionRate = 0.59999996f;
		conduitConsumer.capacityTag = TagManager.Create(SimHashes.Methane);
		conduitConsumer.capacityKG = 0.59999996f;
		conduitConsumer.forceAlwaysSatisfied = true;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		float num = 0.00375f;
		EnergyGenerator energyGenerator = go.AddOrGet<EnergyGenerator>();
		energyGenerator.powerDistributionOrder = 8;
		EnergyGenerator.Formula formula = default(EnergyGenerator.Formula);
		formula.inputs = new EnergyGenerator.InputItem[]
		{
			new EnergyGenerator.InputItem(TagManager.Create(SimHashes.Methane), 0.06f, 0.59999996f)
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

	public const string ID = "MethaneGenerator";

	public const float METHANE_CONSUMPTION_RATE = 0.06f;

	private const int WIDTH = 4;

	private const int HEIGHT = 3;
}
