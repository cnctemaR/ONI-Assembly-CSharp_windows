using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class SushiBarConfig : IBuildingConfig
{
	public override string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC5;
	}

	public override BuildingDef CreateBuildingDef()
	{
		string text = "SushiBar";
		int num = 4;
		int num2 = 2;
		string text2 = "sushi_station_kanim";
		int num3 = 30;
		float num4 = 30f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] woods = MATERIALS.WOODS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER3;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, woods, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.NONE, tier2, 0.2f);
		BuildingTemplates.CreateElectricalBuildingDef(buildingDef);
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "large";
		buildingDef.RequiresPowerInput = false;
		buildingDef.EnergyConsumptionWhenActive = 0f;
		buildingDef.ExhaustKilowattsWhenActive = 0f;
		buildingDef.SelfHeatKilowattsWhenActive = 0.5f;
		buildingDef.RequiredSkillPerkID = Db.Get().SkillPerks.CanSushiBar.Id;
		buildingDef.AddSearchTerms(SEARCH_TERMS.FOOD);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		SushiBar sushiBar = go.AddOrGet<SushiBar>();
		go.AddOrGet<FabricatorIngredientStatusManager>();
		go.AddOrGet<CopyBuildingSettings>();
		ComplexFabricatorLayeredWorkable complexFabricatorLayeredWorkable = go.AddOrGet<ComplexFabricatorLayeredWorkable>();
		complexFabricatorLayeredWorkable.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_cookstation_kanim") };
		complexFabricatorLayeredWorkable.foregroundLayer = Grid.SceneLayer.TransferArm;
		complexFabricatorLayeredWorkable.synchronizeAnims = true;
		sushiBar.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		Prioritizable.AddRef(go);
		go.AddOrGet<DropAllWorkable>();
		this.ConfigureRecipes();
		BuildingTemplates.CreateComplexFabricatorStorage(go, sushiBar);
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.CookTop, false);
	}

	private void ConfigureRecipes()
	{
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("BeanPlantSeed", 1f),
			new ComplexRecipe.RecipeElement("SaltySticksFood", 1f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("Edamame", 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		EdamameConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("SushiBar", array, array2), array, array2)
		{
			time = FOOD.RECIPES.STANDARD_COOK_TIME,
			description = global::STRINGS.ITEMS.FOOD.EDAMAME.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag> { "SushiBar" },
			sortOrder = 1
		};
		ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("BasicPlantBar", 1f),
			new ComplexRecipe.RecipeElement("Nori", 1f),
			new ComplexRecipe.RecipeElement("FishMeat", 1f)
		};
		ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("Maki", 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		MakiConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("SushiBar", array3, array4), array3, array4)
		{
			time = FOOD.RECIPES.STANDARD_COOK_TIME,
			description = global::STRINGS.ITEMS.FOOD.MAKI.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag> { "SushiBar" },
			sortOrder = 2
		};
		ComplexRecipe.RecipeElement[] array5 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("Urchin", 1f)
		};
		ComplexRecipe.RecipeElement[] array6 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(UrchinMeatConfig.ID, 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		UrchinMeatConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("SushiBar", array5, array6), array5, array6)
		{
			time = FOOD.RECIPES.STANDARD_COOK_TIME,
			description = global::STRINGS.ITEMS.FOOD.URCHINMEAT.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag> { "SushiBar" },
			sortOrder = 5
		};
		ComplexRecipe.RecipeElement[] array7 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("BasicPlantBar", 1f),
			new ComplexRecipe.RecipeElement("Nori", 1f),
			new ComplexRecipe.RecipeElement("SquidMeat", 1f)
		};
		ComplexRecipe.RecipeElement[] array8 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("Nigiri", 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		NigiriConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("SushiBar", array7, array8), array7, array8)
		{
			time = FOOD.RECIPES.STANDARD_COOK_TIME,
			description = global::STRINGS.ITEMS.FOOD.NIGIRI.RECIPEDESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag> { "SushiBar" },
			sortOrder = 3
		};
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}

	public const string ID = "SushiBar";
}
