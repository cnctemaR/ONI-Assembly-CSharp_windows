using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class SolidLimitValveConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "SolidLimitValve";
		int num = 1;
		int num2 = 2;
		string text2 = "limit_valve_solid_kanim";
		int num3 = 30;
		float num4 = 10f;
		float[] array = new float[]
		{
			global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0[0],
			1f
		};
		string[] array2 = new string[] { "RefinedMetal", "BuildingGasket" };
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.Anywhere;
		EffectorValues tier = NOISE_POLLUTION.NOISY.TIER1;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, array, array2, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.PENALTY.TIER0, tier, 0.2f);
		buildingDef.InputConduitType = ConduitType.Solid;
		buildingDef.OutputConduitType = ConduitType.Solid;
		buildingDef.Floodable = false;
		buildingDef.Entombable = false;
		buildingDef.Overheatable = false;
		buildingDef.ViewMode = OverlayModes.SolidConveyor.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		buildingDef.BaseTimeUntilRepair = -1f;
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 1);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 10f;
		buildingDef.PowerInputOffset = new CellOffset(0, 1);
		buildingDef.LogicInputPorts = new List<LogicPorts.Port>
		{
			new LogicPorts.Port(LimitValve.RESET_PORT_ID, new CellOffset(0, 1), global::STRINGS.BUILDINGS.PREFABS.SOLIDLIMITVALVE.LOGIC_PORT_RESET, global::STRINGS.BUILDINGS.PREFABS.SOLIDLIMITVALVE.RESET_PORT_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.SOLIDLIMITVALVE.RESET_PORT_INACTIVE, false, LogicPortSpriteType.ResetUpdate, true)
		};
		buildingDef.LogicOutputPorts = new List<LogicPorts.Port> { LogicPorts.Port.OutputPort(LimitValve.OUTPUT_PORT_ID, new CellOffset(0, 0), global::STRINGS.BUILDINGS.PREFABS.SOLIDLIMITVALVE.LOGIC_PORT_OUTPUT, global::STRINGS.BUILDINGS.PREFABS.SOLIDLIMITVALVE.OUTPUT_PORT_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.SOLIDLIMITVALVE.OUTPUT_PORT_INACTIVE, false, false) };
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, "SolidLimitValve");
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		global::UnityEngine.Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGetDef<PoweredActiveTransitionController.Def>();
		go.AddOrGet<RequireOutputs>().ignoreFullPipe = true;
		go.AddOrGet<SolidConduitBridge>();
		LimitValve limitValve = go.AddOrGet<LimitValve>();
		limitValve.conduitType = ConduitType.Solid;
		limitValve.displayUnitsInsteadOfMass = true;
		limitValve.Limit = 0f;
		limitValve.maxLimitKg = 500f;
		limitValve.sliderRanges = new NonLinearSlider.Range[]
		{
			new NonLinearSlider.Range(50f, 50f),
			new NonLinearSlider.Range(30f, 200f),
			new NonLinearSlider.Range(20f, limitValve.maxLimitKg)
		};
	}

	public const string ID = "SolidLimitValve";

	private const ConduitType CONDUIT_TYPE = ConduitType.Solid;
}
