using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class ApothecaryConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "Apothecary";
		int num = 2;
		int num2 = 3;
		string text2 = "apothecary_kanim";
		int num3 = 30;
		float num4 = 120f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num5 = 800f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, all_METALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.NONE, none, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 240f;
		buildingDef.ExhaustKilowattsWhenActive = 0.25f;
		buildingDef.SelfHeatKilowattsWhenActive = 0.5f;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "Glass";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<DropAllWorkable>();
		Prioritizable.AddRef(go);
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		ComplexFabricator complexFabricator = go.AddOrGet<ComplexFabricator>();
		go.AddOrGet<FabricatorIngredientStatusManager>();
		go.AddOrGet<CopyBuildingSettings>();
		go.AddOrGet<ComplexFabricatorWorkable>().overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_apothecary_kanim") };
		go.AddOrGet<ComplexFabricatorWorkable>().AnimOffset = new Vector3(-1f, 0f, 0f);
		complexFabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		BuildingTemplates.CreateComplexFabricatorStorage(go, complexFabricator);
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("Water".ToTag(), 100f),
			new ComplexRecipe.RecipeElement("Sand".ToTag(), 100f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("GenericPill".ToTag(), 1f)
		};
		string text = ComplexRecipeManager.MakeRecipeID("Apothecary", array, array2);
		GenericPillConfig.recipe = new ComplexRecipe(text, array, array2)
		{
			time = 40f,
			description = ITEMS.PILLS.PLACEBO.RECIPEDESC,
			useResultAsDescription = true,
			fabricators = new List<Tag> { "Apothecary" }
		};
		ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("Carbon".ToTag(), 100f),
			new ComplexRecipe.RecipeElement(SwampLilyFlowerConfig.ID, 1f)
		};
		ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement("VitaminSupplement".ToTag(), 1f)
		};
		string text2 = ComplexRecipeManager.MakeRecipeID("Apothecary", array3, array4);
		VitaminSupplementConfig.recipe = new ComplexRecipe(text2, array3, array4)
		{
			time = 40f,
			description = ITEMS.PILLS.VITAMINSUPPLEMENT.RECIPEDESC,
			useResultAsDescription = true,
			fabricators = new List<Tag> { "Apothecary" }
		};
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGetDef<PoweredActiveStoppableController.Def>();
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			ComplexFabricatorWorkable component = game_object.GetComponent<ComplexFabricatorWorkable>();
			component.SetAttributeConverter(Db.Get().AttributeConverters.CompoundingSpeed);
		};
	}

	public const string ID = "Apothecary";
}
