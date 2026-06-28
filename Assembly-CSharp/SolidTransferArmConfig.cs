using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class SolidTransferArmConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("SolidTransferArm", 3, 1, "conveyor_transferarm_kanim", 10, 10f, global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.Anywhere, global::TUNING.BUILDINGS.DECOR.PENALTY.TIER2, NOISE_POLLUTION.NOISY.TIER0, 0.2f);
		buildingDef.AudioCategory = "Metal";
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.ExhaustKilowattsWhenActive = 0f;
		buildingDef.SelfHeatKilowattsWhenActive = 2f;
		buildingDef.PermittedRotations = PermittedRotations.R360;
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, "SolidTransferArm");
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<Operational>();
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, SolidTransferArmConfig.INPUT_PORTS);
		StationaryChoreRangeVisualizer stationaryChoreRangeVisualizer = go.AddOrGet<StationaryChoreRangeVisualizer>();
		stationaryChoreRangeVisualizer.range = 4;
		stationaryChoreRangeVisualizer.movable = true;
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, SolidTransferArmConfig.INPUT_PORTS);
		StationaryChoreRangeVisualizer stationaryChoreRangeVisualizer = go.AddOrGet<StationaryChoreRangeVisualizer>();
		stationaryChoreRangeVisualizer.range = 4;
		Constructable component = go.GetComponent<Constructable>();
		component.choreTags = GameTags.ChoreTypes.ConveyorChores;
		component.requiredRolePerk = RoleManager.rolePerks.ConveyorBuild.id;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		GeneratedBuildings.RegisterLogicPorts(go, SolidTransferArmConfig.INPUT_PORTS);
		go.AddOrGet<LogicOperationalController>();
		SolidTransferArm solidTransferArm = go.AddOrGet<SolidTransferArm>();
		solidTransferArm.pickupRange = 4;
		StationaryChoreRangeVisualizer stationaryChoreRangeVisualizer = go.AddOrGet<StationaryChoreRangeVisualizer>();
		stationaryChoreRangeVisualizer.range = 4;
	}

	public const string ID = "SolidTransferArm";

	private const int RANGE = 4;

	private static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[] { LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(0, 0), UI.LOGIC_PORTS.CONTROL_OPERATIONAL, false) };
}
