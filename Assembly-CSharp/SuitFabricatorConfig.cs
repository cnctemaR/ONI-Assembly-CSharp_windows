using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class SuitFabricatorConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "SuitFabricator";
		int num = 4;
		int num2 = 3;
		string text2 = "suit_maker_kanim";
		int num3 = 100;
		float num4 = 240f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		float num5 = 800f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER3;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, refined_METALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.NONE, tier2, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 480f;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PowerInputOffset = new CellOffset(1, 0);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<Prioritizable>();
		ComplexFabricator complexFabricator = go.AddOrGet<ComplexFabricator>();
		complexFabricator.resultState = ComplexFabricator.ResultState.Heated;
		complexFabricator.heatedTemperature = 318.15f;
		complexFabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		go.AddOrGet<FabricatorIngredientStatusManager>();
		go.AddOrGet<CopyBuildingSettings>();
		go.AddOrGet<ComplexFabricatorWorkable>().overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_suit_fabricator_kanim") };
		Prioritizable.AddRef(go);
		BuildingTemplates.CreateComplexFabricatorStorage(go, complexFabricator);
		this.ConfigureRecipes();
	}

	private void ConfigureRecipes()
	{
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Copper.CreateTag(), 300f),
			new ComplexRecipe.RecipeElement("BasicFabric".ToTag(), 2f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("Atmo_Suit".ToTag(), 1f)
		};
		AtmoSuitConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("SuitFabricator", array, array2), array, array2)
		{
			time = (float)global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_FABTIME,
			description = global::STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.RECIPE_DESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
			fabricators = new List<Tag> { "SuitFabricator" },
			requiredTech = Db.Get().TechItems.suitsOverlay.parentTech.Id
		};
		ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Aluminum.CreateTag(), 300f),
			new ComplexRecipe.RecipeElement("BasicFabric".ToTag(), 2f)
		};
		ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("Atmo_Suit".ToTag(), 1f)
		};
		AtmoSuitConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("SuitFabricator", array3, array4), array3, array4)
		{
			time = (float)global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_FABTIME,
			description = global::STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.RECIPE_DESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
			fabricators = new List<Tag> { "SuitFabricator" },
			requiredTech = Db.Get().TechItems.suitsOverlay.parentTech.Id
		};
		ComplexRecipe.RecipeElement[] array5 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Iron.CreateTag(), 300f),
			new ComplexRecipe.RecipeElement("BasicFabric".ToTag(), 2f)
		};
		ComplexRecipe.RecipeElement[] array6 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("Atmo_Suit".ToTag(), 1f)
		};
		AtmoSuitConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("SuitFabricator", array5, array6), array5, array6)
		{
			time = (float)global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_FABTIME,
			description = global::STRINGS.EQUIPMENT.PREFABS.ATMO_SUIT.RECIPE_DESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
			fabricators = new List<Tag> { "SuitFabricator" },
			requiredTech = Db.Get().TechItems.suitsOverlay.parentTech.Id
		};
		ComplexRecipe.RecipeElement[] array7 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Steel.ToString(), 200f),
			new ComplexRecipe.RecipeElement(SimHashes.Petroleum.ToString(), 25f)
		};
		ComplexRecipe.RecipeElement[] array8 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("Jet_Suit".ToTag(), 1f)
		};
		JetSuitConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("SuitFabricator", array7, array8), array7, array8)
		{
			time = (float)global::TUNING.EQUIPMENT.SUITS.ATMOSUIT_FABTIME,
			description = global::STRINGS.EQUIPMENT.PREFABS.JET_SUIT.RECIPE_DESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
			fabricators = new List<Tag> { "SuitFabricator" },
			requiredTech = Db.Get().TechItems.jetSuit.parentTech.Id
		};
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

	public const string ID = "SuitFabricator";
}
