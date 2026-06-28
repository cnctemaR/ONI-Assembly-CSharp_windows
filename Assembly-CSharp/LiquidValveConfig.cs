using System;
using TUNING;
using UnityEngine;

public class LiquidValveConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		EffectorValues tier = NOISE_POLLUTION.NOISY.TIER1;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("LiquidValve", 1, 2, "valveliquid_kanim", 200f, 30, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.RAW_METALS, 1600f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.PENALTY.TIER0, tier);
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.OutputConduitType = ConduitType.Liquid;
		buildingDef.Floodable = false;
		buildingDef.ViewMode = SimViewMode.LiquidVentMap;
		buildingDef.MaterialCategory = MATERIALS.RAW_METALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PermittedRotations = PermittedRotations.R360;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 1);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		GeneratedBuildings.MakeBuildableAnywhere(go);
		Valve valve = go.AddOrGet<Valve>();
		valve.conduitType = ConduitType.Liquid;
		valve.smallAmount = 1f;
		valve.largeAmount = 10f;
		valve.maxFlow = 10f;
		valve.animFlowRanges = new Valve.AnimRangeInfo[]
		{
			new Valve.AnimRangeInfo(3f, "lo"),
			new Valve.AnimRangeInfo(7f, "med"),
			new Valve.AnimRangeInfo(10f, "hi")
		};
		Workable workable = go.AddOrGet<Workable>();
		workable.workTime = 5f;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		global::UnityEngine.Object.DestroyImmediate(go.GetComponent<RequireInputs>());
		global::UnityEngine.Object.DestroyImmediate(go.GetComponent<ConduitConsumer>());
		global::UnityEngine.Object.DestroyImmediate(go.GetComponent<ConduitDispenser>());
		BuildingTemplates.DoPostConfigure(go);
	}

	private const ConduitType CONDUIT_TYPE = ConduitType.Liquid;
}
