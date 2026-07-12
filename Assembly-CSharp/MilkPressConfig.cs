using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class MilkPressConfig : IBuildingConfig
{
	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public override BuildingDef CreateBuildingDef()
	{
		string text = "MilkPress";
		int num = 2;
		int num2 = 3;
		string text2 = "milkpress_kanim";
		int num3 = 100;
		float num4 = 30f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] all_MINERALS = MATERIALS.ALL_MINERALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER4;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, all_MINERALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.PENALTY.TIER1, tier2, 0.2f);
		buildingDef.RequiresPowerInput = false;
		buildingDef.EnergyConsumptionWhenActive = 0f;
		buildingDef.SelfHeatKilowattsWhenActive = 2f;
		buildingDef.OutputConduitType = ConduitType.Liquid;
		buildingDef.UtilityOutputOffset = new CellOffset(1, 0);
		buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.AudioSize = "Metal";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		ComplexFabricator complexFabricator = go.AddOrGet<ComplexFabricator>();
		complexFabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		complexFabricator.duplicantOperated = true;
		go.AddOrGet<FabricatorIngredientStatusManager>();
		go.AddOrGet<CopyBuildingSettings>();
		ComplexFabricatorWorkable complexFabricatorWorkable = go.AddOrGet<ComplexFabricatorWorkable>();
		BuildingTemplates.CreateComplexFabricatorStorage(go, complexFabricator);
		complexFabricatorWorkable.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_milkpress_kanim") };
		complexFabricatorWorkable.workingPstComplete = new HashedString[] { "working_pst_complete" };
		complexFabricator.storeProduced = true;
		complexFabricator.inStorage.SetDefaultStoredItemModifiers(MilkPressConfig.RefineryStoredItemModifiers);
		complexFabricator.outStorage.SetDefaultStoredItemModifiers(MilkPressConfig.RefineryStoredItemModifiers);
		ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
		conduitDispenser.conduitType = ConduitType.Liquid;
		conduitDispenser.alwaysDispense = true;
		conduitDispenser.elementFilter = null;
		conduitDispenser.storage = go.GetComponent<ComplexFabricator>().outStorage;
		this.AddRecipes(go);
		Prioritizable.AddRef(go);
	}

	private void AddRecipes(GameObject go)
	{
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("ColdWheatSeed", 10f),
			new ComplexRecipe.RecipeElement(SimHashes.Water.CreateTag(), 15f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Milk.CreateTag(), 20f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		ComplexRecipe complexRecipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("MilkPress", array, array2), array, array2, 0, 0);
		complexRecipe.time = 40f;
		complexRecipe.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.MILKPRESS.WHEAT_MILK_RECIPE_DESCRIPTION, ITEMS.FOOD.COLDWHEATSEED.NAME, SimHashes.Milk.CreateTag().ProperName());
		complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
		complexRecipe.fabricators = new List<Tag> { TagManager.Create("MilkPress") };
		ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SpiceNutConfig.ID, 3f),
			new ComplexRecipe.RecipeElement(SimHashes.Water.CreateTag(), 17f)
		};
		ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Milk.CreateTag(), 20f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		ComplexRecipe complexRecipe2 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("MilkPress", array3, array4), array3, array4, 0, 0);
		complexRecipe2.time = 40f;
		complexRecipe2.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.MILKPRESS.NUT_MILK_RECIPE_DESCRIPTION, ITEMS.FOOD.SPICENUT.NAME, SimHashes.Milk.CreateTag().ProperName());
		complexRecipe2.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
		complexRecipe2.fabricators = new List<Tag> { TagManager.Create("MilkPress") };
		ComplexRecipe.RecipeElement[] array5 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("BeanPlantSeed", 2f),
			new ComplexRecipe.RecipeElement(SimHashes.Water.CreateTag(), 18f)
		};
		ComplexRecipe.RecipeElement[] array6 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Milk.CreateTag(), 20f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		ComplexRecipe complexRecipe3 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("MilkPress", array5, array6), array5, array6, 0, 0);
		complexRecipe3.time = 40f;
		complexRecipe3.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.MILKPRESS.NUT_MILK_RECIPE_DESCRIPTION, ITEMS.FOOD.SPICENUT.NAME, SimHashes.Milk.CreateTag().ProperName());
		complexRecipe3.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
		complexRecipe3.fabricators = new List<Tag> { TagManager.Create("MilkPress") };
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		SymbolOverrideControllerUtil.AddToPrefab(go);
		go.GetComponent<KPrefabID>().prefabSpawnFn += delegate(GameObject game_object)
		{
			ComplexFabricatorWorkable component = game_object.GetComponent<ComplexFabricatorWorkable>();
			component.WorkerStatusItem = Db.Get().DuplicantStatusItems.Processing;
			component.AttributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
			component.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
			component.SkillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
			component.SkillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		};
	}

	public const string ID = "MilkPress";

	private static readonly List<Storage.StoredItemModifier> RefineryStoredItemModifiers = new List<Storage.StoredItemModifier>
	{
		Storage.StoredItemModifier.Hide,
		Storage.StoredItemModifier.Preserve,
		Storage.StoredItemModifier.Insulate,
		Storage.StoredItemModifier.Seal
	};
}
