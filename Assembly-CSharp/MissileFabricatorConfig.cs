using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class MissileFabricatorConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "MissileFabricator";
		int num = 5;
		int num2 = 4;
		string text2 = "missile_fabricator_kanim";
		int num3 = 250;
		float num4 = 60f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER6;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, refined_METALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.PENALTY.TIER2, tier2, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 960f;
		buildingDef.SelfHeatKilowattsWhenActive = 8f;
		buildingDef.ExhaustKilowattsWhenActive = 0f;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PowerInputOffset = new CellOffset(1, 0);
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.UtilityInputOffset = new CellOffset(-1, 1);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		ComplexFabricator complexFabricator = go.AddOrGet<ComplexFabricator>();
		complexFabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		go.AddOrGet<FabricatorIngredientStatusManager>();
		go.AddOrGet<CopyBuildingSettings>();
		complexFabricator.keepExcessLiquids = true;
		Workable workable = go.AddOrGet<ComplexFabricatorWorkable>();
		complexFabricator.duplicantOperated = true;
		BuildingTemplates.CreateComplexFabricatorStorage(go, complexFabricator);
		complexFabricator.storeProduced = false;
		complexFabricator.inStorage.SetDefaultStoredItemModifiers(MissileFabricatorConfig.RefineryStoredItemModifiers);
		complexFabricator.buildStorage.SetDefaultStoredItemModifiers(MissileFabricatorConfig.RefineryStoredItemModifiers);
		complexFabricator.outputOffset = new Vector3(1f, 0.5f);
		workable.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_missile_fabricator_kanim") };
		BuildingElementEmitter buildingElementEmitter = go.AddOrGet<BuildingElementEmitter>();
		buildingElementEmitter.emitRate = 0.0125f;
		buildingElementEmitter.temperature = 313.15f;
		buildingElementEmitter.element = SimHashes.CarbonDioxide;
		buildingElementEmitter.modifierOffset = new Vector2(2f, 2f);
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.capacityTag = GameTags.Liquid;
		conduitConsumer.capacityKG = 100f;
		conduitConsumer.storage = complexFabricator.inStorage;
		conduitConsumer.alwaysConsume = false;
		conduitConsumer.forceAlwaysSatisfied = true;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Store;
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Iron).tag, 25f, true),
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Petroleum).tag, 50f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("MissileBasic", 5f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		string text = ComplexRecipeManager.MakeObsoleteRecipeID("MissileFabricator", array[0].material);
		string text2 = ComplexRecipeManager.MakeRecipeID("MissileFabricator", array, array2);
		MissileBasicConfig.recipe = new ComplexRecipe(text2, array, array2)
		{
			time = 80f,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
			description = string.Format(global::STRINGS.BUILDINGS.PREFABS.MISSILEFABRICATOR.RECIPE_DESCRIPTION, ITEMS.MISSILE_BASIC.NAME, ElementLoader.GetElement(array[0].material).name, ElementLoader.GetElement(array[1].material).name),
			fabricators = new List<Tag> { TagManager.Create("MissileFabricator") }
		};
		ComplexRecipeManager.Get().AddObsoleteIDMapping(text, text2);
		ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Copper).tag, 25f, true),
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Petroleum).tag, 50f)
		};
		ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("MissileBasic", 5f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		string text3 = ComplexRecipeManager.MakeObsoleteRecipeID("MissileFabricator", array3[0].material);
		string text4 = ComplexRecipeManager.MakeRecipeID("MissileFabricator", array3, array4);
		MissileBasicConfig.recipe = new ComplexRecipe(text4, array3, array4)
		{
			time = 80f,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
			description = string.Format(global::STRINGS.BUILDINGS.PREFABS.MISSILEFABRICATOR.RECIPE_DESCRIPTION, ITEMS.MISSILE_BASIC.NAME, ElementLoader.GetElement(array3[0].material).name, ElementLoader.GetElement(array3[1].material).name),
			fabricators = new List<Tag> { TagManager.Create("MissileFabricator") }
		};
		ComplexRecipeManager.Get().AddObsoleteIDMapping(text3, text4);
		ComplexRecipe.RecipeElement[] array5 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Aluminum).tag, 25f, true),
			new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Petroleum).tag, 50f)
		};
		ComplexRecipe.RecipeElement[] array6 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("MissileBasic", 5f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		string text5 = ComplexRecipeManager.MakeObsoleteRecipeID("MissileFabricator", array5[0].material);
		string text6 = ComplexRecipeManager.MakeRecipeID("MissileFabricator", array5, array6);
		MissileBasicConfig.recipe = new ComplexRecipe(text6, array5, array6)
		{
			time = 80f,
			nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
			description = string.Format(global::STRINGS.BUILDINGS.PREFABS.MISSILEFABRICATOR.RECIPE_DESCRIPTION, ITEMS.MISSILE_BASIC.NAME, ElementLoader.GetElement(array5[0].material).name, ElementLoader.GetElement(array5[1].material).name),
			fabricators = new List<Tag> { TagManager.Create("MissileFabricator") }
		};
		ComplexRecipeManager.Get().AddObsoleteIDMapping(text5, text6);
		if (ElementLoader.FindElementByHash(SimHashes.Cobalt) != null)
		{
			ComplexRecipe.RecipeElement[] array7 = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Cobalt).tag, 25f, true),
				new ComplexRecipe.RecipeElement(ElementLoader.FindElementByHash(SimHashes.Petroleum).tag, 50f)
			};
			ComplexRecipe.RecipeElement[] array8 = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement("MissileBasic", 5f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
			};
			string text7 = ComplexRecipeManager.MakeObsoleteRecipeID("MissileFabricator", array7[0].material);
			string text8 = ComplexRecipeManager.MakeRecipeID("MissileFabricator", array7, array8);
			MissileBasicConfig.recipe = new ComplexRecipe(text8, array7, array8)
			{
				time = 80f,
				nameDisplay = ComplexRecipe.RecipeNameDisplay.ResultWithIngredient,
				description = string.Format(global::STRINGS.BUILDINGS.PREFABS.MISSILEFABRICATOR.RECIPE_DESCRIPTION, ITEMS.MISSILE_BASIC.NAME, ElementLoader.GetElement(array7[0].material).name, ElementLoader.GetElement(array7[1].material).name),
				fabricators = new List<Tag> { TagManager.Create("MissileFabricator") }
			};
			ComplexRecipeManager.Get().AddObsoleteIDMapping(text7, text8);
		}
		Prioritizable.AddRef(go);
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
			component.requiredSkillPerk = Db.Get().SkillPerks.CanMakeMissiles.Id;
		};
	}

	public const string ID = "MissileFabricator";

	public const float MISSILE_FABRICATION_TIME = 80f;

	public const float CO2_PRODUCTION_RATE = 0.0125f;

	private static readonly List<Storage.StoredItemModifier> RefineryStoredItemModifiers = new List<Storage.StoredItemModifier>
	{
		Storage.StoredItemModifier.Hide,
		Storage.StoredItemModifier.Preserve,
		Storage.StoredItemModifier.Seal
	};
}
