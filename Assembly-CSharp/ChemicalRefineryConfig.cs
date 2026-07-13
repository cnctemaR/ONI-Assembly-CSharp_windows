using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class ChemicalRefineryConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "ChemicalRefinery";
		int num = 4;
		int num2 = 3;
		string text2 = "chemistry_lab_kanim";
		int num3 = 30;
		float num4 = 60f;
		float[] array = new float[]
		{
			global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3[0],
			global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2[0]
		};
		string[] array2 = new string[] { "RefinedMetal", "Glass" };
		float num5 = 2400f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier = NOISE_POLLUTION.NOISY.TIER1;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, array, array2, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.PENALTY.TIER1, tier, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 480f;
		buildingDef.SelfHeatKilowattsWhenActive = 1f;
		buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.AudioSize = "large";
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.OutputConduitType = ConduitType.Liquid;
		buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
		buildingDef.PowerInputOffset = new CellOffset(1, 0);
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(2, 1);
		buildingDef.RequiredSkillPerkID = Db.Get().SkillPerks.AllowChemistry.Id;
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
		complexFabricatorWorkable.requiredSkillPerk = Db.Get().SkillPerks.AllowChemistry.Id;
		BuildingTemplates.CreateComplexFabricatorStorage(go, complexFabricator);
		complexFabricatorWorkable.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_chemistrylab_kanim") };
		complexFabricatorWorkable.workingPstComplete = new HashedString[] { "working_pst_complete" };
		complexFabricator.heatedTemperature = SupermaterialRefineryConfig.OUTPUT_TEMPERATURE;
		complexFabricator.storeProduced = true;
		complexFabricator.inStorage.SetDefaultStoredItemModifiers(ChemicalRefineryConfig.RefineryStoredItemModifiers);
		complexFabricator.buildStorage.SetDefaultStoredItemModifiers(ChemicalRefineryConfig.RefineryStoredItemModifiers);
		complexFabricator.outStorage.SetDefaultStoredItemModifiers(ChemicalRefineryConfig.RefineryStoredItemModifiers);
		complexFabricator.storeProduced = false;
		complexFabricator.keepExcessLiquids = true;
		complexFabricator.inStorage.capacityKg = 1000f;
		ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
		conduitDispenser.conduitType = ConduitType.Liquid;
		conduitDispenser.alwaysDispense = true;
		conduitDispenser.elementFilter = null;
		conduitDispenser.storage = go.GetComponent<ComplexFabricator>().outStorage;
		go.AddOrGet<FabricatorIngredientStatusManager>();
		go.AddOrGet<CopyBuildingSettings>();
		Prioritizable.AddRef(go);
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Water.CreateTag(), 93f),
			new ComplexRecipe.RecipeElement(SimHashes.Salt.CreateTag(), 7f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.SaltWater.CreateTag(), 100f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, true)
		};
		ComplexRecipe complexRecipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("ChemicalRefinery", array, array2), array, array2);
		complexRecipe.time = 40f;
		complexRecipe.description = global::STRINGS.BUILDINGS.PREFABS.CHEMICALREFINERY.SALTWATER_RECIPE_DESCRIPTION;
		complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
		complexRecipe.fabricators = new List<Tag> { TagManager.Create("ChemicalRefinery") };
		ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.PhytoOil.CreateTag(), 160f),
			new ComplexRecipe.RecipeElement(SimHashes.BleachStone.CreateTag(), 40f)
		};
		ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.RefinedLipid.CreateTag(), 200f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, true)
		};
		ComplexRecipe complexRecipe2 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("ChemicalRefinery", array3, array4), array3, array4);
		complexRecipe2.time = 40f;
		complexRecipe2.description = global::STRINGS.BUILDINGS.PREFABS.CHEMICALREFINERY.REFINEDLIPID_RECIPE_DESCRIPTION;
		complexRecipe2.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
		complexRecipe2.fabricators = new List<Tag> { TagManager.Create("ChemicalRefinery") };
		float num = 0.01f;
		float num2 = (1f - num) * 0.5f;
		ComplexRecipe.RecipeElement[] array5 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Fullerene.CreateTag(), 100f * num),
			new ComplexRecipe.RecipeElement(SimHashes.Gold.CreateTag(), 100f * num2),
			new ComplexRecipe.RecipeElement(SimHashes.Petroleum.CreateTag(), 100f * num2)
		};
		ComplexRecipe.RecipeElement[] array6 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.SuperCoolant.CreateTag(), 100f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, true)
		};
		ComplexRecipe complexRecipe3 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("ChemicalRefinery", array5, array6), array5, array6);
		complexRecipe3.time = 80f;
		complexRecipe3.description = global::STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.SUPERCOOLANT_RECIPE_DESCRIPTION;
		complexRecipe3.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
		complexRecipe3.fabricators = new List<Tag> { TagManager.Create("ChemicalRefinery") };
		complexRecipe3.requiredTech = Db.Get().TechItems.superLiquids.parentTechId;
		float num3 = 0.35f;
		ComplexRecipe.RecipeElement[] array7 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Isoresin.CreateTag(), 100f * num3),
			new ComplexRecipe.RecipeElement(SimHashes.Petroleum.CreateTag(), 100f * (1f - num3))
		};
		ComplexRecipe.RecipeElement[] array8 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.ViscoGel.CreateTag(), 100f, ComplexRecipe.RecipeElement.TemperatureOperation.Heated, true)
		};
		ComplexRecipe complexRecipe4 = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("ChemicalRefinery", array7, array8), array7, array8);
		complexRecipe4.time = 80f;
		complexRecipe4.description = global::STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.VISCOGEL_RECIPE_DESCRIPTION;
		complexRecipe4.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
		complexRecipe4.fabricators = new List<Tag> { TagManager.Create("ChemicalRefinery") };
		complexRecipe4.requiredTech = Db.Get().TechItems.superLiquids.parentTechId;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<RequireInputs>().requireConduitHasMass = false;
		go.GetComponent<KPrefabID>().prefabSpawnFn += delegate(GameObject game_object)
		{
			ComplexFabricatorWorkable component = game_object.GetComponent<ComplexFabricatorWorkable>();
			component.WorkerStatusItem = Db.Get().DuplicantStatusItems.Processing;
			component.AttributeConverter = Db.Get().AttributeConverters.ResearchSpeed;
			component.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
			component.SkillExperienceSkillGroup = Db.Get().SkillGroups.Research.Id;
			component.SkillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
			KAnimFile anim = Assets.GetAnim("anim_interacts_chemistrylab_kanim");
			KAnimFile[] array = new KAnimFile[] { anim };
			component.overrideAnims = array;
			component.workAnims = new HashedString[] { "working_pre", "working_loop" };
			component.synchronizeAnims = false;
		};
	}

	public const string ID = "ChemicalRefinery";

	private HashedString[] dupeInteractAnims;

	private const float INPUT_KG = 100f;

	private const float OUTPUT_KG = 100f;

	private static readonly List<Storage.StoredItemModifier> RefineryStoredItemModifiers = new List<Storage.StoredItemModifier>
	{
		Storage.StoredItemModifier.Hide,
		Storage.StoredItemModifier.Preserve,
		Storage.StoredItemModifier.Insulate,
		Storage.StoredItemModifier.Seal
	};
}
