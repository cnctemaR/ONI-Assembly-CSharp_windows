using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class TravelTubeEntranceConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "TravelTubeEntrance";
		int num = 3;
		int num2 = 2;
		string text2 = "tube_launcher_kanim";
		float num3 = 400f;
		int num4 = 100;
		float num5 = 120f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		float num6 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, num5, tier, refined_METALS, num6, buildLocationRule, global::TUNING.BUILDINGS.DECOR.PENALTY.TIER1, none);
		buildingDef.Overheatable = false;
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 960f;
		buildingDef.Entombable = true;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PowerInputOffset = new CellOffset(1, 0);
		return buildingDef;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, TravelTubeEntranceConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, TravelTubeEntranceConfig.INPUT_PORTS);
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		TravelTubeEntrance travelTubeEntrance = go.AddOrGet<TravelTubeEntrance>();
		travelTubeEntrance.joulesPerLaunch = 10000f;
		travelTubeEntrance.jouleCapacity = 40000f;
		go.AddOrGet<TravelTubeEntrance.Work>();
		go.AddOrGet<LogicOperationalController>();
		go.AddOrGet<EnergyConsumerSelfSustaining>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<RequireInputs>().visualizeRequirements = false;
		GeneratedBuildings.RegisterLogicPorts(go, TravelTubeEntranceConfig.INPUT_PORTS);
		BuildingTemplates.DoPostConfigure(go);
	}

	public const string ID = "TravelTubeEntrance";

	private const float JOULES_PER_LAUNCH = 10000f;

	private const float LAUNCHES_FROM_FULL_CHARGE = 4f;

	private static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[]
	{
		new LogicPorts.Port(LogicOperationalController.PORT_ID, new CellOffset(1, 1), UI.LOGIC_PORTS.CONTROL_OPERATIONAL, false)
	};
}
