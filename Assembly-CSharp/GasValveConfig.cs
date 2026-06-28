using System;
using TUNING;
using UnityEngine;

public class GasValveConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("GasValve", 1, 2, "valvegas_kanim", 200f, 10f, BUILDINGS.CONSTRUCTION_MASS.TIER3, MATERIALS.RAW_METALS, 1600f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.PENALTY.TIER0, null);
		buildingDef.InputConduitType = ConduitType.Gas;
		buildingDef.OutputConduitType = ConduitType.Gas;
		buildingDef.Floodable = false;
		buildingDef.ViewMode = SimViewMode.GasVentMap;
		buildingDef.MaterialCategory = MATERIALS.RAW_METALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PermittedRotations = Rotatable.PermittedRotations.R360;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 1);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		Valve valve = go.AddOrGet<Valve>();
		valve.smallAmount = 0.1f;
		valve.largeAmount = 1f;
		valve.maxFlow = 0.1f;
		valve.minFlow = 0.01f;
		valve.animFlowRanges = new Valve.AnimRangeInfo[]
		{
			new Valve.AnimRangeInfo(0f, "off"),
			new Valve.AnimRangeInfo(0.25f, "lo"),
			new Valve.AnimRangeInfo(0.5f, "med"),
			new Valve.AnimRangeInfo(0.75f, "hi")
		};
		Workable workable = go.AddOrGet<Workable>();
		workable.workTime = 5f;
	}

	public override void DoPostConfigure(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}
}
