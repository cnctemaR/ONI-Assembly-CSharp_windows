using System;
using System.Collections.Generic;
using System.Linq;
using STRINGS;
using TUNING;
using UnityEngine;

public class RockCrusherConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "RockCrusher";
		int num = 4;
		int num2 = 4;
		string text2 = "rockrefinery_kanim";
		int num3 = 30;
		float num4 = 60f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num5 = 2400f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER6;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, all_METALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.PENALTY.TIER2, tier2, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 240f;
		buildingDef.SelfHeatKilowattsWhenActive = 16f;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.AudioSize = "large";
		buildingDef.AddSearchTerms(SEARCH_TERMS.METAL);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		ComplexFabricator complexFabricator = go.AddOrGet<ComplexFabricator>();
		complexFabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		complexFabricator.duplicantOperated = true;
		go.AddOrGet<FabricatorIngredientStatusManager>();
		go.AddOrGet<CopyBuildingSettings>();
		ComplexFabricatorWorkable complexFabricatorWorkable = go.AddOrGet<ComplexFabricatorWorkable>();
		BuildingTemplates.CreateComplexFabricatorStorage(go, complexFabricator);
		complexFabricatorWorkable.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_rockrefinery_kanim") };
		complexFabricatorWorkable.workingPstComplete = new HashedString[] { "working_pst_complete" };
		Tag tag = SimHashes.Sand.CreateTag();
		Tag[] array = (from e in ElementLoader.elements.FindAll((Element e) => e.HasTag(GameTags.Crushable))
			select e.tag).ToArray<Tag>();
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(array, 100f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, "", false, false)
		};
		ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(tag, 100f)
		};
		ComplexRecipe complexRecipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("RockCrusher", array2, array3), array2, array3);
		complexRecipe.time = 40f;
		complexRecipe.description = global::STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.SAND_FROM_RAW_MINERAL_DESCRIPTION;
		complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Custom;
		complexRecipe.customName = global::STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.SAND_FROM_RAW_MINERAL_NAME;
		complexRecipe.fabricators = new List<Tag> { TagManager.Create("RockCrusher") };
		complexRecipe.sortOrder = 0;
		foreach (Element element in ElementLoader.elements.FindAll((Element e) => e.IsSolid && e.HasTag(GameTags.Metal)))
		{
			if (!element.HasTag(GameTags.Noncrushable))
			{
				Element lowTempTransition = element.highTempTransition.lowTempTransition;
				if (lowTempTransition != element)
				{
					ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[]
					{
						new ComplexRecipe.RecipeElement(element.tag, 100f)
					};
					ComplexRecipe.RecipeElement[] array5;
					if (element.HasTag(GameTags.UseSmeltingByproducts) && element.highTempTransitionOreID != SimHashes.Vacuum && element.highTempTransitionOreMassConversion > 0f)
					{
						Element element2 = ElementLoader.FindElementByHash(element.highTempTransitionOreID);
						float highTempTransitionOreMassConversion = element.highTempTransitionOreMassConversion;
						float num = 50f;
						float num2 = num * highTempTransitionOreMassConversion;
						float num3 = num - num2;
						float num4 = 50f;
						array5 = new ComplexRecipe.RecipeElement[]
						{
							new ComplexRecipe.RecipeElement(lowTempTransition.tag, num3),
							new ComplexRecipe.RecipeElement(element2.tag, num2, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false),
							new ComplexRecipe.RecipeElement(tag, num4, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
						};
					}
					else
					{
						array5 = new ComplexRecipe.RecipeElement[]
						{
							new ComplexRecipe.RecipeElement(lowTempTransition.tag, 50f),
							new ComplexRecipe.RecipeElement(tag, 50f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
						};
					}
					string text = ComplexRecipeManager.MakeObsoleteRecipeID("RockCrusher", lowTempTransition.tag);
					string text2 = ComplexRecipeManager.MakeRecipeID("RockCrusher", array4, array5);
					ComplexRecipe complexRecipe2 = new ComplexRecipe(text2, array4, array5);
					complexRecipe2.time = 40f;
					complexRecipe2.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.METAL_RECIPE_DESCRIPTION, lowTempTransition.name, element.name);
					complexRecipe2.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
					complexRecipe2.fabricators = new List<Tag> { TagManager.Create("RockCrusher") };
					complexRecipe2.sortOrder = 1;
					ComplexRecipeManager.Get().AddObsoleteIDMapping(text, text2);
				}
			}
		}
		Element element3 = ElementLoader.FindElementByHash(SimHashes.Lime);
		ComplexRecipe.RecipeElement[] array6 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("EggShell", 5f)
		};
		ComplexRecipe.RecipeElement[] array7 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Lime).tag, 5f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		string text3 = ComplexRecipeManager.MakeObsoleteRecipeID("RockCrusher", element3.tag);
		string text4 = ComplexRecipeManager.MakeRecipeID("RockCrusher", array6, array7);
		ComplexRecipe complexRecipe3 = new ComplexRecipe(text4, array6, array7);
		complexRecipe3.time = 40f;
		complexRecipe3.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.LIME_RECIPE_DESCRIPTION, SimHashes.Lime.CreateTag().ProperName(), MISC.TAGS.EGGSHELL);
		complexRecipe3.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
		complexRecipe3.fabricators = new List<Tag> { TagManager.Create("RockCrusher") };
		complexRecipe3.sortOrder = 4;
		ComplexRecipeManager.Get().AddObsoleteIDMapping(text3, text4);
		Element element4 = ElementLoader.FindElementByHash(SimHashes.Lime);
		ComplexRecipe.RecipeElement[] array8 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("CrabShell", 60f)
		};
		ComplexRecipe.RecipeElement[] array9 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(element4.tag, 60f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		ComplexRecipe complexRecipe4 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("RockCrusher", array8, array9), array8, array9);
		complexRecipe4.time = 40f;
		complexRecipe4.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.LIME_RECIPE_DESCRIPTION, SimHashes.Lime.CreateTag().ProperName(), global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.CRAB_SHELL.NAME);
		complexRecipe4.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
		complexRecipe4.fabricators = new List<Tag> { TagManager.Create("RockCrusher") };
		complexRecipe4.sortOrder = 4;
		float num5 = 5f;
		ComplexRecipe.RecipeElement[] array10 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("CrabWoodShell", 100f * num5)
		};
		ComplexRecipe.RecipeElement[] array11 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("WoodLog", 100f * num5, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		ComplexRecipe complexRecipe5 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("RockCrusher", array10, array11), array10, array11);
		complexRecipe5.time = 40f;
		complexRecipe5.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.LIME_RECIPE_DESCRIPTION, WoodLogConfig.TAG.ProperName(), global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.CRAB_SHELL.VARIANT_WOOD.NAME);
		complexRecipe5.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
		complexRecipe5.fabricators = new List<Tag> { TagManager.Create("RockCrusher") };
		complexRecipe5.sortOrder = 5;
		Element element5 = ElementLoader.FindElementByHash(SimHashes.Lime);
		ComplexRecipe.RecipeElement[] array12 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("SnailShell", 10f)
		};
		ComplexRecipe.RecipeElement[] array13 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(element5.tag, 10f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		ComplexRecipe complexRecipe6 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("RockCrusher", array12, array13), array12, array13);
		complexRecipe6.time = 40f;
		complexRecipe6.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.LIME_RECIPE_DESCRIPTION, SimHashes.Lime.CreateTag().ProperName(), global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.SNAIL_SHELL.NAME);
		complexRecipe6.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
		complexRecipe6.fabricators = new List<Tag> { TagManager.Create("RockCrusher") };
		complexRecipe6.sortOrder = 4;
		Element element6 = ElementLoader.FindElementByHash(SimHashes.GoldAmalgam);
		ComplexRecipe.RecipeElement[] array14 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("SnailIronShell", 10f)
		};
		ComplexRecipe.RecipeElement[] array15 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(element6.tag, 10f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		ComplexRecipe complexRecipe7 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("RockCrusher", array14, array15), array14, array15);
		complexRecipe7.time = 40f;
		complexRecipe7.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.RECIPE_DESCRIPTION, global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.SNAIL_IRON_SHELL.NAME, SimHashes.GoldAmalgam.CreateTag().ProperName());
		complexRecipe7.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
		complexRecipe7.fabricators = new List<Tag> { TagManager.Create("RockCrusher") };
		complexRecipe7.sortOrder = 5;
		ComplexRecipe.RecipeElement[] array16 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Fossil).tag, 100f)
		};
		ComplexRecipe.RecipeElement[] array17 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Lime).tag, 5f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false),
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.SedimentaryRock).tag, 95f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		ComplexRecipe complexRecipe8 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("RockCrusher", array16, array17), array16, array17);
		complexRecipe8.time = 40f;
		complexRecipe8.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.LIME_FROM_LIMESTONE_RECIPE_DESCRIPTION, SimHashes.Fossil.CreateTag().ProperName(), SimHashes.SedimentaryRock.CreateTag().ProperName(), SimHashes.Lime.CreateTag().ProperName());
		complexRecipe8.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
		complexRecipe8.fabricators = new List<Tag> { TagManager.Create("RockCrusher") };
		complexRecipe8.sortOrder = 4;
		ComplexRecipe.RecipeElement[] array18 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Corallium.CreateTag(), 100f)
		};
		ComplexRecipe.RecipeElement[] array19 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Lime.CreateTag(), 10f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false),
			new ComplexRecipe.RecipeElement(SimHashes.Sand.CreateTag(), 90f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		ComplexRecipe complexRecipe9 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("RockCrusher", array18, array19), array18, array19);
		complexRecipe9.time = 40f;
		complexRecipe9.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.LIME_FROM_LIMESTONE_RECIPE_DESCRIPTION, SimHashes.Corallium.CreateTag().ProperName(), SimHashes.Sand.CreateTag().ProperName(), SimHashes.Lime.CreateTag().ProperName());
		complexRecipe9.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
		complexRecipe9.fabricators = new List<Tag> { TagManager.Create("RockCrusher") };
		complexRecipe9.sortOrder = 4;
		ComplexRecipe.RecipeElement[] array20 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("GarbageElectrobank", 1f)
		};
		ComplexRecipe.RecipeElement[] array21 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Katairite).tag, 100f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		ComplexRecipe complexRecipe10 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("RockCrusher", array20, array21), array20, array21, DlcManager.DLC3);
		complexRecipe10.time = 40f;
		complexRecipe10.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.RECIPE_DESCRIPTION, global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.ELECTROBANK_GARBAGE.NAME, SimHashes.Katairite.CreateTag().ProperName());
		complexRecipe10.nameDisplay = ComplexRecipe.RecipeNameDisplay.Ingredient;
		complexRecipe10.fabricators = new List<Tag> { TagManager.Create("RockCrusher") };
		complexRecipe10.sortOrder = 6;
		float num6 = 5E-05f;
		ComplexRecipe.RecipeElement[] array22 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Salt.CreateTag(), 100f)
		};
		ComplexRecipe.RecipeElement[] array23 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(TableSaltConfig.ID.ToTag(), 100f * num6, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false),
			new ComplexRecipe.RecipeElement(SimHashes.Sand.CreateTag(), 100f * (1f - num6), ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		ComplexRecipe complexRecipe11 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("RockCrusher", array22, array23), array22, array23);
		complexRecipe11.time = 40f;
		complexRecipe11.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.RECIPE_DESCRIPTION, SimHashes.Salt.CreateTag().ProperName(), global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.TABLE_SALT.NAME);
		complexRecipe11.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
		complexRecipe11.fabricators = new List<Tag> { TagManager.Create("RockCrusher") };
		complexRecipe11.sortOrder = 7;
		if (ElementLoader.FindElementByHash(SimHashes.Graphite) != null)
		{
			float num7 = 0.9f;
			ComplexRecipe.RecipeElement[] array24 = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement(SimHashes.Fullerene.CreateTag(), 100f)
			};
			ComplexRecipe.RecipeElement[] array25 = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement(SimHashes.Graphite.CreateTag(), 100f * num7, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false),
				new ComplexRecipe.RecipeElement(SimHashes.Sand.CreateTag(), 100f * (1f - num7), ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
			};
			ComplexRecipe complexRecipe12 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("RockCrusher", array24, array25), array24, array25, DlcManager.EXPANSION1);
			complexRecipe12.time = 40f;
			complexRecipe12.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.RECIPE_DESCRIPTION, SimHashes.Fullerene.CreateTag().ProperName(), SimHashes.Graphite.CreateTag().ProperName());
			complexRecipe12.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
			complexRecipe12.fabricators = new List<Tag> { TagManager.Create("RockCrusher") };
			complexRecipe12.sortOrder = 8;
		}
		float num8 = 120f;
		float num9 = num8 * 0.2667f;
		ComplexRecipe.RecipeElement[] array26 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("IceBellyPoop", num8)
		};
		ComplexRecipe.RecipeElement[] array27 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Phosphorite.CreateTag(), num9, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false),
			new ComplexRecipe.RecipeElement(SimHashes.Clay.CreateTag(), num8 - num9, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		ComplexRecipe complexRecipe13 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("RockCrusher", array26, array27), array26, array27, DlcManager.DLC2);
		complexRecipe13.time = 40f;
		complexRecipe13.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.RECIPE_DESCRIPTION_TWO_OUTPUT, global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.ICE_BELLY_POOP.NAME, SimHashes.Phosphorite.CreateTag().ProperName(), SimHashes.Clay.CreateTag().ProperName());
		complexRecipe13.nameDisplay = ComplexRecipe.RecipeNameDisplay.Ingredient;
		complexRecipe13.fabricators = new List<Tag> { TagManager.Create("RockCrusher") };
		complexRecipe13.sortOrder = 10;
		float num10 = 1f;
		ComplexRecipe.RecipeElement[] array28 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("Urchin", num10)
		};
		ComplexRecipe.RecipeElement[] array29 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Diamond.CreateTag(), 100f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		ComplexRecipe complexRecipe14 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("RockCrusher", array28, array29), array28, array29, DlcManager.DLC5);
		complexRecipe14.time = 40f;
		complexRecipe14.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.RECIPE_DESCRIPTION, global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.URCHIN.NAME, SimHashes.Diamond.CreateTag().ProperName());
		complexRecipe14.nameDisplay = ComplexRecipe.RecipeNameDisplay.Ingredient;
		complexRecipe14.fabricators = new List<Tag> { TagManager.Create("RockCrusher") };
		complexRecipe14.sortOrder = 10;
		ComplexRecipe.RecipeElement[] array30 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("GoldBellyCrown", 1f)
		};
		ComplexRecipe.RecipeElement[] array31 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.GoldAmalgam).tag, 250f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		ComplexRecipe complexRecipe15 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("RockCrusher", array30, array31), array30, array31, DlcManager.DLC2);
		complexRecipe15.time = 40f;
		complexRecipe15.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.RECIPE_DESCRIPTION, global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.GOLD_BELLY_CROWN.NAME, SimHashes.GoldAmalgam.CreateTag().ProperName());
		complexRecipe15.nameDisplay = ComplexRecipe.RecipeNameDisplay.Ingredient;
		complexRecipe15.fabricators = new List<Tag> { TagManager.Create("RockCrusher") };
		complexRecipe15.sortOrder = 11;
		Prioritizable.AddRef(go);
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

	public const string ID = "RockCrusher";

	private const float INPUT_KG = 100f;

	private const float METAL_ORE_EFFICIENCY = 0.5f;
}
