using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class ClothingFabricatorConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "ClothingFabricator";
		int num = 4;
		int num2 = 3;
		string text2 = "clothingfactory_kanim";
		int num3 = 100;
		float num4 = 240f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER5;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, refined_METALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.NONE, tier2, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 240f;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PowerInputOffset = new CellOffset(2, 0);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		go.AddOrGet<DropAllWorkable>();
		Prioritizable.AddRef(go);
		ComplexFabricator complexFabricator = go.AddOrGet<ComplexFabricator>();
		go.AddOrGet<ComplexFabricatorWorkable>().overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_clothingfactory_kanim") };
		go.AddOrGet<ComplexFabricatorWorkable>().AnimOffset = new Vector3(-1f, 0f, 0f);
		complexFabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		go.AddOrGet<FabricatorIngredientStatusManager>();
		go.AddOrGet<CopyBuildingSettings>();
		this.ConfigureRecipes();
		BuildingTemplates.CreateComplexFabricatorStorage(go, complexFabricator);
	}

	private void ConfigureRecipes()
	{
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("BasicFabric".ToTag(), (float)global::TUNING.EQUIPMENT.VESTS.WARM_VEST_MASS)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("Warm_Vest".ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		WarmVestConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("ClothingFabricator", array, array2), array, array2)
		{
			time = global::TUNING.EQUIPMENT.VESTS.WARM_VEST_FABTIME,
			description = global::STRINGS.EQUIPMENT.PREFABS.WARM_VEST.RECIPE_DESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag> { "ClothingFabricator" },
			sortOrder = 1
		};
		ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("BasicFabric".ToTag(), (float)global::TUNING.EQUIPMENT.VESTS.COOL_VEST_MASS)
		};
		ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("Cool_Vest".ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		CoolVestConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("ClothingFabricator", array3, array4), array3, array4)
		{
			time = global::TUNING.EQUIPMENT.VESTS.COOL_VEST_FABTIME,
			description = global::STRINGS.EQUIPMENT.PREFABS.COOL_VEST.RECIPE_DESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag> { "ClothingFabricator" },
			sortOrder = 1
		};
		ComplexRecipe.RecipeElement[] array5 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("BasicFabric".ToTag(), (float)global::TUNING.EQUIPMENT.VESTS.FUNKY_VEST_MASS)
		};
		ComplexRecipe.RecipeElement[] array6 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("Funky_Vest".ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		FunkyVestConfig.recipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("ClothingFabricator", array5, array6), array5, array6)
		{
			time = global::TUNING.EQUIPMENT.VESTS.FUNKY_VEST_FABTIME,
			description = global::STRINGS.EQUIPMENT.PREFABS.FUNKY_VEST.RECIPE_DESC,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.Result,
			fabricators = new List<Tag> { "ClothingFabricator" },
			sortOrder = 1
		};
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
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

	public const string ID = "ClothingFabricator";
}
