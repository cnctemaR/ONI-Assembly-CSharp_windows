using System;
using System.Collections.Generic;
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
		complexFabricatorWorkable.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_rockrefinery_kanim") };
		complexFabricatorWorkable.workingPstComplete = new HashedString[] { "working_pst_complete" };
		Tag tag = SimHashes.Sand.CreateTag();
		foreach (Element element in ElementLoader.elements.FindAll((Element e) => e.HasTag(GameTags.Crushable)))
		{
			ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement(element.tag, 100f)
			};
			ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement(tag, 100f)
			};
			string text = ComplexRecipeManager.MakeObsoleteRecipeID("RockCrusher", element.tag);
			string text2 = ComplexRecipeManager.MakeRecipeID("RockCrusher", array, array2);
			ComplexRecipe complexRecipe = new ComplexRecipe(text2, array, array2, 0);
			complexRecipe.time = 40f;
			complexRecipe.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.RECIPE_DESCRIPTION, element.name, tag.ProperName());
			complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
			complexRecipe.fabricators = new List<Tag> { TagManager.Create("RockCrusher") };
			ComplexRecipeManager.Get().AddObsoleteIDMapping(text, text2);
		}
		foreach (Element element2 in ElementLoader.elements.FindAll((Element e) => e.IsSolid && e.HasTag(GameTags.Metal)))
		{
			if (!element2.HasTag(GameTags.Noncrushable))
			{
				Element lowTempTransition = element2.highTempTransition.lowTempTransition;
				if (lowTempTransition != element2)
				{
					ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[]
					{
						new ComplexRecipe.RecipeElement(element2.tag, 100f)
					};
					ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[]
					{
						new ComplexRecipe.RecipeElement(lowTempTransition.tag, 50f),
						new ComplexRecipe.RecipeElement(tag, 50f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
					};
					string text3 = ComplexRecipeManager.MakeObsoleteRecipeID("RockCrusher", lowTempTransition.tag);
					string text4 = ComplexRecipeManager.MakeRecipeID("RockCrusher", array3, array4);
					ComplexRecipe complexRecipe2 = new ComplexRecipe(text4, array3, array4, 0);
					complexRecipe2.time = 40f;
					complexRecipe2.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.METAL_RECIPE_DESCRIPTION, lowTempTransition.name, element2.name);
					complexRecipe2.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
					complexRecipe2.fabricators = new List<Tag> { TagManager.Create("RockCrusher") };
					ComplexRecipeManager.Get().AddObsoleteIDMapping(text3, text4);
				}
			}
		}
		Element element3 = ElementLoader.FindElementByHash(SimHashes.Lime);
		ComplexRecipe.RecipeElement[] array5 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("EggShell", 5f)
		};
		ComplexRecipe.RecipeElement[] array6 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Lime).tag, 5f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		string text5 = ComplexRecipeManager.MakeObsoleteRecipeID("RockCrusher", element3.tag);
		string text6 = ComplexRecipeManager.MakeRecipeID("RockCrusher", array5, array6);
		ComplexRecipe complexRecipe3 = new ComplexRecipe(text6, array5, array6, 0);
		complexRecipe3.time = 40f;
		complexRecipe3.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.LIME_RECIPE_DESCRIPTION, SimHashes.Lime.CreateTag().ProperName(), MISC.TAGS.EGGSHELL);
		complexRecipe3.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
		complexRecipe3.fabricators = new List<Tag> { TagManager.Create("RockCrusher") };
		ComplexRecipeManager.Get().AddObsoleteIDMapping(text5, text6);
		Element element4 = ElementLoader.FindElementByHash(SimHashes.Lime);
		ComplexRecipe.RecipeElement[] array7 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("BabyCrabShell", 1f)
		};
		ComplexRecipe.RecipeElement[] array8 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(element4.tag, 5f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		ComplexRecipe complexRecipe4 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("RockCrusher", array7, array8), array7, array8, 0);
		complexRecipe4.time = 40f;
		complexRecipe4.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.LIME_RECIPE_DESCRIPTION, SimHashes.Lime.CreateTag().ProperName(), ITEMS.INDUSTRIAL_PRODUCTS.CRAB_SHELL.NAME);
		complexRecipe4.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
		complexRecipe4.fabricators = new List<Tag> { TagManager.Create("RockCrusher") };
		Element element5 = ElementLoader.FindElementByHash(SimHashes.Lime);
		ComplexRecipe.RecipeElement[] array9 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("CrabShell", 1f)
		};
		ComplexRecipe.RecipeElement[] array10 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(element5.tag, 10f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		ComplexRecipe complexRecipe5 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("RockCrusher", array9, array10), array9, array10, 0);
		complexRecipe5.time = 40f;
		complexRecipe5.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.LIME_RECIPE_DESCRIPTION, SimHashes.Lime.CreateTag().ProperName(), ITEMS.INDUSTRIAL_PRODUCTS.CRAB_SHELL.NAME);
		complexRecipe5.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
		complexRecipe5.fabricators = new List<Tag> { TagManager.Create("RockCrusher") };
		ComplexRecipe.RecipeElement[] array11 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Fossil).tag, 100f)
		};
		ComplexRecipe.RecipeElement[] array12 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Lime).tag, 5f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false),
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.SedimentaryRock).tag, 95f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		ComplexRecipe complexRecipe6 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("RockCrusher", array11, array12), array11, array12, 0);
		complexRecipe6.time = 40f;
		complexRecipe6.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.LIME_FROM_LIMESTONE_RECIPE_DESCRIPTION, SimHashes.Fossil.CreateTag().ProperName(), SimHashes.SedimentaryRock.CreateTag().ProperName(), SimHashes.Lime.CreateTag().ProperName());
		complexRecipe6.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
		complexRecipe6.fabricators = new List<Tag> { TagManager.Create("RockCrusher") };
		float num = 5E-05f;
		ComplexRecipe.RecipeElement[] array13 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Salt.CreateTag(), 100f)
		};
		ComplexRecipe.RecipeElement[] array14 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(TableSaltConfig.ID.ToTag(), 100f * num, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false),
			new ComplexRecipe.RecipeElement(SimHashes.Sand.CreateTag(), 100f * (1f - num), ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		ComplexRecipe complexRecipe7 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("RockCrusher", array13, array14), array13, array14, 0);
		complexRecipe7.time = 40f;
		complexRecipe7.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.RECIPE_DESCRIPTION, SimHashes.Salt.CreateTag().ProperName(), ITEMS.INDUSTRIAL_PRODUCTS.TABLE_SALT.NAME);
		complexRecipe7.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
		complexRecipe7.fabricators = new List<Tag> { TagManager.Create("RockCrusher") };
		if (ElementLoader.FindElementByHash(SimHashes.Graphite) != null)
		{
			float num2 = 0.9f;
			ComplexRecipe.RecipeElement[] array15 = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement(SimHashes.Fullerene.CreateTag(), 100f)
			};
			ComplexRecipe.RecipeElement[] array16 = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement(SimHashes.Graphite.CreateTag(), 100f * num2, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false),
				new ComplexRecipe.RecipeElement(SimHashes.Sand.CreateTag(), 100f * (1f - num2), ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
			};
			ComplexRecipe complexRecipe8 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("RockCrusher", array15, array16), array15, array16, 0);
			complexRecipe8.time = 40f;
			complexRecipe8.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.RECIPE_DESCRIPTION, SimHashes.Fullerene.CreateTag().ProperName(), SimHashes.Graphite.CreateTag().ProperName());
			complexRecipe8.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
			complexRecipe8.fabricators = new List<Tag> { TagManager.Create("RockCrusher") };
		}
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
