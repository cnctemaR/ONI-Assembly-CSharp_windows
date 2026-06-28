using System;
using TUNING;
using UnityEngine;

public class HydrogenGeneratorConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("HydrogenGenerator", 4, 3, "generatormerc_kanim", 400f, 100, 120f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER5, MATERIALS.RAW_METALS, 2400f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER2, null);
		buildingDef.GeneratorWattageRating = 800f;
		buildingDef.GeneratorBaseCapacity = 1000f;
		buildingDef.ExhaustKilowattsWhenActive = 2f;
		buildingDef.OperatingKilowatts = 2f;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.AudioCategory = "Metal";
		buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
		buildingDef.PowerOutputOffset = new CellOffset(1, 0);
		buildingDef.InputConduitType = ConduitType.Gas;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<LoopingSounds>();
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 2f;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Gas;
		conduitConsumer.consumptionRate = 1f;
		conduitConsumer.capacityTag = TagManager.Create(SimHashes.Hydrogen);
		conduitConsumer.capacityKG = 2f;
		conduitConsumer.forceAlwaysSatisfied = true;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		EnergyGenerator energyGenerator = go.AddOrGet<EnergyGenerator>();
		energyGenerator.formula = EnergyGenerator.CreateSimpleFormula(SimHashes.Hydrogen, 0.1f, 2f, SimHashes.Void, 0f, true);
		energyGenerator.powerDistributionOrder = 8;
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
}
