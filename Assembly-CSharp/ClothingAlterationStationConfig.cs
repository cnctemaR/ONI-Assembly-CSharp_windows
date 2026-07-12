using System;
using System.Collections.Generic;
using Database;
using STRINGS;
using TUNING;
using UnityEngine;

public class ClothingAlterationStationConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "ClothingAlterationStation";
		int num = 4;
		int num2 = 3;
		string text2 = "super_snazzy_suit_alteration_station_kanim";
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
		buildingDef.PowerInputOffset = new CellOffset(0, 0);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		go.AddOrGet<DropAllWorkable>();
		Prioritizable.AddRef(go);
		ComplexFabricator complexFabricator = go.AddOrGet<ComplexFabricator>();
		complexFabricator.outputOffset = new Vector3(1f, 0f, 0f);
		ComplexFabricatorWorkable complexFabricatorWorkable = go.AddOrGet<ComplexFabricatorWorkable>();
		complexFabricatorWorkable.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_super_snazzy_suit_alteration_station_kanim") };
		complexFabricatorWorkable.workingPstComplete = new HashedString[] { "working_pst_complete" };
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
			new ComplexRecipe.RecipeElement("Funky_Vest".ToTag(), 1f, false),
			new ComplexRecipe.RecipeElement("BasicFabric".ToTag(), 3f)
		};
		foreach (EquippableFacadeResource equippableFacadeResource in Db.GetEquippableFacades().resources.FindAll((EquippableFacadeResource match) => match.DefID == "CustomClothing"))
		{
			ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[]
			{
				new ComplexRecipe.RecipeElement("CustomClothing".ToTag(), 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, equippableFacadeResource.Id, false)
			};
			ComplexRecipe complexRecipe = new ComplexRecipe(ComplexRecipeManager.MakeRecipeID("ClothingAlterationStation", array, array2, equippableFacadeResource.Id), array, array2);
			complexRecipe.time = global::TUNING.EQUIPMENT.VESTS.CUSTOM_CLOTHING_FABTIME;
			complexRecipe.description = global::STRINGS.EQUIPMENT.PREFABS.CUSTOMCLOTHING.RECIPE_DESC;
			complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.Result;
			complexRecipe.fabricators = new List<Tag> { "ClothingAlterationStation" };
			complexRecipe.sortOrder = 1;
		}
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<KPrefabID>().prefabSpawnFn += delegate(GameObject game_object)
		{
			ComplexFabricatorWorkable component = game_object.GetComponent<ComplexFabricatorWorkable>();
			component.WorkerStatusItem = Db.Get().DuplicantStatusItems.Fabricating;
			component.AttributeConverter = Db.Get().AttributeConverters.ArtSpeed;
			component.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
			component.SkillExperienceSkillGroup = Db.Get().SkillGroups.Art.Id;
			component.SkillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
			component.requiredSkillPerk = Db.Get().SkillPerks.CanClothingAlteration.Id;
			game_object.GetComponent<ComplexFabricator>().choreType = Db.Get().ChoreTypes.Art;
		};
	}

	public const string ID = "ClothingAlterationStation";
}
