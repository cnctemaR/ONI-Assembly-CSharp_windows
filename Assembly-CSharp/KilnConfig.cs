using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class KilnConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "Kiln";
		int num = 2;
		int num2 = 2;
		string text2 = "kiln_kanim";
		int num3 = 100;
		float num4 = 30f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num5 = 800f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER5;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, all_METALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.PENALTY.TIER1, tier2, 0.2f);
		buildingDef.Overheatable = false;
		buildingDef.RequiresPowerInput = false;
		buildingDef.ExhaustKilowattsWhenActive = 16f;
		buildingDef.SelfHeatKilowattsWhenActive = 4f;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 1));
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<BuildingComplete>().isManuallyOperated = false;
		ComplexFabricator complexFabricator = go.AddOrGet<ComplexFabricator>();
		complexFabricator.heatedTemperature = 353.15f;
		complexFabricator.duplicantOperated = false;
		complexFabricator.showProgressBar = true;
		complexFabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		go.AddOrGet<FabricatorIngredientStatusManager>();
		go.AddOrGet<CopyBuildingSettings>();
		BuildingTemplates.CreateComplexFabricatorStorage(go, complexFabricator);
		this.ConfigureRecipes();
		Prioritizable.AddRef(go);
	}

	private void ConfigureRecipes()
	{
		Tag tag = SimHashes.Ceramic.CreateTag();
		Tag tag2 = SimHashes.Clay.CreateTag();
		Tag tag3 = SimHashes.Carbon.CreateTag();
		float num = 100f;
		float num2 = 25f;
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(tag2, num),
			new ComplexRecipe.RecipeElement(tag3, num2)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(tag, num, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
		};
		string text = ComplexRecipeManager.MakeObsoleteRecipeID("Kiln", tag);
		string text2 = ComplexRecipeManager.MakeRecipeID("Kiln", array, array2);
		ComplexRecipe complexRecipe = new ComplexRecipe(text2, array, array2);
		complexRecipe.time = 40f;
		complexRecipe.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.EGGCRACKER.RECIPE_DESCRIPTION, ElementLoader.FindElementByHash(SimHashes.Clay).name, ElementLoader.FindElementByHash(SimHashes.Ceramic).name);
		complexRecipe.fabricators = new List<Tag> { TagManager.Create("Kiln") };
		complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
		complexRecipe.sortOrder = 100;
		ComplexRecipeManager.Get().AddObsoleteIDMapping(text, text2);
		Tag tag4 = SimHashes.RefinedCarbon.CreateTag();
		ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(tag3, num + num2)
		};
		ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(tag4, num, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
		};
		string text3 = ComplexRecipeManager.MakeObsoleteRecipeID("Kiln", tag4);
		string text4 = ComplexRecipeManager.MakeRecipeID("Kiln", array3, array4);
		ComplexRecipe complexRecipe2 = new ComplexRecipe(text4, array3, array4);
		complexRecipe2.time = 40f;
		complexRecipe2.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.EGGCRACKER.RECIPE_DESCRIPTION, ElementLoader.FindElementByHash(SimHashes.Carbon).name, ElementLoader.FindElementByHash(SimHashes.RefinedCarbon).name);
		complexRecipe2.fabricators = new List<Tag> { TagManager.Create("Kiln") };
		complexRecipe2.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
		complexRecipe2.sortOrder = 200;
		ComplexRecipeManager.Get().AddObsoleteIDMapping(text3, text4);
		Tag tag5 = SimHashes.RefinedCarbon.CreateTag();
		ComplexRecipe.RecipeElement[] array5 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.WoodLog.CreateTag(), 100f)
		};
		ComplexRecipe.RecipeElement[] array6 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(tag5, 50f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, false)
		};
		string text5 = ComplexRecipeManager.MakeObsoleteRecipeID("Kiln", tag5);
		string text6 = ComplexRecipeManager.MakeRecipeID("Kiln", array5, array6);
		ComplexRecipe complexRecipe3 = new ComplexRecipe(text6, array5, array6);
		complexRecipe3.time = 40f;
		complexRecipe3.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.EGGCRACKER.RECIPE_DESCRIPTION, ElementLoader.FindElementByHash(SimHashes.WoodLog).name, ElementLoader.FindElementByHash(SimHashes.RefinedCarbon).name);
		complexRecipe3.fabricators = new List<Tag> { TagManager.Create("Kiln") };
		complexRecipe3.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
		complexRecipe3.sortOrder = 300;
		ComplexRecipeManager.Get().AddObsoleteIDMapping(text5, text6);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicOperationalController>();
		go.AddOrGetDef<PoweredActiveController.Def>();
		SymbolOverrideControllerUtil.AddToPrefab(go);
	}

	public const string ID = "Kiln";

	public const float INPUT_CLAY_PER_SECOND = 1f;

	public const float CERAMIC_PER_SECOND = 1f;

	public const float CO2_RATIO = 0.1f;

	public const float OUTPUT_TEMP = 353.15f;

	public const float REFILL_RATE = 2400f;

	public const float CERAMIC_STORAGE_AMOUNT = 2400f;

	public const float COAL_RATE = 0.1f;
}
