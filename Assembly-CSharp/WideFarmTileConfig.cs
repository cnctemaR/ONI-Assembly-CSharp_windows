using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class WideFarmTileConfig : IBuildingConfig
{
	public override string[] GetRequiredDlcIds()
	{
		return new string[] { "DLC5_ID" };
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("WideFarmTile", 3, 1, "farmTile_wide_kanim", 250, 60f, new float[] { 200f, 100f }, new string[] { "Metal", "Glasses" }, 800f, BuildLocationRule.Tile, DECOR.BONUS.TIER1, NOISE_POLLUTION.NOISY.TIER0, 0.2f);
		BuildingTemplates.CreateFoundationTileDef(buildingDef);
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.OutputConduitType = ConduitType.None;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
		buildingDef.RequiresPowerInput = false;
		buildingDef.RequiresPowerOutput = false;
		buildingDef.PowerInputOffset = new CellOffset(0, 0);
		buildingDef.PowerOutputOffset = new CellOffset(0, 0);
		buildingDef.UseHighEnergyParticleInputPort = false;
		buildingDef.UseHighEnergyParticleOutputPort = false;
		buildingDef.HighEnergyParticleInputOffset = new CellOffset(0, 0);
		buildingDef.HighEnergyParticleOutputOffset = new CellOffset(0, 0);
		buildingDef.PermittedRotations = PermittedRotations.Unrotatable;
		buildingDef.DragBuild = false;
		buildingDef.Replaceable = true;
		buildingDef.PreventBuildOverPlants = true;
		buildingDef.ExhaustKilowattsWhenActive = 0f;
		buildingDef.SelfHeatKilowattsWhenActive = 0f;
		buildingDef.UseStructureTemperature = true;
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.Disinfectable = true;
		buildingDef.Entombable = false;
		buildingDef.Repairable = true;
		buildingDef.ForegroundLayer = Grid.SceneLayer.BuildingBack;
		buildingDef.SceneLayer = Grid.SceneLayer.TileMain;
		buildingDef.AddSearchTerms(SEARCH_TERMS.FOOD);
		buildingDef.AddSearchTerms(SEARCH_TERMS.FARM);
		buildingDef.AudioCategory = "Metal";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		KPrefabID component = go.GetComponent<KPrefabID>();
		component.AddTag(GameTags.CodexCategories.FarmBuilding, false);
		GeneratedBuildings.MakeBuildingAlwaysOperational(go);
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
		simCellOccupier.doReplaceElement = true;
		simCellOccupier.notifyOnMelt = true;
		go.AddOrGet<TileTemperature>();
		BuildingTemplates.CreateDefaultStorage(go, false).SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		PlantablePlot plantablePlot = go.AddOrGet<PlantablePlot>();
		plantablePlot.occupyingObjectRelativePosition = new Vector3(0f, 1f, 0f);
		plantablePlot.AddDepositTag(GameTags.LargeSeed);
		plantablePlot.AddAdditionalCriteria(new Func<GameObject, bool>(WideFarmTileConfig.MatchesTags));
		plantablePlot.SetFertilizationFlags(true, true);
		plantablePlot.SetReceptacleDirection(SingleEntityReceptacle.ReceptacleDirection.Top);
		go.AddOrGetDef<WideFarmTile.Def>();
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.capacityTag = GameTags.Liquid;
		conduitConsumer.capacityKG = 5f;
		conduitConsumer.consumptionRate = 1f;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		go.AddOrGet<CopyBuildingSettings>().copyGroupTag = GameTags.Farm;
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<LoopingSounds>();
		component.prefabInitFn += this.OnPrefabInit;
	}

	private void OnPrefabInit(GameObject instance)
	{
		instance.AddOrGet<PlantablePlot>().AddAdditionalCriteria(new Func<GameObject, bool>(WideFarmTileConfig.MatchesTags));
	}

	private static bool MatchesTags(GameObject objInQuestion)
	{
		KPrefabID component = objInQuestion.GetComponent<KPrefabID>();
		bool flag = component.HasTag(GameTags.BackwallSeed);
		return component.HasTag(GameTags.LargeSeed) && !flag;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<KBatchedAnimController>().initialBlendParameters = 4;
		go.GetComponent<RequireInputs>().requireConduitHasMass = false;
	}

	public const string ID = "WideFarmTile";
}
