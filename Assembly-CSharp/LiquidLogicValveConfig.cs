using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class LiquidLogicValveConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "LiquidLogicValve";
		int num = 1;
		int num2 = 2;
		string text2 = "valveliquid_logic_kanim";
		int num3 = 30;
		float num4 = 10f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER1;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.Anywhere;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER1;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, refined_METALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.PENALTY.TIER0, tier2, 0.2f);
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.OutputConduitType = ConduitType.Liquid;
		buildingDef.Floodable = false;
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 10f;
		buildingDef.PowerInputOffset = new CellOffset(0, 1);
		buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 1);
		buildingDef.LogicInputPorts = new List<LogicPorts.Port> { LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(0, 0), global::STRINGS.BUILDINGS.PREFABS.LIQUIDLOGICVALVE.LOGIC_PORT, global::STRINGS.BUILDINGS.PREFABS.LIQUIDLOGICVALVE.LOGIC_PORT_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.LIQUIDLOGICVALVE.LOGIC_PORT_INACTIVE, true, false) };
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, "LiquidLogicValve");
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		global::UnityEngine.Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		OperationalValve operationalValve = go.AddOrGet<OperationalValve>();
		operationalValve.conduitType = ConduitType.Liquid;
		operationalValve.maxFlow = 10f;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		global::UnityEngine.Object.DestroyImmediate(go.GetComponent<ConduitConsumer>());
		global::UnityEngine.Object.DestroyImmediate(go.GetComponent<ConduitDispenser>());
		go.GetComponent<RequireInputs>().SetRequirements(true, false);
		go.AddOrGet<LogicOperationalController>().unNetworkedValue = 0;
		go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits, false);
	}

	public const string ID = "LiquidLogicValve";

	private const ConduitType CONDUIT_TYPE = ConduitType.Liquid;
}
