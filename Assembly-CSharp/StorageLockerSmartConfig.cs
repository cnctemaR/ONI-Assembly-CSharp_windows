using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class StorageLockerSmartConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "StorageLockerSmart";
		int num = 1;
		int num2 = 2;
		string text2 = "smartstoragelocker_kanim";
		int num3 = 30;
		float num4 = 60f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, refined_METALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.PENALTY.TIER1, none, 0.2f);
		buildingDef.Floodable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.Overheatable = false;
		buildingDef.ViewMode = SimViewMode.Logic;
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 60f;
		buildingDef.ExhaustKilowattsWhenActive = 0.125f;
		return buildingDef;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, StorageLockerSmartConfig.OUTPUT_PORT);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, StorageLockerSmartConfig.OUTPUT_PORT);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, StorageLockerSmartConfig.OUTPUT_PORT);
		SoundEventVolumeCache.instance.AddVolume("storagelocker_kanim", "StorageLocker_Hit_metallic_low", NOISE_POLLUTION.NOISY.TIER1);
		Prioritizable.AddRef(go);
		Storage storage = go.AddOrGet<Storage>();
		storage.showInUI = true;
		storage.allowItemRemoval = true;
		storage.showDescriptor = true;
		storage.storageFilters = STORAGEFILTERS.NOT_EDIBLE_SOLIDS;
		StorageLockerSmart storageLockerSmart = go.AddOrGet<StorageLockerSmart>();
		storageLockerSmart.noFilterTint = new Color(0.5019608f, 0.5019608f, 0.5019608f, 1f);
		storageLockerSmart.filterTint = new Color(1f, 1f, 1f, 1f);
		go.AddOrGet<UserMenu>();
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			StorageController.Instance instance = new StorageController.Instance(game_object.GetComponent<KPrefabID>());
			instance.StartSM();
		};
	}

	public const string ID = "StorageLockerSmart";

	private static readonly LogicPorts.Port OUTPUT_PORT = LogicPorts.Port.OutputPort(FilteredStorage.FULL_PORT_ID, new CellOffset(0, 1), global::STRINGS.BUILDINGS.PREFABS.REFRIGERATOR.LOGIC_PORT_DESC, true);
}
