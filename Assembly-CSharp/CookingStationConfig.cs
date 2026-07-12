using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class CookingStationConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "CookingStation";
		int num = 3;
		int num2 = 2;
		string text2 = "cookstation_kanim";
		int num3 = 30;
		float num4 = 30f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER3;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, all_METALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.NONE, tier2, 0.2f);
		BuildingTemplates.CreateElectricalBuildingDef(buildingDef);
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "large";
		buildingDef.EnergyConsumptionWhenActive = 60f;
		buildingDef.ExhaustKilowattsWhenActive = 0.5f;
		buildingDef.SelfHeatKilowattsWhenActive = 4f;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		CookingStation cookingStation = go.AddOrGet<CookingStation>();
		cookingStation.heatedTemperature = 368.15f;
		go.AddOrGet<FabricatorIngredientStatusManager>();
		go.AddOrGet<CopyBuildingSettings>();
		go.AddOrGet<ComplexFabricatorWorkable>().overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_cookstation_kanim") };
		cookingStation.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		Prioritizable.AddRef(go);
		go.AddOrGet<DropAllWorkable>();
		this.ConfigureRecipes();
		go.AddOrGetDef<PoweredController.Def>();
		BuildingTemplates.CreateComplexFabricatorStorage(go, cookingStation);
	}

	private void ConfigureRecipes()
	{
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("BasicPlantFood", 3f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("PickledMeal", 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
		};
		PickledMealConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CookingStation", array, array2), array, array2)
		{
			time = FOOD.RECIPES.SMALL_COOK_TIME,
			description = ITEMS.FOOD.PICKLEDMEAL.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag> { "CookingStation" },
			sortOrder = 21
		};
		ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("MushBar", 1f)
		};
		ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("FriedMushBar".ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
		};
		FriedMushBarConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CookingStation", array3, array4), array3, array4)
		{
			time = FOOD.RECIPES.STANDARD_COOK_TIME,
			description = ITEMS.FOOD.FRIEDMUSHBAR.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag> { "CookingStation" },
			sortOrder = 1
		};
		ComplexRecipe.RecipeElement[] array5 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(MushroomConfig.ID, 1f)
		};
		ComplexRecipe.RecipeElement[] array6 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("FriedMushroom", 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
		};
		FriedMushroomConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CookingStation", array5, array6), array5, array6)
		{
			time = FOOD.RECIPES.STANDARD_COOK_TIME,
			description = ITEMS.FOOD.FRIEDMUSHROOM.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag> { "CookingStation" },
			sortOrder = 20
		};
		ComplexRecipe.RecipeElement[] array7 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("Meat", 2f)
		};
		ComplexRecipe.RecipeElement[] array8 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("CookedMeat", 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
		};
		CookedMeatConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CookingStation", array7, array8), array7, array8)
		{
			time = FOOD.RECIPES.STANDARD_COOK_TIME,
			description = ITEMS.FOOD.COOKEDMEAT.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag> { "CookingStation" },
			sortOrder = 21
		};
		ComplexRecipe.RecipeElement[] array9 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("FishMeat", 1f)
		};
		ComplexRecipe.RecipeElement[] array10 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("CookedFish", 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
		};
		CookedMeatConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CookingStation", array9, array10), array9, array10)
		{
			time = FOOD.RECIPES.STANDARD_COOK_TIME,
			description = ITEMS.FOOD.COOKEDMEAT.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag> { "CookingStation" },
			sortOrder = 21
		};
		ComplexRecipe.RecipeElement[] array11 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(PrickleFruitConfig.ID, 1f)
		};
		ComplexRecipe.RecipeElement[] array12 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("GrilledPrickleFruit", 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
		};
		GrilledPrickleFruitConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CookingStation", array11, array12), array11, array12)
		{
			time = FOOD.RECIPES.STANDARD_COOK_TIME,
			description = ITEMS.FOOD.GRILLEDPRICKLEFRUIT.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag> { "CookingStation" },
			sortOrder = 20
		};
		if (DlcManager.IsExpansion1Active())
		{
			ComplexRecipe.RecipeElement[] array13 = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement(SwampFruitConfig.ID, 1f)
			};
			ComplexRecipe.RecipeElement[] array14 = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement("SwampDelights", 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
			};
			CookedEggConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CookingStation", array13, array14), array13, array14)
			{
				time = FOOD.RECIPES.STANDARD_COOK_TIME,
				description = ITEMS.FOOD.SWAMPDELIGHTS.RECIPEDESC,
				nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
				fabricators = new List<Tag> { "CookingStation" },
				sortOrder = 20
			};
		}
		ComplexRecipe.RecipeElement[] array15 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("ColdWheatSeed", 3f)
		};
		ComplexRecipe.RecipeElement[] array16 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("ColdWheatBread", 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
		};
		ColdWheatBreadConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CookingStation", array15, array16), array15, array16)
		{
			time = FOOD.RECIPES.STANDARD_COOK_TIME,
			description = ITEMS.FOOD.COLDWHEATBREAD.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag> { "CookingStation" },
			sortOrder = 50
		};
		ComplexRecipe.RecipeElement[] array17 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("RawEgg", 1f)
		};
		ComplexRecipe.RecipeElement[] array18 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("CookedEgg", 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
		};
		CookedEggConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CookingStation", array17, array18), array17, array18)
		{
			time = FOOD.RECIPES.STANDARD_COOK_TIME,
			description = ITEMS.FOOD.COOKEDEGG.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag> { "CookingStation" },
			sortOrder = 1
		};
		if (DlcManager.IsExpansion1Active())
		{
			ComplexRecipe.RecipeElement[] array19 = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement("WormBasicFruit", 1f)
			};
			ComplexRecipe.RecipeElement[] array20 = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement("WormBasicFood", 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
			};
			WormBasicFoodConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CookingStation", array19, array20), array19, array20)
			{
				time = FOOD.RECIPES.STANDARD_COOK_TIME,
				description = ITEMS.FOOD.WORMBASICFOOD.RECIPEDESC,
				nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
				fabricators = new List<Tag> { "CookingStation" },
				sortOrder = 20
			};
		}
		if (DlcManager.IsExpansion1Active())
		{
			ComplexRecipe.RecipeElement[] array21 = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement("WormSuperFruit", 8f),
				new ComplexRecipe.RecipeElement("Sucrose".ToTag(), 4f)
			};
			ComplexRecipe.RecipeElement[] array22 = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement("WormSuperFood", 1f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
			};
			WormSuperFoodConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CookingStation", array21, array22), array21, array22)
			{
				time = FOOD.RECIPES.STANDARD_COOK_TIME,
				description = ITEMS.FOOD.WORMSUPERFOOD.RECIPEDESC,
				nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
				fabricators = new List<Tag> { "CookingStation" },
				sortOrder = 20
			};
		}
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}

	public const string ID = "CookingStation";
}
