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
		complexFabricatorWorkable.workingPstComplete = "working_pst_complete";
		Tag tag = SimHashes.Sand.CreateTag();
		List<Element> list = ElementLoader.elements.FindAll((Element e) => e.HasTag(GameTags.Crushable));
		ComplexRecipe complexRecipe;
		foreach (Element element in list)
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
			complexRecipe = new ComplexRecipe(text2, array, array2);
			complexRecipe.time = 40f;
			complexRecipe.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.RECIPE_DESCRIPTION, element.name, tag.ProperName());
			complexRecipe.useResultAsDescription = true;
			complexRecipe.displayInputAndOutput = true;
			complexRecipe.fabricators = new List<Tag> { TagManager.Create("RockCrusher") };
			ComplexRecipeManager.Get().AddObsoleteIDMapping(text, text2);
		}
		List<Element> list2 = ElementLoader.elements.FindAll((Element e) => e.IsSolid && e.HasTag(GameTags.Metal));
		foreach (Element element2 in list2)
		{
			Element highTempTransition = element2.highTempTransition;
			Element lowTempTransition = highTempTransition.lowTempTransition;
			if (lowTempTransition != element2)
			{
				ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[]
				{
					new ComplexRecipe.RecipeElement(element2.tag, 100f)
				};
				ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[]
				{
					new ComplexRecipe.RecipeElement(lowTempTransition.tag, 50f),
					new ComplexRecipe.RecipeElement(tag, 50f)
				};
				string text3 = ComplexRecipeManager.MakeObsoleteRecipeID("RockCrusher", lowTempTransition.tag);
				string text4 = ComplexRecipeManager.MakeRecipeID("RockCrusher", array3, array4);
				complexRecipe = new ComplexRecipe(text4, array3, array4);
				complexRecipe.time = 40f;
				complexRecipe.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.METAL_RECIPE_DESCRIPTION, lowTempTransition.name, element2.name);
				complexRecipe.useResultAsDescription = true;
				complexRecipe.displayInputAndOutput = true;
				complexRecipe.fabricators = new List<Tag> { TagManager.Create("RockCrusher") };
				ComplexRecipeManager.Get().AddObsoleteIDMapping(text3, text4);
			}
		}
		Element element3 = ElementLoader.FindElementByHash(SimHashes.Lime);
		ComplexRecipe.RecipeElement[] array5 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("EggShell", 5f)
		};
		ComplexRecipe.RecipeElement[] array6 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Lime).tag, 5f)
		};
		string text5 = ComplexRecipeManager.MakeObsoleteRecipeID("RockCrusher", element3.tag);
		string text6 = ComplexRecipeManager.MakeRecipeID("RockCrusher", array5, array6);
		complexRecipe = new ComplexRecipe(text6, array5, array6);
		complexRecipe.time = 40f;
		complexRecipe.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.LIME_RECIPE_DESCRIPTION, SimHashes.Lime.CreateTag().ProperName(), MISC.TAGS.EGGSHELL);
		complexRecipe.useResultAsDescription = true;
		complexRecipe.displayInputAndOutput = true;
		complexRecipe.fabricators = new List<Tag> { TagManager.Create("RockCrusher") };
		ComplexRecipeManager.Get().AddObsoleteIDMapping(text5, text6);
		ComplexRecipe.RecipeElement[] array7 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Fossil).tag, 100f)
		};
		ComplexRecipe.RecipeElement[] array8 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Lime).tag, 5f),
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.SedimentaryRock).tag, 95f)
		};
		string text7 = ComplexRecipeManager.MakeRecipeID("RockCrusher", array7, array8);
		complexRecipe = new ComplexRecipe(text7, array7, array8);
		complexRecipe.time = 40f;
		complexRecipe.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.LIME_FROM_LIMESTONE_RECIPE_DESCRIPTION, SimHashes.Fossil.CreateTag().ProperName(), SimHashes.SedimentaryRock.CreateTag().ProperName(), SimHashes.Lime.CreateTag().ProperName());
		complexRecipe.useResultAsDescription = true;
		complexRecipe.displayInputAndOutput = true;
		complexRecipe.fabricators = new List<Tag> { TagManager.Create("RockCrusher") };
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		SymbolOverrideControllerUtil.AddToPrefab(go);
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			ComplexFabricatorWorkable component = game_object.GetComponent<ComplexFabricatorWorkable>();
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
