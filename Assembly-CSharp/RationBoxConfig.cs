using System;
using TUNING;
using UnityEngine;

public class RationBoxConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("RationBox", 2, 2, "rationbox_kanim", 100f, 10, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.RAW_MINERALS, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.BONUS.TIER0, null);
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.AudioCategory = "Metal";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<Prioritizable>();
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 150f;
		storage.disableOnStore = true;
		storage.showInUI = true;
		storage.showDescriptor = true;
		storage.storageFilters = STORAGEFILTERS.FOOD;
		storage.allowItemRemoval = true;
		TreeFilterable treeFilterable = go.AddOrGet<TreeFilterable>();
		treeFilterable.AddTagToFilter(GameTags.Edible);
		go.AddOrGet<UserMenu>();
		RationBox rationBox = go.AddOrGet<RationBox>();
		rationBox.noFilterTint = new Color(0.5147059f, 0.5147059f, 0.5147059f, 1f);
		rationBox.filterTint = new Color(1f, 1f, 1f, 1f);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			StorageController.Instance instance = new StorageController.Instance(game_object.GetComponent<KPrefabID>());
			instance.StartSM();
		};
	}
}
