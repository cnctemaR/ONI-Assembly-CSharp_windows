using System;
using TUNING;
using UnityEngine;

public class StorageLockerConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("StorageLocker", 1, 2, "storagelocker_kanim", 100f, 10f, BUILDINGS.CONSTRUCTION_MASS.TIER4, MATERIALS.RAW_MINERALS, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER1, null);
		buildingDef.Floodable = false;
		buildingDef.AudioCategory = "Metal";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		Storage storage = go.AddOrGet<Storage>();
		go.AddOrGet<Prioritizable>();
		storage.disableOnStore = true;
		storage.showInUI = true;
		storage.allowItemRemoval = true;
		storage.showDescriptor = true;
		StorageLocker storageLocker = go.AddOrGet<StorageLocker>();
		storageLocker.noFilterTint = new Color(0.5019608f, 0.5019608f, 0.5019608f, 1f);
		storageLocker.filterTint = new Color(1f, 1f, 1f, 1f);
		storage.storageFilters = STORAGEFILTERS.DEFAULT;
		go.AddOrGet<UserMenu>();
		go.AddOrGet<Upgradable>();
		string prefabID = go.GetComponent<Building>().Def.PrefabID;
		Upgradable.UpgradableConfig.UpgradeModifier[] array = new Upgradable.UpgradableConfig.UpgradeModifier[]
		{
			new Upgradable.UpgradableConfig.UpgradeModifier(Upgradable.Upgrade.Target.Capacity, 2f)
		};
		Upgradable.AddToUpgradableConfigs(new Upgradable.UpgradableConfig(prefabID, 0, 1, 120f, "Metal", 400f, array));
	}

	public override void DoPostConfigure(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			StorageController.Instance instance = new StorageController.Instance(game_object.GetComponent<KPrefabID>());
			instance.StartSM();
		};
	}
}
