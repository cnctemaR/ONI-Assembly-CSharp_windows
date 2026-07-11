using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class SupermaterialRefineryConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "SupermaterialRefinery";
		int num = 4;
		int num2 = 5;
		string text2 = "supermaterial_refinery_kanim";
		int num3 = 30;
		float num4 = 480f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num5 = 2400f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER6;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, all_METALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.PENALTY.TIER2, tier2, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 1600f;
		buildingDef.SelfHeatKilowattsWhenActive = 16f;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		Refinery refinery = go.AddOrGet<Refinery>();
		refinery.sideScreenStyle = RefinerySideScreen.StyleSetting.ListInputOutput;
		refinery.duplicantOperated = true;
		RefineryWorkable refineryWorkable = go.AddOrGet<RefineryWorkable>();
		BuildingTemplates.CreateRefineryStorage(go, refinery);
		refineryWorkable.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_rockrefinery_kanim") };
		Prioritizable.AddRef(go);
		float num = 0.01f;
		float num2 = (1f - num) * 0.5f;
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Fullerene.CreateTag(), 100f * num),
			new ComplexRecipe.RecipeElement(SimHashes.Gold.CreateTag(), 100f * num2),
			new ComplexRecipe.RecipeElement(SimHashes.Petroleum.CreateTag(), 100f * num2)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.SuperCoolant.CreateTag(), 100f)
		};
		string text = ComplexRecipeManager.MakeRecipeID("SupermaterialRefinery", array, array2);
		ComplexRecipe complexRecipe = new ComplexRecipe(text, array, array2);
		complexRecipe.time = 80f;
		complexRecipe.description = global::STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.SUPERCOOLANT_RECIPE_DESCRIPTION;
		complexRecipe.useResultAsDescription = true;
		complexRecipe.fabricators = new List<Tag> { TagManager.Create("SupermaterialRefinery") };
		float num3 = 0.15f;
		float num4 = 0.05f;
		float num5 = 1f - num4 - num3;
		ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Isoresin.CreateTag(), 100f * num3),
			new ComplexRecipe.RecipeElement(SimHashes.Katairite.CreateTag(), 100f * num5),
			new ComplexRecipe.RecipeElement(BasicFabricConfig.ID.ToTag(), 100f * num4)
		};
		ComplexRecipe.RecipeElement[] array4 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.SuperInsulator.CreateTag(), 100f)
		};
		string text2 = ComplexRecipeManager.MakeRecipeID("SupermaterialRefinery", array3, array4);
		complexRecipe = new ComplexRecipe(text2, array3, array4);
		complexRecipe.time = 80f;
		complexRecipe.description = global::STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.SUPERINSULATOR_RECIPE_DESCRIPTION;
		complexRecipe.useResultAsDescription = true;
		complexRecipe.fabricators = new List<Tag> { TagManager.Create("SupermaterialRefinery") };
		float num6 = 0.05f;
		ComplexRecipe.RecipeElement[] array5 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Niobium.CreateTag(), 100f * num6),
			new ComplexRecipe.RecipeElement(SimHashes.Tungsten.CreateTag(), 100f * (1f - num6))
		};
		ComplexRecipe.RecipeElement[] array6 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.TempConductorSolid.CreateTag(), 100f)
		};
		string text3 = ComplexRecipeManager.MakeRecipeID("SupermaterialRefinery", array5, array6);
		complexRecipe = new ComplexRecipe(text3, array5, array6);
		complexRecipe.time = 80f;
		complexRecipe.description = global::STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.TEMPCONDUCTORSOLID_RECIPE_DESCRIPTION;
		complexRecipe.useResultAsDescription = true;
		complexRecipe.fabricators = new List<Tag> { TagManager.Create("SupermaterialRefinery") };
		float num7 = 0.35f;
		ComplexRecipe.RecipeElement[] array7 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.Isoresin.CreateTag(), 100f * num7),
			new ComplexRecipe.RecipeElement(SimHashes.Petroleum.CreateTag(), 100f * (1f - num7))
		};
		ComplexRecipe.RecipeElement[] array8 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(SimHashes.ViscoGel.CreateTag(), 100f)
		};
		string text4 = ComplexRecipeManager.MakeRecipeID("SupermaterialRefinery", array7, array8);
		complexRecipe = new ComplexRecipe(text4, array7, array8);
		complexRecipe.time = 80f;
		complexRecipe.description = global::STRINGS.BUILDINGS.PREFABS.SUPERMATERIALREFINERY.VISCOGEL_RECIPE_DESCRIPTION;
		complexRecipe.useResultAsDescription = true;
		complexRecipe.fabricators = new List<Tag> { TagManager.Create("SupermaterialRefinery") };
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}

	public const string ID = "SupermaterialRefinery";

	private const float INPUT_KG = 100f;

	private const float OUTPUT_KG = 100f;
}
