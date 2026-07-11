using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class GasLogicValveConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "GasLogicValve";
		int num = 1;
		int num2 = 2;
		string text2 = "valvegas_logic_kanim";
		int num3 = 30;
		float num4 = 10f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER1;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.Anywhere;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER1;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, refined_METALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.PENALTY.TIER0, tier2, 0.2f);
		buildingDef.InputConduitType = ConduitType.Gas;
		buildingDef.OutputConduitType = ConduitType.Gas;
		buildingDef.Floodable = false;
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 10f;
		buildingDef.PowerInputOffset = new CellOffset(0, 1);
		buildingDef.ViewMode = OverlayModes.GasConduits.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 1);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		OperationalValve operationalValve = go.AddOrGet<OperationalValve>();
		operationalValve.conduitType = ConduitType.Gas;
		operationalValve.maxFlow = 1f;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, GasLogicValveConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, GasLogicValveConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		global::UnityEngine.Object.DestroyImmediate(go.GetComponent<ConduitConsumer>());
		global::UnityEngine.Object.DestroyImmediate(go.GetComponent<ConduitDispenser>());
		go.GetComponent<RequireInputs>().SetRequirements(true, false);
		GeneratedBuildings.RegisterLogicPorts(go, GasLogicValveConfig.INPUT_PORTS);
		go.AddOrGet<LogicOperationalController>().unNetworkedValue = 0;
	}

	public const string ID = "GasLogicValve";

	private const ConduitType CONDUIT_TYPE = ConduitType.Gas;

	private static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[] { LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(0, 0), global::STRINGS.BUILDINGS.PREFABS.GASLOGICVALVE.LOGIC_PORT, global::STRINGS.BUILDINGS.PREFABS.GASLOGICVALVE.LOGIC_PORT_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.GASLOGICVALVE.LOGIC_PORT_INACTIVE, true, false) };
}
