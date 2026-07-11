using System;
using TUNING;
using UnityEngine;

public class JuicerConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "Juicer";
		int num = 3;
		int num2 = 4;
		string text2 = "juicer_kanim";
		int num3 = 30;
		float num4 = 10f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] raw_METALS = MATERIALS.RAW_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, raw_METALS, num5, buildLocationRule, BUILDINGS.DECOR.BONUS.TIER1, none, 0.2f);
		buildingDef.DlcId = "PACK1";
		buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
		buildingDef.Floodable = true;
		buildingDef.AudioCategory = "Metal";
		buildingDef.Overheatable = true;
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.UtilityInputOffset = new CellOffset(1, 1);
		buildingDef.RequiresPowerInput = true;
		buildingDef.PowerInputOffset = new CellOffset(1, 0);
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.SelfHeatKilowattsWhenActive = 0.5f;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.RecBuilding, false);
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 20f;
		storage.SetDefaultStoredItemModifiers(Storage.StandardFabricatorStorage);
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.Water).tag;
		conduitConsumer.capacityKG = 2f;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		ManualDeliveryKG manualDeliveryKG = go.AddComponent<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.requestedItemTag = MushroomConfig.ID.ToTag();
		manualDeliveryKG.capacity = 10f;
		manualDeliveryKG.refillMass = 5f;
		manualDeliveryKG.minimumMass = 1f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
		ManualDeliveryKG manualDeliveryKG2 = go.AddComponent<ManualDeliveryKG>();
		manualDeliveryKG2.SetStorage(storage);
		manualDeliveryKG2.requestedItemTag = PrickleFruitConfig.ID.ToTag();
		manualDeliveryKG2.capacity = 10f;
		manualDeliveryKG2.refillMass = 5f;
		manualDeliveryKG2.minimumMass = 1f;
		manualDeliveryKG2.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
		ManualDeliveryKG manualDeliveryKG3 = go.AddComponent<ManualDeliveryKG>();
		manualDeliveryKG3.SetStorage(storage);
		manualDeliveryKG3.requestedItemTag = "BasicPlantFood".ToTag();
		manualDeliveryKG3.capacity = 10f;
		manualDeliveryKG3.refillMass = 5f;
		manualDeliveryKG3.minimumMass = 1f;
		manualDeliveryKG3.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
		JuicerWorkable juicerWorkable = go.AddOrGet<JuicerWorkable>();
		juicerWorkable.basePriority = RELAXATION.PRIORITY.TIER5;
		Juicer juicer = go.AddOrGet<Juicer>();
		juicer.ingredient_tags = new Tag[]
		{
			MushroomConfig.ID.ToTag(),
			PrickleFruitConfig.ID.ToTag(),
			"BasicPlantFood".ToTag()
		};
		juicer.specificEffect = "Juicer";
		juicer.trackingEffect = "RecentlyRecDrink";
		juicer.ingredientMassPerUse = 0.25f;
		juicer.waterMassPerUse = 1f;
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.RecRoom.Id;
		roomTracker.requirement = RoomTracker.Requirement.Recommended;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}

	public const string ID = "Juicer";

	public const float INGREDIENT_MASS_PER_USE = 0.25f;

	public const float WATER_MASS_PER_USE = 1f;
}
