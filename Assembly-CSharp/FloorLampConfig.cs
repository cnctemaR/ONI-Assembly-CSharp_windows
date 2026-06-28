using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class FloorLampConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "FloorLamp";
		int num = 1;
		int num2 = 2;
		string text2 = "floorlamp_kanim";
		float num3 = 100f;
		int num4 = 10;
		float num5 = 10f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER1;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num6 = 800f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, num5, tier, all_METALS, num6, buildLocationRule, global::TUNING.BUILDINGS.DECOR.NONE, none);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 5f;
		buildingDef.OperatingKilowatts = 0.5f;
		buildingDef.ViewMode = SimViewMode.Light;
		buildingDef.MaterialCategory = MATERIALS.ALL_METALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.HotKey = global::Action.BuildMenuKeyA;
		return buildingDef;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		LightShapePreview lightShapePreview = go.AddComponent<LightShapePreview>();
		lightShapePreview.radius = 4f;
		lightShapePreview.shape = LightShape.Circle;
		lightShapePreview.offset = new CellOffset((int)def.BuildingComplete.GetComponent<Light2D>().Offset.x, (int)def.BuildingComplete.GetComponent<Light2D>().Offset.y);
		GeneratedBuildings.RegisterLogicPorts(go, FloorLampConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, FloorLampConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, FloorLampConfig.INPUT_PORTS);
		go.AddOrGet<LogicOperationalController>();
		go.AddOrGet<EnergyConsumer>();
		go.AddOrGet<LoopingSounds>();
		Light2D light2D = go.AddOrGet<Light2D>();
		light2D.overlayColour = LIGHT2D.FLOORLAMP_OVERLAYCOLOR;
		light2D.Color = LIGHT2D.FLOORLAMP_COLOR;
		light2D.Range = 4f;
		light2D.Angle = 0f;
		light2D.Direction = LIGHT2D.FLOORLAMP_DIRECTION;
		light2D.Offset = LIGHT2D.FLOORLAMP_OFFSET;
		light2D.shape = LightShape.Circle;
		light2D.drawOverlay = true;
		BuildingTemplates.DoPostConfigure(go);
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			LightController.Instance instance = new LightController.Instance(game_object.GetComponent<KPrefabID>());
			instance.StartSM();
		};
	}

	public const string ID = "FloorLamp";

	private static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[]
	{
		new LogicPorts.Port(LogicOperationalController.PORT_ID, new CellOffset(0, 0), UI.LOGIC_PORTS.CONTROL_OPERATIONAL, false)
	};
}
