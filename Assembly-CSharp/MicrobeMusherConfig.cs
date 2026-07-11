using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class MicrobeMusherConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "MicrobeMusher";
		int num = 2;
		int num2 = 3;
		string text2 = "microbemusher_kanim";
		int num3 = 30;
		float num4 = 30f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num5 = 800f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER3;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, all_METALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.PENALTY.TIER2, tier2, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 240f;
		buildingDef.ExhaustKilowattsWhenActive = 0.5f;
		buildingDef.SelfHeatKilowattsWhenActive = 2f;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "Glass";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<DropAllWorkable>();
		Prioritizable.AddRef(go);
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		MicrobeMusher microbeMusher = go.AddOrGet<MicrobeMusher>();
		microbeMusher.mushbarSpawnOffset = new Vector3(1f, 0f, 0f);
		go.AddOrGet<FabricatorIngredientStatusManager>();
		go.AddOrGet<CopyBuildingSettings>();
		go.AddOrGet<ComplexFabricatorWorkable>().overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_musher_kanim") };
		microbeMusher.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		BuildingTemplates.CreateComplexFabricatorStorage(go, microbeMusher);
		this.ConfigureRecipes();
		go.AddOrGetDef<PoweredController.Def>();
	}

	private void ConfigureRecipes()
	{
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("Dirt".ToTag(), 75f),
			new ComplexRecipe.RecipeElement("Water".ToTag(), 75f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("MushBar".ToTag(), 1f)
		};
		string text = ComplexRecipeManager.MakeRecipeID("MicrobeMusher", array, array2);
		MushBarConfig.recipe = new ComplexRecipe(text, array, array2)
		{
			time = 40f,
			description = ITEMS.FOOD.MUSHBAR.RECIPEDESC,
			useResultAsDescription = true,
			fabricators = new List<Tag> { "MicrobeMusher" },
			sortOrder = 1
		};
		ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("BasicPlantFood", 2f),
			new ComplexRecipe.RecipeElement("Water".ToTag(), 50f)
		};
		ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("BasicPlantBar".ToTag(), 1f)
		};
		string text2 = ComplexRecipeManager.MakeRecipeID("MicrobeMusher", array3, array4);
		BasicPlantBarConfig.recipe = new ComplexRecipe(text2, array3, array4)
		{
			time = FOOD.RECIPES.STANDARD_COOK_TIME,
			description = ITEMS.FOOD.BASICPLANTBAR.RECIPEDESC,
			useResultAsDescription = true,
			fabricators = new List<Tag> { "MicrobeMusher" },
			sortOrder = 2
		};
		ComplexRecipe.RecipeElement[] array5 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("ColdWheatSeed", 5f),
			new ComplexRecipe.RecipeElement(PrickleFruitConfig.ID, 1f)
		};
		ComplexRecipe.RecipeElement[] array6 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("FruitCake".ToTag(), 1f)
		};
		string text3 = ComplexRecipeManager.MakeRecipeID("MicrobeMusher", array5, array6);
		FruitCakeConfig.recipe = new ComplexRecipe(text3, array5, array6)
		{
			time = FOOD.RECIPES.STANDARD_COOK_TIME,
			description = ITEMS.FOOD.FRUITCAKE.RECIPEDESC,
			useResultAsDescription = true,
			fabricators = new List<Tag> { "MicrobeMusher" },
			sortOrder = 3
		};
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}

	public const string ID = "MicrobeMusher";
}
