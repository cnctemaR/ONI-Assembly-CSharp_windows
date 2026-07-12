using System;
using TUNING;
using UnityEngine;

public class SweepBotStationConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "SweepBotStation";
		int num = 2;
		int num2 = 2;
		string text2 = "sweep_bot_base_station_kanim";
		int num3 = 30;
		float num4 = 30f;
		float[] array = new float[] { BUILDINGS.CONSTRUCTION_MASS_KG.TIER2[0] - SweepBotConfig.MASS };
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, array, refined_METALS, num5, buildLocationRule, BUILDINGS.DECOR.PENALTY.TIER1, none, 0.2f);
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.Floodable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 240f;
		buildingDef.ExhaustKilowattsWhenActive = 0f;
		buildingDef.SelfHeatKilowattsWhenActive = 1f;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Prioritizable.AddRef(go);
		Storage storage = go.AddComponent<Storage>();
		storage.showInUI = true;
		storage.allowItemRemoval = false;
		storage.ignoreSourcePriority = true;
		storage.showDescriptor = false;
		storage.storageFilters = STORAGEFILTERS.STORAGE_LOCKERS_STANDARD;
		storage.storageFullMargin = STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
		storage.fetchCategory = Storage.FetchCategory.Building;
		storage.capacityKg = 25f;
		storage.allowClearable = false;
		Storage storage2 = go.AddComponent<Storage>();
		storage2.showInUI = true;
		storage2.allowItemRemoval = true;
		storage2.ignoreSourcePriority = true;
		storage2.showDescriptor = true;
		storage2.storageFilters = STORAGEFILTERS.STORAGE_LOCKERS_STANDARD;
		storage2.storageFullMargin = STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
		storage2.fetchCategory = Storage.FetchCategory.StorageSweepOnly;
		storage2.capacityKg = 1000f;
		storage2.allowClearable = true;
		storage2.showCapacityStatusItem = true;
		go.AddOrGet<CharacterOverlay>().shouldShowName = true;
		go.AddOrGet<SweepBotStation>().SetStorages(storage, storage2);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGetDef<StorageController.Def>();
	}

	public const string ID = "SweepBotStation";

	public const float POWER_USAGE = 240f;
}
