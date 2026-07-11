using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class GantryConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "Gantry";
		int num = 6;
		int num2 = 2;
		string text2 = "gantry_kanim";
		int num3 = 30;
		float num4 = 30f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER6;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.Anywhere;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, all_METALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.NONE, none, 1f);
		buildingDef.ObjectLayer = ObjectLayer.Gantry;
		buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		buildingDef.Entombable = true;
		buildingDef.IsFoundation = false;
		buildingDef.RequiresPowerInput = true;
		buildingDef.PowerInputOffset = new CellOffset(-2, 1);
		buildingDef.EnergyConsumptionWhenActive = 1200f;
		buildingDef.ExhaustKilowattsWhenActive = 8f;
		buildingDef.SelfHeatKilowattsWhenActive = 4f;
		buildingDef.AudioCategory = "Metal";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, GantryConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, GantryConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<Gantry>();
		GeneratedBuildings.RegisterLogicPorts(go, GantryConfig.INPUT_PORTS);
		global::UnityEngine.Object.DestroyImmediate(go.GetComponent<LogicOperationalController>());
	}

	public const string ID = "Gantry";

	private static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[] { LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, new CellOffset(-1, 1), UI.LOGIC_PORTS.CONTROL_OPERATIONAL, false) };
}
