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
		float num3 = 50f;
		int num4 = 30;
		float num5 = 30f;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		EffectorValues tier = NOISE_POLLUTION.NOISY.TIER0;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, num5, global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, refined_METALS, 1600f, BuildLocationRule.OnFloor, global::TUNING.BUILDINGS.DECOR.BONUS.TIER1, tier);
		buildingDef.ForegroundLayer = Grid.SceneLayer.Front;
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		buildingDef.PreventIdlingInFrontOfBuilding = true;
		buildingDef.Floodable = false;
		buildingDef.RequiresPowerInput = true;
		buildingDef.PowerInputOffset = new CellOffset(0, 2);
		buildingDef.EnergyConsumptionWhenActive = 10f;
		buildingDef.OperatingKilowatts = 0.5f;
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

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<Checkpoint>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		GeneratedBuildings.RegisterLogicPorts(go, CheckpointConfig.INPUT_PORTS);
	}

	public const string ID = "Checkpoint";

	public static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[]
	{
		new LogicPorts.Port(Checkpoint.PORT_ID, new CellOffset(0, 2), global::STRINGS.BUILDINGS.PREFABS.CHECKPOINT.LOGIC_PORT_DESC, true)
	};
}
