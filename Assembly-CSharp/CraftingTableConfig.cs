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
		buildingDef.AddSearchTerms(SEARCH_TERMS.BIONIC);
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
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		go.AddOrGet<ComplexFabricatorWorkable>().overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_craftingstation_kanim") };
		Prioritizable.AddRef(go);
		BuildingTemplates.CreateComplexFabricatorStorage(go, complexFabricator);
		this.ConfigureRecipes();
	}

	private void ConfigureRecipes()
	{
		List<Tag> list = new List<Tag>();
		list.AddRange(GameTags.StartingMetalOres);
		list.Add(SimHashes.IronOre.CreateTag());
		this.CreateMetalMiniVoltRecipe(list.ToArray());
		if (DlcManager.IsAllContentSubscribed(new string[] { "EXPANSION1_ID", "DLC3_ID" }))
		{
			ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement(SimHashes.UraniumOre.CreateTag(), 10f, true)
			};
			ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement("DisposableElectrobank_UraniumOre".ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
			};
			ComplexRecipe complexRecipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CraftingTable", array, array2), array, array2, new string[] { "DLC3_ID" });
			complexRecipe.time = INDUSTRIAL.RECIPES.STANDARD_FABRICATION_TIME * 2f;
			complexRecipe.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.CRAFTINGTABLE.RECIPE_DESCRIPTION, ElementLoader.FindElementByHash(SimHashes.UraniumOre).name, global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.ELECTROBANK_URANIUM_ORE.NAME);
			complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
			complexRecipe.fabricators = new List<Tag> { "CraftingTable" };
			complexRecipe.requiredTech = Db.Get().TechItems.disposableElectrobankUraniumOre.parentTechId;
			complexRecipe.sortOrder = 0;
		}
		ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(GameTags.BasicMetalOres, 50f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, "", true, false)
		};
		ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("Oxygen_Mask".ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		ComplexRecipe complexRecipe2 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CraftingTable", array3, array4), array3, array4);
		complexRecipe2.time = (float)global::TUNING.EQUIPMENT.SUITS.OXYMASK_FABTIME;
		complexRecipe2.description = global::STRINGS.EQUIPMENT.PREFABS.OXYGEN_MASK.RECIPE_DESC;
		complexRecipe2.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
		complexRecipe2.fabricators = new List<Tag> { "CraftingTable" };
		complexRecipe2.requiredTech = Db.Get().TechItems.oxygenMask.parentTechId;
		complexRecipe2.sortOrder = 2;
		ComplexRecipe.RecipeElement[] array5 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("Worn_Oxygen_Mask".ToTag(), 1f, true)
		};
		ComplexRecipe.RecipeElement[] array6 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("Oxygen_Mask".ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		ComplexRecipe complexRecipe3 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CraftingTable", array5, array6), array5, array6);
		complexRecipe3.time = (float)global::TUNING.EQUIPMENT.SUITS.OXYMASK_FABTIME;
		complexRecipe3.description = global::STRINGS.EQUIPMENT.PREFABS.OXYGEN_MASK.REPAIR_WORN_DESC;
		complexRecipe3.nameDisplay = ComplexRecipe.RecipeNameDisplay.Custom;
		complexRecipe3.fabricators = new List<Tag> { "CraftingTable" };
		complexRecipe3.requiredTech = Db.Get().TechItems.oxygenMask.parentTechId;
		complexRecipe3.sortOrder = 2;
		complexRecipe3.customName = global::STRINGS.EQUIPMENT.PREFABS.OXYGEN_MASK.REPAIR_WORN_RECIPE_NAME;
	}

	private void CreateMetalMiniVoltRecipe(Tag[] inputMetals)
	{
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(inputMetals, 200f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, "", false, true)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("DisposableElectrobank_RawMetal".ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		ComplexRecipe complexRecipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("CraftingTable", array, array2), array, array2, DlcManager.DLC3);
		complexRecipe.time = INDUSTRIAL.RECIPES.STANDARD_FABRICATION_TIME * 2f;
		complexRecipe.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.CRAFTINGTABLE.RECIPE_DESCRIPTION, MISC.TAGS.METAL, global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.ELECTROBANK_METAL_ORE.NAME);
		complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
		complexRecipe.fabricators = new List<Tag> { "CraftingTable" };
		complexRecipe.sortOrder = 0;
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
			DiscoveredResources.Instance.Discover("Worn_Oxygen_Mask");
		};
	}

	public const string ID = "CraftingTable";
}
