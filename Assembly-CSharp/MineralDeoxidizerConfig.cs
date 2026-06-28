using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class MineralDeoxidizerConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "MineralDeoxidizer";
		int num = 1;
		int num2 = 2;
		string text2 = "mineraldeoxidizer_kanim";
		float num3 = 100f;
		int num4 = 30;
		float num5 = 30f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num6 = 800f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER3;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, num5, tier, all_METALS, num6, buildLocationRule, global::TUNING.BUILDINGS.DECOR.PENALTY.TIER1, tier2);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.ExhaustKilowattsWhenActive = 0.5f;
		buildingDef.OperatingKilowatts = 1f;
		buildingDef.ViewMode = SimViewMode.OxygenMap;
		buildingDef.MaterialCategory = MATERIALS.ALL_METALS;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.Breakable = true;
		buildingDef.Upgradeable = false;
		buildingDef.HotKey = global::Action.BuildMenuKeyD;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		Prioritizable.AddRef(go);
		Electrolyzer electrolyzer = go.AddOrGet<Electrolyzer>();
		electrolyzer.maxMass = 1.8f;
		electrolyzer.hasMeter = false;
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 1000f;
		storage.showInUI = true;
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
		{
			new ElementConverter.ConsumedElement(new Tag("Algae"), 0.55f)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[]
		{
			new ElementConverter.OutputElement(0.5f, SimHashes.Oxygen, 303.15f, false, 0f, 1f, false, 1f, byte.MaxValue, 0)
		};
		elementConverter.conversionInterval = 1f;
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = new Tag("Algae");
		manualDeliveryKG.capacity = 100f;
		manualDeliveryKG.refillMass = 25f;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, MineralDeoxidizerConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, MineralDeoxidizerConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		GeneratedBuildings.RegisterLogicPorts(go, MineralDeoxidizerConfig.INPUT_PORTS);
		go.AddOrGet<LogicOperationalController>();
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			PoweredActiveController.Instance instance = new PoweredActiveController.Instance(game_object.GetComponent<KPrefabID>());
			instance.StartSM();
		};
	}

	public const string ID = "MineralDeoxidizer";

	private const float OXYGEN_GENERATION_RATE = 0.5f;

	private const float OXYGEN_TEMPERATURE = 303.15f;

	private static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[]
	{
		new LogicPorts.Port(LogicOperationalController.PORT_ID, new CellOffset(0, 1), UI.LOGIC_PORTS.CONTROL_OPERATIONAL, false)
	};
}
