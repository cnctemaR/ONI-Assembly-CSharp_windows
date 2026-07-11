using System;
using TUNING;
using UnityEngine;

public class HotTubConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "HotTub";
		int num = 5;
		int num2 = 2;
		string text2 = "hottub_kanim";
		int num3 = 30;
		float num4 = 10f;
		float[] array = new float[] { 200f, 200f };
		string[] array2 = new string[] { "Metal", "BuildingWood" };
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, array, array2, num5, buildLocationRule, BUILDINGS.DECOR.BONUS.TIER3, none, 0.2f);
		buildingDef.DlcId = "PACK1";
		buildingDef.SceneLayer = Grid.SceneLayer.BuildingFront;
		buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
		buildingDef.Floodable = true;
		buildingDef.AudioCategory = "Metal";
		buildingDef.Overheatable = true;
		buildingDef.OverheatTemperature = this.MINIMUM_WATER_TEMPERATURE;
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.OutputConduitType = ConduitType.Liquid;
		buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(2, 0);
		buildingDef.RequiresPowerInput = true;
		buildingDef.PowerInputOffset = new CellOffset(-2, 0);
		buildingDef.EnergyConsumptionWhenActive = 240f;
		buildingDef.SelfHeatKilowattsWhenActive = 4f;
		buildingDef.ExhaustKilowattsWhenActive = 1f;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.RecBuilding, false);
		Storage storage = go.AddOrGet<Storage>();
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.Water).tag;
		conduitConsumer.capacityKG = this.WATER_AMOUNT;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		conduitConsumer.storage = storage;
		conduitConsumer.SetOnState(false);
		ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
		conduitDispenser.conduitType = ConduitType.Liquid;
		conduitDispenser.storage = storage;
		conduitDispenser.SetOnState(false);
		ManualDeliveryKG manualDeliveryKG = go.AddComponent<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = new Tag("BleachStone");
		manualDeliveryKG.capacity = 100f;
		manualDeliveryKG.refillMass = 10f;
		manualDeliveryKG.minimumMass = 1f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
		HotTub hotTub = go.AddOrGet<HotTub>();
		hotTub.waterStorage = storage;
		hotTub.hotTubCapacity = this.WATER_AMOUNT;
		hotTub.waterCoolingRate = 15f;
		hotTub.minimumWaterTemperature = this.MINIMUM_WATER_TEMPERATURE;
		hotTub.bleachStoneConsumption = this.BLEACH_STONE_CONSUMPTION_RATE;
		hotTub.maxOperatingTemperature = this.MAXIMUM_TUB_TEMPERATURE;
		hotTub.specificEffect = "HotTub";
		hotTub.trackingEffect = "RecentlyHotTub";
		hotTub.basePriority = RELAXATION.PRIORITY.TIER4;
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.RecRoom.Id;
		roomTracker.requirement = RoomTracker.Requirement.Recommended;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<RequireInputs>().requireConduitHasMass = false;
	}

	public const string ID = "HotTub";

	private float WATER_AMOUNT = 100f;

	private const float KDTU_TRANSFER_RATE = 15f;

	private float MINIMUM_WATER_TEMPERATURE = 310.85f;

	private float MAXIMUM_TUB_TEMPERATURE = 310.85f;

	private float BLEACH_STONE_CONSUMPTION_RATE = 0.11666667f;
}
