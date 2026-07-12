using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class DataMinerConfig : IBuildingConfig
{
	public override string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC3;
	}

	public override BuildingDef CreateBuildingDef()
	{
		string text = "DataMiner";
		int num = 3;
		int num2 = 2;
		string text2 = "data_miner_kanim";
		int num3 = 30;
		float num4 = 30f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER1;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, all_METALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.NONE, tier2, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
		buildingDef.EnergyConsumptionWhenActive = 1000f;
		buildingDef.ExhaustKilowattsWhenActive = 0.5f;
		buildingDef.SelfHeatKilowattsWhenActive = 3f;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<BuildingComplete>().isManuallyOperated = false;
		go.AddOrGet<LogicOperationalController>();
		go.AddOrGet<CopyBuildingSettings>();
		DataMiner dataMiner = go.AddOrGet<DataMiner>();
		dataMiner.duplicantOperated = false;
		dataMiner.showProgressBar = true;
		dataMiner.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
		BuildingTemplates.CreateComplexFabricatorStorage(go, dataMiner);
		ComplexRecipe.RecipeElement[] array = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(this.INPUT_MATERIAL_TAG, 5f)
		};
		ComplexRecipe.RecipeElement[] array2 = new ComplexRecipe.RecipeElement[]
		{
			new ComplexRecipe.RecipeElement(this.OUTPUT_MATERIAL_TAG, 1f, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature, false)
		};
		string text = ComplexRecipeManager.MakeObsoleteRecipeID("DataMiner", this.OUTPUT_MATERIAL_TAG);
		string text2 = ComplexRecipeManager.MakeRecipeID("DataMiner", array, array2);
		ComplexRecipe complexRecipe = new ComplexRecipe(text2, array, array2);
		complexRecipe.time = 200f;
		complexRecipe.description = string.Format(global::STRINGS.BUILDINGS.PREFABS.EGGCRACKER.RECIPE_DESCRIPTION, ElementLoader.FindElementByHash(this.INPUT_MATERIAL).name, this.OUTPUT_MATERIAL_NAME);
		complexRecipe.fabricators = new List<Tag> { TagManager.Create("DataMiner") };
		complexRecipe.nameDisplay = ComplexRecipe.RecipeNameDisplay.IngredientToResult;
		complexRecipe.sortOrder = 300;
		ComplexRecipeManager.Get().AddObsoleteIDMapping(text, text2);
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}

	public const string ID = "DataMiner";

	public const float POWER_USAGE_W = 1000f;

	public const float BASE_UNITS_PRODUCED_PER_CYCLE = 3f;

	public const float BASE_DTU_PRODUCTION = 3f;

	public const float STORAGE_CAPACITY_KG = 1000f;

	public const float MASS_CONSUMED_PER_BANK_KG = 5f;

	public const float BASE_DURATION_SECONDS = 200f;

	public static MathUtil.MinMax PRODUCTION_RATE_SCALE = new MathUtil.MinMax(0.6f, 5.3333335f);

	public static MathUtil.MinMax TEMPERATURE_SCALING_RANGE = new MathUtil.MinMax(10f, 325f);

	public SimHashes INPUT_MATERIAL = SimHashes.Polypropylene;

	public Tag INPUT_MATERIAL_TAG = SimHashes.Polypropylene.CreateTag();

	public Tag OUTPUT_MATERIAL_TAG = DatabankHelper.TAG;

	public string OUTPUT_MATERIAL_NAME = DatabankHelper.NAME;

	public const float BASE_PRODUCTION_PROGRESS_PER_TICK = 0.001f;
}
