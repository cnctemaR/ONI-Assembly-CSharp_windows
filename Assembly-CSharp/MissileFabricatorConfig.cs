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
		buildingDef.RequiredSkillPerkID = Db.Get().SkillPerks.CanMakeMissiles.Id;
		buildingDef.AddSearchTerms(SEARCH_TERMS.MISSILE);
		buildingDef.POIUnlockable = true;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		ComplexFabricator complexFabricator = go.AddOrGet<ComplexFabricator>();
		complexFabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		go.AddOrGet<FabricatorIngredientStatusManager>();
		go.AddOrGet<CopyBuildingSettings>();
		complexFabricator.keepExcessLiquids = true;
		complexFabricator.allowManualFluidDelivery = false;
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
		conduitConsumer.capacityKG = 400f;
		conduitConsumer.storage = complexFabricator.inStorage;
		conduitConsumer.alwaysConsume = false;
		conduitConsumer.forceAlwaysSatisfied = true;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Store;
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(GameTags.BasicRefinedMetals, 25f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, "", false, true),
			new ComplexRecipe.RecipeElement(new Tag[]
			{
				SimHashes.Petroleum.CreateTag(),
				SimHashes.RefinedLipid.CreateTag()
			}, 50f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("MissileBasic", 5f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		ComplexRecipe complexRecipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("MissileFabricator", array, array2), array, array2);
		complexRecipe.time = 80f;
		complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
		complexRecipe.description = GameUtil.SafeStringFormat(global::STRINGS.BUILDINGS.PREFABS.MISSILEFABRICATOR.RECIPE_DESCRIPTION, new object[]
		{
			global::STRINGS.ITEMS.MISSILE_BASIC.NAME,
			MISC.TAGS.REFINEDMETAL,
			ElementLoader.FindElementByHash(SimHashes.Petroleum).name
		});
		complexRecipe.fabricators = new List<Tag> { TagManager.Create("MissileFabricator") };
		ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(GameTags.BasicRefinedMetals, 50f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, "", false, true),
			new ComplexRecipe.RecipeElement(new Tag[]
			{
				SimHashes.Fertilizer.CreateTag(),
				SimHashes.Peat.CreateTag()
			}, 100f),
			new ComplexRecipe.RecipeElement(new Tag[]
			{
				SimHashes.Petroleum.CreateTag(),
				SimHashes.RefinedLipid.CreateTag()
			}, 200f)
		};
		ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("MissileLongRange", 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		complexRecipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("MissileFabricator", array3, array4), array3, array4, DlcManager.DLC4, DlcManager.EXPANSION1);
		complexRecipe.time = 80f;
		complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
		complexRecipe.description = GameUtil.SafeStringFormat(global::STRINGS.BUILDINGS.PREFABS.MISSILEFABRICATOR.RECIPE_DESCRIPTION_LONGRANGE, new object[]
		{
			global::STRINGS.ITEMS.MISSILE_LONGRANGE.NAME,
			MISC.TAGS.REFINEDMETAL,
			ElementLoader.FindElementByHash(SimHashes.Fertilizer).name,
			ElementLoader.FindElementByHash(SimHashes.Petroleum).name
		});
		complexRecipe.fabricators = new List<Tag> { TagManager.Create("MissileFabricator") };
		ComplexRecipe.RecipeElement[] array5 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(GameTags.BasicRefinedMetals, 50f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, "", false, true),
			new ComplexRecipe.RecipeElement(new Tag[]
			{
				SimHashes.Fertilizer.CreateTag(),
				SimHashes.Peat.CreateTag()
			}, 100f),
			new ComplexRecipe.RecipeElement(new Tag[]
			{
				SimHashes.Petroleum.CreateTag(),
				SimHashes.RefinedLipid.CreateTag()
			}, 200f)
		};
		ComplexRecipe.RecipeElement[] array6 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("MissileLongRange", 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		complexRecipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("MissileFabricator", array5, array6), array5, array6, DlcManager.EXPANSION1, new string[0]);
		complexRecipe.time = 80f;
		complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
		complexRecipe.description = GameUtil.SafeStringFormat(global::STRINGS.BUILDINGS.PREFABS.MISSILEFABRICATOR.RECIPE_DESCRIPTION_LONGRANGE, new object[]
		{
			global::STRINGS.ITEMS.MISSILE_LONGRANGE.NAME,
			MISC.TAGS.REFINEDMETAL,
			ElementLoader.FindElementByHash(SimHashes.Fertilizer).name,
			ElementLoader.FindElementByHash(SimHashes.Petroleum).name
		});
		complexRecipe.fabricators = new List<Tag> { TagManager.Create("MissileFabricator") };
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

	public const float LONG_RANGE_MISSILE_REFINED_METAL = 50f;

	public const float LONG_RANGE_MISSILE_LIQUID_INPUT = 200f;

	public const float LONG_RANGE_MISSILE_SOLID_INPUT = 100f;

	private static readonly List<Storage.StoredItemModifier> RefineryStoredItemModifiers = new List<Storage.StoredItemModifier>
	{
		Storage.StoredItemModifier.Hide,
		Storage.StoredItemModifier.Preserve,
		Storage.StoredItemModifier.Seal
	};
}
