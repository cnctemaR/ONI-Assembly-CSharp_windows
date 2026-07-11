using System;
using System.Collections.Generic;
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
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		FuelTank fuelTank = go.AddOrGet<FuelTank>();
		fuelTank.capacityKg = fuelTank.minimumLaunchMass;
		fuelTank.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>
		{
			Storage.StoredItemModifier.Hide,
			Storage.StoredItemModifier.Seal,
			Storage.StoredItemModifier.Insulate
		});
		go.AddOrGet<CopyBuildingSettings>();
		go.AddOrGet<DropToUserCapacity>();
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(fuelTank);
		manualDeliveryKG.refillMass = fuelTank.capacityKg;
		manualDeliveryKG.capacity = fuelTank.capacityKg;
		manualDeliveryKG.operationalRequirement = FetchOrder2.OperationalRequirement.None;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.consumptionRate = 10f;
		conduitConsumer.capacityTag = GameTags.Liquid;
		conduitConsumer.capacityKG = fuelTank.capacityKg;
		conduitConsumer.forceAlwaysSatisfied = true;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Store;
		RocketModule rocketModule = go.AddOrGet<RocketModule>();
		rocketModule.SetBGKAnim(Assets.GetAnim("rocket_liquid_fuel_tank_bg_kanim"));
		EntityTemplates.ExtendBuildingToRocketModule(go);
	}

	public const string ID = "LiquidFuelTank";

	public const float FuelCapacity = 900f;
}
