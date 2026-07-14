using System;
using TUNING;
using UnityEngine;

public class LiquidHeaterConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "LiquidHeater";
		int num = 4;
		int num2 = 1;
		string text2 = "boiler_kanim";
		int num3 = 30;
		float num4 = 30f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num5 = 3200f;
		BuildLocationRule buildLocationRule = BuildLocationRule.Anywhere;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, all_METALS, num5, buildLocationRule, BUILDINGS.DECOR.PENALTY.TIER1, none, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.Floodable = false;
		buildingDef.EnergyConsumptionWhenActive = 960f;
		buildingDef.ExhaustKilowattsWhenActive = 0f;
		buildingDef.SelfHeatKilowattsWhenActive = 0f;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "SolidMetal";
		buildingDef.OverheatTemperature = 398.15f;
		buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(1, 0));
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		go.AddOrGet<KBatchedAnimHeatPostProcessingEffect>();
		SpaceHeater spaceHeater = go.AddOrGet<SpaceHeater>();
		spaceHeater.SetLiquidHeater();
		spaceHeater.produceHeat = true;
		spaceHeater.hasTargetTemperature = false;
		spaceHeater.minimumCellMass = 400f;
		spaceHeater.maxPower = 4000f;
		spaceHeater.minPower = 960f;
		spaceHeater.maxSelfHeatKWs = 64f;
		spaceHeater.maxExhaustedKWs = 4000f;
		go.AddOrGet<LiquidHeaterBubbleEmitter>().BubblePowerThreshold = 961f;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicOperationalController>();
	}

	public const string ID = "LiquidHeater";

	public const float MAX_SELF_HEAT = 64f;

	public const float MAX_EXHAUST_HEAT = 4000f;

	public const float MIN_POWER_USAGE = 960f;

	public const float MAX_POWER_USAGE = 4000f;

	public const float BUBBLE_POWER_THRESHOLD = 961f;

	public const float MAX_NORMAL_TEMPERATURE = 358.15f;
}
