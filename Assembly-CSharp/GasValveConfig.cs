using System;
using TUNING;
using UnityEngine;

public class GasValveConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("GasValve", 1, 2, "valvegas_kanim", 50f, 30, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, MATERIALS.RAW_METALS, 1600f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.PENALTY.TIER0, null);
		buildingDef.InputConduitType = ConduitType.Gas;
		buildingDef.OutputConduitType = ConduitType.Gas;
		buildingDef.Floodable = false;
		buildingDef.ViewMode = SimViewMode.GasVentMap;
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
		valve.conduitType = ConduitType.Gas;
		valve.smallAmount = 0.1f;
		valve.largeAmount = 1f;
		valve.maxFlow = 1f;
		valve.animFlowRanges = new Valve.AnimRangeInfo[]
		{
			new Valve.AnimRangeInfo(0.25f, "lo"),
			new Valve.AnimRangeInfo(0.5f, "med"),
			new Valve.AnimRangeInfo(0.75f, "hi")
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

	private const ConduitType CONDUIT_TYPE = ConduitType.Gas;
}
