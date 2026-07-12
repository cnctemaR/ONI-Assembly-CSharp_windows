using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class RefrigeratorConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "Refrigerator";
		int num = 1;
		int num2 = 2;
		string text2 = "fridge_kanim";
		int num3 = 30;
		float num4 = 10f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] raw_MINERALS = MATERIALS.RAW_MINERALS;
		float num5 = 800f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER0;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, raw_MINERALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.BONUS.TIER1, tier2, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.SelfHeatKilowattsWhenActive = 0.125f;
		buildingDef.ExhaustKilowattsWhenActive = 0f;
		buildingDef.LogicOutputPorts = new List<LogicPorts.Port> { LogicPorts.Port.OutputPort(FilteredStorage.FULL_PORT_ID, new CellOffset(0, 1), global::STRINGS.BUILDINGS.PREFABS.REFRIGERATOR.LOGIC_PORT, global::STRINGS.BUILDINGS.PREFABS.REFRIGERATOR.LOGIC_PORT_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.REFRIGERATOR.LOGIC_PORT_INACTIVE, false, false) };
		buildingDef.Floodable = false;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "Metal";
		SoundEventVolumeCache.instance.AddVolume("fridge_kanim", "Refrigerator_open", NOISE_POLLUTION.NOISY.TIER1);
		SoundEventVolumeCache.instance.AddVolume("fridge_kanim", "Refrigerator_close", NOISE_POLLUTION.NOISY.TIER1);
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Storage storage = go.AddOrGet<Storage>();
		storage.showInUI = true;
		storage.showDescriptor = true;
		storage.storageFilters = STORAGEFILTERS.FOOD;
		storage.allowItemRemoval = true;
		storage.capacityKg = 100f;
		storage.storageFullMargin = STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
		storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
		storage.showCapacityStatusItem = true;
		Prioritizable.AddRef(go);
		go.AddOrGet<TreeFilterable>();
		go.AddOrGet<Refrigerator>();
		RefrigeratorController.Def def = go.AddOrGetDef<RefrigeratorController.Def>();
		def.powerSaverEnergyUsage = 20f;
		def.coolingHeatKW = 0.375f;
		def.steadyHeatKW = 0f;
		go.AddOrGet<UserNameable>();
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGetDef<RocketUsageRestriction.Def>().restrictOperational = false;
		go.AddOrGetDef<StorageController.Def>();
	}

	public const string ID = "Refrigerator";

	private const int ENERGY_SAVER_POWER = 20;
}
