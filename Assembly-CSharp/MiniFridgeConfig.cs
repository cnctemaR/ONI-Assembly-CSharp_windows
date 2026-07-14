using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class MiniFridgeConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("MiniFridge", 1, 1, "minifridge_kanim", 30, 10f, new float[]
		{
			global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2[0],
			1f
		}, new string[] { "BuildableRaw", "BuildingGasket" }, 2400f, BuildLocationRule.OnFloor, global::TUNING.BUILDINGS.DECOR.BONUS.TIER1, NOISE_POLLUTION.NONE, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.AddLogicPowerPort = false;
		buildingDef.EnergyConsumptionWhenActive = 60f;
		buildingDef.SelfHeatKilowattsWhenActive = 0.125f;
		buildingDef.ExhaustKilowattsWhenActive = 0f;
		buildingDef.LogicOutputPorts = new List<LogicPorts.Port> { LogicPorts.Port.OutputPort(FilteredStorage.FULL_PORT_ID, new CellOffset(0, 0), global::STRINGS.BUILDINGS.PREFABS.REFRIGERATOR.LOGIC_PORT, global::STRINGS.BUILDINGS.PREFABS.REFRIGERATOR.LOGIC_PORT_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.REFRIGERATOR.LOGIC_PORT_INACTIVE, false, false) };
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		buildingDef.ForegroundLayer = Grid.SceneLayer.BuildingBack;
		buildingDef.Floodable = false;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AddSearchTerms(SEARCH_TERMS.FRIDGE);
		return buildingDef;
	}

	public override string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC5;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		base.ConfigureBuildingTemplate(go, prefab_tag);
		LoreBearerUtil.AddPOILoreSupport(go);
		go.AddTag(RoomConstraints.ConstraintTags.KitchenRefrigerator);
		Storage storage = go.AddOrGet<Storage>();
		storage.showInUI = true;
		storage.showDescriptor = true;
		storage.storageFilters = STORAGEFILTERS.FOOD;
		storage.allowItemRemoval = true;
		storage.capacityKg = 50f;
		storage.storageFullMargin = STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
		storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
		storage.showCapacityStatusItem = true;
		Prioritizable.AddRef(go);
		go.AddOrGet<TreeFilterable>().allResourceFilterLabelString = UI.UISIDESCREENS.TREEFILTERABLESIDESCREEN.ALLBUTTON_EDIBLES;
		go.AddOrGet<FoodStorage>();
		go.AddOrGet<Refrigerator>();
		go.AddOrGet<DiscoverResources>().Add("FieldRation", GameTags.Edible);
		RefrigeratorController.Def def = go.AddOrGetDef<RefrigeratorController.Def>();
		def.powerSaverEnergyUsage = 10f;
		def.coolingHeatKW = 0.1875f;
		def.steadyHeatKW = 0f;
		go.AddOrGet<UserNameable>();
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGetDef<RocketUsageRestriction.Def>().restrictOperational = false;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<ShelfDisplay>();
		go.AddOrGetDef<StorageController.Def>();
		SymbolOverrideControllerUtil.AddToPrefab(go);
	}

	public const string ID = "MiniFridge";

	private const int ENERGY_SAVER_POWER = 10;
}
