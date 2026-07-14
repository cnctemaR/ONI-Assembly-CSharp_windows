using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class MilkFeederConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "MilkFeeder";
		int num = 3;
		int num2 = 3;
		string text2 = "critter_milk_feeder_kanim";
		int num3 = 100;
		float num4 = 120f;
		float[] array = new float[]
		{
			global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4[0],
			global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2[0]
		};
		string[] array2 = new string[] { "RefinedMetal", "Glasses" };
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, array, array2, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.PENALTY.TIER2, none, 0.2f);
		buildingDef.AudioCategory = "Metal";
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
		buildingDef.AddSearchTerms(SEARCH_TERMS.RANCHING);
		buildingDef.AddSearchTerms(SEARCH_TERMS.CRITTER);
		return buildingDef;
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Prioritizable.AddRef(go);
		go.AddOrGet<LogicOperationalController>();
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.CreaturePen.Id;
		roomTracker.requirement = RoomTracker.Requirement.Required;
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 80f;
		storage.showInUI = true;
		storage.showDescriptor = true;
		storage.allowItemRemoval = false;
		storage.allowSettingOnlyFetchMarkedItems = false;
		storage.showCapacityStatusItem = true;
		storage.showCapacityAsMainStatus = true;
		storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.consumptionRate = 10f;
		conduitConsumer.capacityTag = GameTags.Creatures.CritterDrinkable;
		conduitConsumer.forceAlwaysSatisfied = true;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		conduitConsumer.storage = storage;
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.RanchStationType, false);
		MilkFeeder.Def def = go.AddOrGetDef<MilkFeeder.Def>();
		def.elementProducedTag = GameTags.Creatures.CritterDrinkable;
		def.unitsProducedPerFeeding = 5f;
		def.drinkCellOffset = MilkFeederConfig.DRINK_FROM_OFFSET;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}

	public override void ConfigurePost(BuildingDef def)
	{
	}

	public const string ID = "MilkFeeder";

	public const string HAD_CONSUMED_MILK_RECENTLY_EFFECT_ID = "HadMilk";

	public const string HAD_CONSUMED_INK_RECENTLY_EFFECT_ID = "HadInk";

	public static readonly global::Tuple<Tag, string>[] EffectsPerDrinkableLiquid = new global::Tuple<Tag, string>[]
	{
		new global::Tuple<Tag, string>(SimHashes.Milk.CreateTag(), "HadMilk"),
		new global::Tuple<Tag, string>(SimHashes.Ink.CreateTag(), "HadInk")
	};

	public const float EFFECT_DURATION_IN_SECONDS = 600f;

	public const float UNITS_OF_MILK_CONSUMED_PER_FEEDING = 5f;

	private static readonly CellOffset DRINK_FROM_OFFSET = new CellOffset(1, 0);

	private static readonly Tag MILK_TAG = SimHashes.Milk.CreateTag();
}
