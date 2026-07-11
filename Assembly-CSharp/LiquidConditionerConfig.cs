using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class LiquidConditionerConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "LiquidConditioner";
		int num = 2;
		int num2 = 2;
		string text2 = "liquidconditioner_kanim";
		int num3 = 100;
		float num4 = 120f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER6;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER2;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, all_METALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.NONE, tier2, 0.2f);
		BuildingTemplates.CreateElectricalBuildingDef(buildingDef);
		buildingDef.EnergyConsumptionWhenActive = 1200f;
		buildingDef.SelfHeatKilowattsWhenActive = 0f;
		buildingDef.Invincible = true;
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.OutputConduitType = ConduitType.Liquid;
		buildingDef.Floodable = false;
		buildingDef.PowerInputOffset = new CellOffset(1, 0);
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		buildingDef.ViewMode = SimViewMode.LiquidVentMap;
		buildingDef.OverheatTemperature = 398.15f;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		AirConditioner airConditioner = go.AddOrGet<AirConditioner>();
		airConditioner.temperatureDelta = -14f;
		airConditioner.maxEnvironmentDelta = -50f;
		airConditioner.isLiquidConditioner = true;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.consumptionRate = 10f;
		Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
		storage.showInUI = true;
		storage.capacityKg = 2f * conduitConsumer.consumptionRate;
		storage.SetDefaultStoredItemModifiers(LiquidConditionerConfig.StoredItemModifiers);
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, LiquidConditionerConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, LiquidConditionerConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		GeneratedBuildings.RegisterLogicPorts(go, LiquidConditionerConfig.INPUT_PORTS);
		go.AddOrGet<LogicOperationalController>();
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			PoweredActiveController.Instance instance = new PoweredActiveController.Instance(game_object.GetComponent<KPrefabID>());
			instance.StartSM();
		};
	}

	public const string ID = "LiquidConditioner";

	private static readonly List<Storage.StoredItemModifier> StoredItemModifiers = new List<Storage.StoredItemModifier>
	{
		Storage.StoredItemModifier.Hide,
		Storage.StoredItemModifier.Insulate,
		Storage.StoredItemModifier.Seal
	};

	private static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[] { LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(1, 1), UI.LOGIC_PORTS.CONTROL_OPERATIONAL, false) };
}
