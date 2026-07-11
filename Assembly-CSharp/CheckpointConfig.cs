using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class CheckpointConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "Checkpoint";
		int num = 1;
		int num2 = 3;
		string text2 = "checkpoint_kanim";
		int num3 = 30;
		float num4 = 30f;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
		string[] array = refined_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER0;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, array, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.BONUS.TIER1, tier2, 0.2f);
		buildingDef.ForegroundLayer = Grid.SceneLayer.Front;
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		buildingDef.PreventIdleTraversalPastBuilding = true;
		buildingDef.Floodable = false;
		buildingDef.RequiresPowerInput = true;
		buildingDef.PowerInputOffset = new CellOffset(0, 2);
		buildingDef.EnergyConsumptionWhenActive = 10f;
		buildingDef.SelfHeatKilowattsWhenActive = 0.5f;
		return buildingDef;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, CheckpointConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, CheckpointConfig.INPUT_PORTS);
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<Checkpoint>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, CheckpointConfig.INPUT_PORTS);
	}

	public const string ID = "Checkpoint";

	private static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[] { LogicPorts.Port.InputPort(Checkpoint.PORT_ID, new CellOffset(0, 2), global::STRINGS.BUILDINGS.PREFABS.CHECKPOINT.LOGIC_PORT, global::STRINGS.BUILDINGS.PREFABS.CHECKPOINT.LOGIC_PORT_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.CHECKPOINT.LOGIC_PORT_INACTIVE, true, false) };
}
