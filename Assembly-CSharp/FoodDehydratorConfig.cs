using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class FoodDehydratorConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "FoodDehydrator";
		int num = 3;
		int num2 = 3;
		string text2 = "dehydrator_kanim";
		int num3 = 30;
		float num4 = 30f;
		float[] array = new float[]
		{
			global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3[0],
			global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2[0]
		};
		string[] array2 = new string[] { "RefinedMetal", "Plastic" };
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier = NOISE_POLLUTION.NOISY.TIER3;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, array, array2, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.NONE, tier, 0.2f);
		BuildingTemplates.CreateStandardBuildingDef(buildingDef);
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "large";
		buildingDef.InputConduitType = ConduitType.Gas;
		buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
		buildingDef.SelfHeatKilowattsWhenActive = 4f;
		buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
		buildingDef.ForegroundLayer = Grid.SceneLayer.BuildingFront;
		buildingDef.AddSearchTerms(SEARCH_TERMS.FOOD);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		ComplexFabricator complexFabricator = go.AddOrGet<ComplexFabricator>();
		complexFabricator.heatedTemperature = 368.15f;
		complexFabricator.duplicantOperated = false;
		complexFabricator.showProgressBar = true;
		complexFabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		complexFabricator.keepAdditionalTag = FOODDEHYDRATORTUNING.FUEL_TAG;
		complexFabricator.storeProduced = true;
		go.AddOrGet<FabricatorIngredientStatusManager>();
		go.AddOrGet<CopyBuildingSettings>();
		BuildingTemplates.CreateComplexFabricatorStorage(go, complexFabricator);
		complexFabricator.inStorage.SetDefaultStoredItemModifiers(FoodDehydratorConfig.GourmetCookingStationStoredItemModifiers);
		complexFabricator.buildStorage.SetDefaultStoredItemModifiers(FoodDehydratorConfig.GourmetCookingStationStoredItemModifiers);
		complexFabricator.outStorage.SetDefaultStoredItemModifiers(FoodDehydratorConfig.GourmetCookingStationStoredItemModifiers);
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.capacityTag = FOODDEHYDRATORTUNING.FUEL_TAG;
		conduitConsumer.capacityKG = 5.0000005f;
		conduitConsumer.alwaysConsume = true;
		conduitConsumer.storage = complexFabricator.inStorage;
		conduitConsumer.forceAlwaysSatisfied = true;
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[]
		{
			new ElementConverter.ConsumedElement(FOODDEHYDRATORTUNING.FUEL_TAG, 0.020000001f, true)
		};
		elementConverter.outputElements = new ElementConverter.OutputElement[]
		{
			new ElementConverter.OutputElement(0.0050000004f, SimHashes.CarbonDioxide, 348.15f, false, false, 0f, 1f, 1f, byte.MaxValue, 0, true)
		};
		this.ConfigureRecipes();
		Prioritizable.AddRef(go);
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
		KAnimFile[] array = new KAnimFile[] { Assets.GetAnim("anim_interacts_dehydrator_kanim") };
		FoodDehydratorWorkableEmpty foodDehydratorWorkableEmpty = go.AddOrGet<FoodDehydratorWorkableEmpty>();
		foodDehydratorWorkableEmpty.workTime = 50f;
		foodDehydratorWorkableEmpty.overrideAnims = array;
		foodDehydratorWorkableEmpty.workLayer = Grid.SceneLayer.Front;
		go.AddOrGet<LogicOperationalController>();
		go.AddOrGetDef<FoodDehydrator.Def>();
	}

	private void ConfigureRecipes()
	{
		List<ValueTuple<EdiblesManager.FoodInfo, Tag>> list = new List<ValueTuple<EdiblesManager.FoodInfo, Tag>>
		{
			new ValueTuple<EdiblesManager.FoodInfo, Tag>(FOOD.FOOD_TYPES.SALSA, DehydratedSalsaConfig.ID),
			new ValueTuple<EdiblesManager.FoodInfo, Tag>(FOOD.FOOD_TYPES.MUSHROOM_WRAP, DehydratedMushroomWrapConfig.ID),
			new ValueTuple<EdiblesManager.FoodInfo, Tag>(FOOD.FOOD_TYPES.SURF_AND_TURF, DehydratedSurfAndTurfConfig.ID),
			new ValueTuple<EdiblesManager.FoodInfo, Tag>(FOOD.FOOD_TYPES.SPICEBREAD, DehydratedSpiceBreadConfig.ID),
			new ValueTuple<EdiblesManager.FoodInfo, Tag>(FOOD.FOOD_TYPES.QUICHE, DehydratedQuicheConfig.ID),
			new ValueTuple<EdiblesManager.FoodInfo, Tag>(FOOD.FOOD_TYPES.CURRY, DehydratedCurryConfig.ID),
			new ValueTuple<EdiblesManager.FoodInfo, Tag>(FOOD.FOOD_TYPES.SPICY_TOFU, DehydratedSpicyTofuConfig.ID),
			new ValueTuple<EdiblesManager.FoodInfo, Tag>(FOOD.FOOD_TYPES.BURGER, DehydratedFoodPackageConfig.ID)
		};
		if (DlcManager.IsExpansion1Active())
		{
			list.Add(new ValueTuple<EdiblesManager.FoodInfo, Tag>(FOOD.FOOD_TYPES.BERRY_PIE, DehydratedBerryPieConfig.ID));
		}
		int num = 100;
		foreach (ValueTuple<EdiblesManager.FoodInfo, Tag> valueTuple in list)
		{
			EdiblesManager.FoodInfo item = valueTuple.Item1;
			Tag item2 = valueTuple.Item2;
			ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement(item, 6000000f / item.CaloriesPerUnit, true),
				new ComplexRecipe.RecipeElement(SimHashes.Polypropylene.CreateTag(), 12f)
			};
			ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement(item2, 6f, ComplexRecipe.RecipeElement.TemperatureOperation.Dehydrated, false),
				new ComplexRecipe.RecipeElement(SimHashes.Water.CreateTag(), 6f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
			};
			ComplexRecipe complexRecipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("FoodDehydrator", array, array2), array, array2);
			complexRecipe.time = 250f;
			complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Custom;
			complexRecipe.customName = string.Format(global::STRINGS.BUILDINGS.PREFABS.FOODDEHYDRATOR.RECIPE_NAME, item.Name);
			complexRecipe.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.FOODDEHYDRATOR.RESULT_DESCRIPTION, item.Name);
			complexRecipe.fabricators = new List<Tag> { TagManager.Create("FoodDehydrator") };
			complexRecipe.sortOrder = num;
			num++;
		}
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}

	public const string ID = "FoodDehydrator";

	public ComplexRecipe DehydratedFoodRecipe;

	private static readonly List<Storage.StoredItemModifier> GourmetCookingStationStoredItemModifiers = new List<Storage.StoredItemModifier>
	{
		Storage.StoredItemModifier.Hide,
		Storage.StoredItemModifier.Preserve,
		Storage.StoredItemModifier.Insulate,
		Storage.StoredItemModifier.Seal
	};
}
