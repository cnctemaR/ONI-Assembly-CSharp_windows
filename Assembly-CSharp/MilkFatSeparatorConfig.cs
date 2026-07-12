using System;
using TUNING;
using UnityEngine;

public class MilkFatSeparatorConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "MilkFatSeparator";
		int num = 4;
		int num2 = 4;
		string text2 = "milk_separator_kanim";
		int num3 = 100;
		float num4 = 120f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, refined_METALS, num5, buildLocationRule, BUILDINGS.DECOR.PENALTY.TIER2, none, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 480f;
		buildingDef.SelfHeatKilowattsWhenActive = 8f;
		buildingDef.ExhaustKilowattsWhenActive = 0f;
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.OutputConduitType = ConduitType.Liquid;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(2, 2);
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.AudioSize = "large";
		buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		return buildingDef;
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Storage storage = go.AddOrGet<Storage>();
		storage.allowItemRemoval = false;
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		storage.showInUI = true;
		go.AddOrGet<Operational>();
		go.AddOrGet<EmptyMilkSeparatorWorkable>();
		go.AddOrGetDef<MilkSeparator.Def>().MILK_FAT_CAPACITY = 15f;
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
		{
			new ElementConverter.ConsumedElement(new Tag("Milk"), 1f, true)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[]
		{
			new ElementConverter.OutputElement(0.089999996f, SimHashes.MilkFat, 0f, false, true, 0f, 0.5f, 1f, byte.MaxValue, 0, true),
			new ElementConverter.OutputElement(0.80999994f, SimHashes.Brine, 0f, false, true, 0f, 0.5f, 0f, byte.MaxValue, 0, true),
			new ElementConverter.OutputElement(0.100000024f, SimHashes.CarbonDioxide, 348.15f, false, false, 1f, 3f, 0f, byte.MaxValue, 0, true)
		};
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.consumptionRate = 10f;
		conduitConsumer.capacityKG = 4f;
		conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.Milk).tag;
		conduitConsumer.forceAlwaysSatisfied = true;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Store;
		ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
		conduitDispenser.conduitType = ConduitType.Liquid;
		conduitDispenser.invertElementFilter = true;
		conduitDispenser.elementFilter = new SimHashes[] { SimHashes.Milk };
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}

	public override void ConfigurePost(BuildingDef def)
	{
	}

	public const string ID = "MilkFatSeparator";

	public const float INPUT_RATE = 1f;

	public const float MILK_STORED_CAPACITY = 4f;

	public const float MILK_FAT_CAPACITY = 15f;

	public const float EFFICIENCY = 0.9f;

	public const float MILKFAT_PERCENT = 0.1f;

	private const float MILK_TO_FAT_OUTPUT_RATE = 0.089999996f;

	private const float MILK_TO_BRINE_WATER_OUTPUT_RATE = 0.80999994f;

	private const float MILK_TO_CO2_RATE = 0.100000024f;
}
