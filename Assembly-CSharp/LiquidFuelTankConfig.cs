using System;
using TUNING;
using UnityEngine;

public class LiquidFuelTankConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "LiquidFuelTank";
		int num = 5;
		int num2 = 5;
		string text2 = "rocket_liquid_fuel_tank_kanim";
		int num3 = 1000;
		float num4 = 60f;
		float[] fuel_TANK_DRY_MASS = BUILDINGS.ROCKETRY_MASS_KG.FUEL_TANK_DRY_MASS;
		string[] array = new string[] { SimHashes.Steel.ToString() };
		float num5 = 9999f;
		BuildLocationRule buildLocationRule = BuildLocationRule.BuildingAttachPoint;
		EffectorValues tier = NOISE_POLLUTION.NOISY.TIER2;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, fuel_TANK_DRY_MASS, array, num5, buildLocationRule, BUILDINGS.DECOR.NONE, tier, 0.2f);
		BuildingTemplates.CreateRocketBuildingDef(buildingDef);
		buildingDef.SceneLayer = Grid.SceneLayer.BuildingFront;
		buildingDef.ViewMode = SimViewMode.None;
		buildingDef.OverheatTemperature = 2273.15f;
		buildingDef.Floodable = false;
		buildingDef.AttachmentSlotTag = GameTags.Rocket;
		buildingDef.ObjectLayer = ObjectLayer.Building;
		buildingDef.UtilityInputOffset = new CellOffset(2, 3);
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.RequiresPowerInput = false;
		buildingDef.attachablePosition = new CellOffset(0, 0);
		buildingDef.CanMove = true;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		BuildingAttachPoint buildingAttachPoint = go.AddOrGet<BuildingAttachPoint>();
		buildingAttachPoint.points = new BuildingAttachPoint.HardPoint[]
		{
			new BuildingAttachPoint.HardPoint(new CellOffset(0, 5), GameTags.Rocket, null)
		};
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		FuelTank fuelTank = go.AddOrGet<FuelTank>();
		fuelTank.FuelType = ElementLoader.FindElementByHash(SimHashes.Petroleum).tag;
		fuelTank.capacityKg = fuelTank.minimumLaunchMass;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.consumptionRate = 10f;
		conduitConsumer.capacityTag = fuelTank.FuelType;
		conduitConsumer.capacityKG = fuelTank.capacityKg;
		conduitConsumer.forceAlwaysSatisfied = true;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		go.AddOrGet<RocketModule>();
		EntityTemplates.ExtendEntityToRocketModule(go);
	}

	public const string ID = "LiquidFuelTank";
}
