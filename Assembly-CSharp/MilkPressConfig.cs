using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class MilkPressConfig : IBuildingConfig
{
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
		buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(1, 0));
		buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.AudioSize = "medium";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LogicOperationalController>();
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
		complexFabricator.buildStorage.SetDefaultStoredItemModifiers(MilkPressConfig.RefineryStoredItemModifiers);
		complexFabricator.outStorage.SetDefaultStoredItemModifiers(MilkPressConfig.RefineryStoredItemModifiers);
		complexFabricator.storeProduced = false;
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
			new ComplexRecipe.RecipeElement(new Tag[]
			{
				SimHashes.Water.CreateTag(),
				SimHashes.Mucus.CreateTag()
			}, 15f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Milk.CreateTag(), 20f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, true)
		};
		ComplexRecipe complexRecipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("MilkPress", array, array2), array, array2, 0, 0);
		complexRecipe.time = 40f;
		complexRecipe.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.MILKPRESS.WHEAT_MILK_RECIPE_DESCRIPTION, global::STRINGS.ITEMS.FOOD.COLDWHEATSEED.NAME, SimHashes.Milk.CreateTag().ProperName());
		complexRecipe.fabricators = new List<Tag> { TagManager.Create("MilkPress") };
		complexRecipe.sortOrder = 1;
		complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Custom;
		complexRecipe.customName = GameUtil.SafeStringFormat(UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_FROM_TO, new object[]
		{
			global::STRINGS.CREATURES.SPECIES.SEEDS.COLDWHEAT.NAME,
			SimHashes.Milk.CreateTag().ProperName()
		});
		complexRecipe.customSpritePrefabID = "Milk";
		ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SpiceNutConfig.ID, 3f),
			new ComplexRecipe.RecipeElement(new Tag[]
			{
				SimHashes.Water.CreateTag(),
				SimHashes.Mucus.CreateTag()
			}, 17f)
		};
		ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Milk.CreateTag(), 20f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, true)
		};
		complexRecipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("MilkPress", array3, array4), array3, array4, 0, 0);
		complexRecipe.time = 40f;
		complexRecipe.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.MILKPRESS.NUT_MILK_RECIPE_DESCRIPTION, global::STRINGS.ITEMS.FOOD.SPICENUT.NAME, SimHashes.Milk.CreateTag().ProperName());
		complexRecipe.fabricators = new List<Tag> { TagManager.Create("MilkPress") };
		complexRecipe.sortOrder = 1;
		complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Custom;
		complexRecipe.customName = GameUtil.SafeStringFormat(UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_FROM_TO, new object[]
		{
			global::STRINGS.ITEMS.FOOD.SPICENUT.NAME,
			SimHashes.Milk.CreateTag().ProperName()
		});
		complexRecipe.customSpritePrefabID = "Milk";
		ComplexRecipe.RecipeElement[] array5 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("BeanPlantSeed", 2f),
			new ComplexRecipe.RecipeElement(new Tag[]
			{
				SimHashes.Water.CreateTag(),
				SimHashes.Mucus.CreateTag()
			}, 18f)
		};
		ComplexRecipe.RecipeElement[] array6 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Milk.CreateTag(), 20f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, true)
		};
		complexRecipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("MilkPress", array5, array6), array5, array6, 0, 0);
		complexRecipe.time = 40f;
		complexRecipe.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.MILKPRESS.NUT_MILK_RECIPE_DESCRIPTION, global::STRINGS.ITEMS.FOOD.BEANPLANTSEED.NAME, SimHashes.Milk.CreateTag().ProperName());
		complexRecipe.fabricators = new List<Tag> { TagManager.Create("MilkPress") };
		complexRecipe.sortOrder = 1;
		complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Custom;
		complexRecipe.customName = GameUtil.SafeStringFormat(UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_FROM_TO, new object[]
		{
			global::STRINGS.CREATURES.SPECIES.SEEDS.BEAN_PLANT.NAME,
			SimHashes.Milk.CreateTag().ProperName()
		});
		complexRecipe.customSpritePrefabID = "Milk";
		if (DlcManager.IsContentSubscribed("DLC4_ID"))
		{
			ComplexRecipe.RecipeElement[] array7 = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement(DewDripConfig.ID, 2f)
			};
			ComplexRecipe.RecipeElement[] array8 = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement(SimHashes.Milk.CreateTag(), 20f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, true)
			};
			complexRecipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("MilkPress", array7, array8), array7, array8, 0, 0);
			complexRecipe.time = 40f;
			complexRecipe.description = GameUtil.SafeStringFormat(global::STRINGS.BUILDINGS.PREFABS.MILKPRESS.DEWDRIPPER_MILK_RECIPE_DESCRIPTION, new object[]
			{
				global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.DEWDRIP.NAME,
				SimHashes.Milk.CreateTag().ProperName()
			});
			complexRecipe.fabricators = new List<Tag> { TagManager.Create("MilkPress") };
			complexRecipe.sortOrder = 2;
			complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Custom;
			complexRecipe.customName = GameUtil.SafeStringFormat(UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_FROM_TO, new object[]
			{
				global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.DEWDRIP.NAME,
				SimHashes.Milk.CreateTag().ProperName()
			});
			complexRecipe.customSpritePrefabID = "Milk";
		}
		ComplexRecipe.RecipeElement[] array9 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.SlimeMold.CreateTag(), 100f)
		};
		ComplexRecipe.RecipeElement[] array10 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.PhytoOil.CreateTag(), 70f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, true),
			new ComplexRecipe.RecipeElement(SimHashes.Dirt.CreateTag(), 30f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		ComplexRecipe complexRecipe2 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("MilkPress", array9, array10), array9, array10, 0, 0);
		complexRecipe2.time = 40f;
		complexRecipe2.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.MILKPRESS.PHYTO_OIL_RECIPE_DESCRIPTION, ELEMENTS.SLIMEMOLD.NAME, SimHashes.PhytoOil.CreateTag().ProperName(), SimHashes.Dirt.CreateTag().ProperName());
		complexRecipe2.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
		complexRecipe2.fabricators = new List<Tag> { TagManager.Create("MilkPress") };
		complexRecipe2.sortOrder = 20;
		if (DlcManager.IsContentSubscribed("DLC4_ID"))
		{
			float num = 100f;
			ComplexRecipe.RecipeElement[] array11 = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement(KelpConfig.ID, num * 0.25f),
				new ComplexRecipe.RecipeElement(SimHashes.Water.CreateTag(), num * 0.75f)
			};
			ComplexRecipe.RecipeElement[] array12 = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement(SimHashes.PhytoOil.CreateTag(), num, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, true)
			};
			complexRecipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("MilkPress", array11, array12), array11, array12, 0, 0, DlcManager.DLC4);
			complexRecipe.time = 40f;
			complexRecipe.description = GameUtil.SafeStringFormat(global::STRINGS.BUILDINGS.PREFABS.MILKPRESS.KELP_TO_PHYTO_OIL_RECIPE_DESCRIPTION, new object[]
			{
				global::STRINGS.ITEMS.INGREDIENTS.KELP.NAME,
				SimHashes.PhytoOil.CreateTag().ProperName()
			});
			complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
			complexRecipe.fabricators = new List<Tag> { TagManager.Create("MilkPress") };
			complexRecipe.sortOrder = 20;
		}
		float num2 = 100f;
		float num3 = 0.5f;
		float num4 = 0.25f;
		float num5 = 0.25f;
		ComplexRecipe.RecipeElement[] array13 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Amber.CreateTag(), num2)
		};
		ComplexRecipe.RecipeElement[] array14 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.NaturalResin.CreateTag(), num3 * num2, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, true),
			new ComplexRecipe.RecipeElement(SimHashes.Fossil.CreateTag(), num4 * num2, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false),
			new ComplexRecipe.RecipeElement(SimHashes.Sand.CreateTag(), num5 * num2, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		complexRecipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("MilkPress", array13, array14), array13, array14, DlcManager.DLC4);
		complexRecipe.time = 40f;
		complexRecipe.description = GameUtil.SafeStringFormat(global::STRINGS.BUILDINGS.PREFABS.MILKPRESS.RESIN_FROM_AMBER_RECIPE_DESCRIPTION, new object[]
		{
			SimHashes.Amber.CreateTag().ProperName(),
			SimHashes.NaturalResin.CreateTag().ProperName(),
			SimHashes.Fossil.CreateTag().ProperName(),
			SimHashes.Sand.CreateTag().ProperName()
		});
		complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
		complexRecipe.fabricators = new List<Tag> { TagManager.Create("MilkPress") };
		complexRecipe.sortOrder = 30;
		if (DlcManager.IsContentSubscribed("DLC5_ID"))
		{
			float num6 = 100f;
			float num7 = 50f;
			ComplexRecipe.RecipeElement[] array15 = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement(SimHashes.PalmWood.CreateTag(), num6)
			};
			ComplexRecipe.RecipeElement[] array16 = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement(SimHashes.Latex.CreateTag(), num7, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, true)
			};
			complexRecipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("MilkPress", array15, array16), array15, array16, 0, 0, DlcManager.DLC5);
			complexRecipe.time = 40f;
			complexRecipe.description = GameUtil.SafeStringFormat(global::STRINGS.BUILDINGS.PREFABS.MILKPRESS.PALMWOOD_TO_LATEX_RECIPE_DESCRIPTION, new object[]
			{
				SimHashes.PalmWood.CreateTag().ProperName(),
				SimHashes.Latex.CreateTag().ProperName()
			});
			complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
			complexRecipe.fabricators = new List<Tag> { TagManager.Create("MilkPress") };
			complexRecipe.sortOrder = 40;
		}
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
