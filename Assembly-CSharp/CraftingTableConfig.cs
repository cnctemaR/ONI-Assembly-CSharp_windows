using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class CraftingTableConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "CraftingTable";
		int num = 2;
		int num2 = 2;
		string text2 = "craftingStation_kanim";
		int num3 = 100;
		float num4 = 30f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] raw_METALS = MATERIALS.RAW_METALS;
		float num5 = 800f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER3;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, raw_METALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.NONE, tier2, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 60f;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PowerInputOffset = new CellOffset(1, 0);
		buildingDef.POIUnlockable = true;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<Prioritizable>();
		ComplexFabricator complexFabricator = go.AddOrGet<ComplexFabricator>();
		complexFabricator.heatedTemperature = 318.15f;
		complexFabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		go.AddOrGet<FabricatorIngredientStatusManager>();
		go.AddOrGet<CopyBuildingSettings>();
		go.AddOrGet<ComplexFabricatorWorkable>().overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_craftingstation_kanim") };
		Prioritizable.AddRef(go);
		BuildingTemplates.CreateComplexFabricatorStorage(go, complexFabricator);
		this.ConfigureRecipes();
	}

	private void ConfigureRecipes()
	{
		foreach (Tag tag in GameTags.StartingMetalOres)
		{
			this.CreateMetalMiniVoltRecipe(tag);
		}
		this.CreateMetalMiniVoltRecipe(SimHashes.IronOre.CreateTag());
		if (DlcManager.IsContentSubscribed("DLC3_ID"))
		{
			ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement("BasicForagePlant", 1.2f, true)
			};
			ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement("DisposableElectrobank_BasicSingleHarvestPlant".ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
			};
			string text = ComplexRecipeManager.MakeRecipeID("CraftingTable", array, array2);
			DisposableElectrobankConfig.recipes.Add("DisposableElectrobank_BasicSingleHarvestPlant".ToTag(), new ComplexRecipe(text, array, array2, DlcManager.DLC3)
			{
				time = INDUSTRIAL.RECIPES.STANDARD_FABRICATION_TIME * 2f,
				description = "_description",
				nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
				fabricators = new List<Tag> { "CraftingTable" },
				requiredTech = Db.Get().TechItems.disposableElectrobankOrganic.parentTechId
			});
		}
		if (DlcManager.IsContentSubscribed("DLC3_ID"))
		{
			ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement(SimHashes.Sucrose.CreateTag(), 200f, true)
			};
			ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement("DisposableElectrobank_Sucrose".ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
			};
			string text2 = ComplexRecipeManager.MakeRecipeID("CraftingTable", array3, array4);
			DisposableElectrobankConfig.recipes.Add("DisposableElectrobank_Sucrose".ToTag(), new ComplexRecipe(text2, array3, array4, DlcManager.DLC3)
			{
				time = INDUSTRIAL.RECIPES.STANDARD_FABRICATION_TIME * 2f,
				description = "_description",
				nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
				fabricators = new List<Tag> { "CraftingTable" },
				requiredTech = Db.Get().TechItems.disposableElectrobankOrganic.parentTechId
			});
		}
		if (DlcManager.IsContentSubscribed("DLC3_ID"))
		{
			ComplexRecipe.RecipeElement[] array5 = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement("LightBugEgg", 6f, true)
			};
			ComplexRecipe.RecipeElement[] array6 = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement("DisposableElectrobank_LightBugEgg".ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
			};
			string text3 = ComplexRecipeManager.MakeRecipeID("CraftingTable", array5, array6);
			DisposableElectrobankConfig.recipes.Add("DisposableElectrobank_LightBugEgg".ToTag(), new ComplexRecipe(text3, array5, array6, DlcManager.DLC3)
			{
				time = INDUSTRIAL.RECIPES.STANDARD_FABRICATION_TIME * 2f,
				description = "_description",
				nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
				fabricators = new List<Tag> { "CraftingTable" },
				requiredTech = Db.Get().TechItems.disposableElectrobankOrganic.parentTechId
			});
		}
		if (DlcManager.IsAllContentSubscribed(new string[] { "EXPANSION1_ID", "DLC3_ID" }))
		{
			ComplexRecipe.RecipeElement[] array7 = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement(SimHashes.UraniumOre.CreateTag(), 20f, true)
			};
			ComplexRecipe.RecipeElement[] array8 = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement("DisposableElectrobank_UraniumOre".ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
			};
			string text4 = ComplexRecipeManager.MakeRecipeID("CraftingTable", array7, array8);
			DisposableElectrobankConfig.recipes.Add("DisposableElectrobank_UraniumOre".ToTag(), new ComplexRecipe(text4, array7, array8, new string[] { "EXPANSION1_ID", "DLC3_ID" })
			{
				time = INDUSTRIAL.RECIPES.STANDARD_FABRICATION_TIME * 2f,
				description = "_description",
				nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
				fabricators = new List<Tag> { "CraftingTable" },
				requiredTech = Db.Get().TechItems.disposableElectrobankUraniumOre.parentTechId
			});
		}
		ComplexRecipe.RecipeElement[] array9 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Cuprite.CreateTag(), 50f, true)
		};
		ComplexRecipe.RecipeElement[] array10 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("Oxygen_Mask".ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		AtmoSuitConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CraftingTable", array9, array10), array9, array10)
		{
			time = (float)global::TUNING.EQUIPMENT.SUITS.OXYMASK_FABTIME,
			description = global::STRINGS.EQUIPMENT.PREFABS.OXYGEN_MASK.RECIPE_DESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
			fabricators = new List<Tag> { "CraftingTable" },
			requiredTech = Db.Get().TechItems.oxygenMask.parentTechId
		};
		AtmoSuitConfig.recipe.RequiresAllIngredientsDiscovered = true;
		ComplexRecipe.RecipeElement[] array11 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.AluminumOre.CreateTag(), 50f, true)
		};
		ComplexRecipe.RecipeElement[] array12 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("Oxygen_Mask".ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		AtmoSuitConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CraftingTable", array11, array12), array11, array12)
		{
			time = (float)global::TUNING.EQUIPMENT.SUITS.OXYMASK_FABTIME,
			description = global::STRINGS.EQUIPMENT.PREFABS.OXYGEN_MASK.RECIPE_DESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
			fabricators = new List<Tag> { "CraftingTable" },
			requiredTech = Db.Get().TechItems.oxygenMask.parentTechId
		};
		AtmoSuitConfig.recipe.RequiresAllIngredientsDiscovered = true;
		ComplexRecipe.RecipeElement[] array13 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.IronOre.CreateTag(), 50f, true)
		};
		ComplexRecipe.RecipeElement[] array14 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("Oxygen_Mask".ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		AtmoSuitConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CraftingTable", array13, array14), array13, array14)
		{
			time = (float)global::TUNING.EQUIPMENT.SUITS.OXYMASK_FABTIME,
			description = global::STRINGS.EQUIPMENT.PREFABS.OXYGEN_MASK.RECIPE_DESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
			fabricators = new List<Tag> { "CraftingTable" },
			requiredTech = Db.Get().TechItems.oxygenMask.parentTechId
		};
		AtmoSuitConfig.recipe.RequiresAllIngredientsDiscovered = true;
		if (ElementLoader.FindElementByHash(SimHashes.Cobaltite) != null)
		{
			ComplexRecipe.RecipeElement[] array15 = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement(SimHashes.Cobaltite.CreateTag(), 50f, true)
			};
			ComplexRecipe.RecipeElement[] array16 = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement("Oxygen_Mask".ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
			};
			AtmoSuitConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CraftingTable", array15, array16), array15, array16)
			{
				time = (float)global::TUNING.EQUIPMENT.SUITS.OXYMASK_FABTIME,
				description = global::STRINGS.EQUIPMENT.PREFABS.OXYGEN_MASK.RECIPE_DESC,
				nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
				fabricators = new List<Tag> { "CraftingTable" },
				requiredTech = Db.Get().TechItems.oxygenMask.parentTechId
			};
			AtmoSuitConfig.recipe.RequiresAllIngredientsDiscovered = true;
		}
		ComplexRecipe.RecipeElement[] array17 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Cinnabar.CreateTag(), 50f, true)
		};
		ComplexRecipe.RecipeElement[] array18 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("Oxygen_Mask".ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		AtmoSuitConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CraftingTable", array17, array18), array17, array18)
		{
			time = (float)global::TUNING.EQUIPMENT.SUITS.OXYMASK_FABTIME,
			description = global::STRINGS.EQUIPMENT.PREFABS.OXYGEN_MASK.RECIPE_DESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
			fabricators = new List<Tag> { "CraftingTable" },
			requiredTech = Db.Get().TechItems.oxygenMask.parentTechId
		};
		AtmoSuitConfig.recipe.RequiresAllIngredientsDiscovered = true;
		ComplexRecipe.RecipeElement[] array19 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("Worn_Oxygen_Mask".ToTag(), 1f, true)
		};
		ComplexRecipe.RecipeElement[] array20 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("Oxygen_Mask".ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		AtmoSuitConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CraftingTable", array19, array20), array19, array20)
		{
			time = (float)global::TUNING.EQUIPMENT.SUITS.OXYMASK_FABTIME,
			description = global::STRINGS.EQUIPMENT.PREFABS.OXYGEN_MASK.RECIPE_DESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
			fabricators = new List<Tag> { "CraftingTable" },
			requiredTech = Db.Get().TechItems.oxygenMask.parentTechId
		};
		AtmoSuitConfig.recipe.RequiresAllIngredientsDiscovered = true;
	}

	private void CreateMetalMiniVoltRecipe(Tag inputMetal)
	{
		if (ElementLoader.FindElementByTag(inputMetal) == null)
		{
			return;
		}
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(inputMetal, 100f, true)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("DisposableElectrobank_RawMetal".ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		string text = ComplexRecipeManager.MakeRecipeID("CraftingTable", array, array2);
		Dictionary<Tag, ComplexRecipe> recipes = DisposableElectrobankConfig.recipes;
		string text2 = "DisposableElectrobank_RawMetal".ToTag().ToString();
		string text3 = "_";
		Tag tag = inputMetal;
		recipes.Add(text2 + text3 + tag.ToString(), new ComplexRecipe(text, array, array2, DlcManager.DLC3)
		{
			time = INDUSTRIAL.RECIPES.STANDARD_FABRICATION_TIME * 2f,
			description = "_description",
			nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
			fabricators = new List<Tag> { "CraftingTable" }
		});
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Suits, true);
		};
		go.GetComponent<KPrefabID>().prefabSpawnFn += delegate(GameObject game_object)
		{
			ComplexFabricatorWorkable component = game_object.GetComponent<ComplexFabricatorWorkable>();
			component.WorkerStatusItem = Db.Get().DuplicantStatusItems.Fabricating;
			component.AttributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
			component.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
			component.SkillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
			component.SkillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		};
	}

	public const string ID = "CraftingTable";
}
