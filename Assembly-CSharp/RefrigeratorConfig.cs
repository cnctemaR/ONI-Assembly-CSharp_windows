using System;
using TUNING;
using UnityEngine;

public class RefrigeratorConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("Refrigerator", 1, 2, "fridge_kanim", 100f, 10f, BUILDINGS.CONSTRUCTION_MASS.TIER4, MATERIALS.RAW_MINERALS, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.BONUS.TIER1, null);
		buildingDef.RequiresPower = true;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.TemperatureModificationWhenActive = 8f;
		buildingDef.OperatingTemperature = 400f;
		buildingDef.Floodable = false;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.AudioCategory = "Metal";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		Storage storage = go.AddOrGet<Storage>();
		storage.disableOnStore = true;
		storage.showInUI = true;
		storage.showDescriptor = true;
		storage.storageFilters = STORAGEFILTERS.FOOD;
		storage.allowItemRemoval = true;
		storage.capacityKg = 100f;
		go.AddOrGet<Prioritizable>();
		TreeFilterable treeFilterable = go.AddOrGet<TreeFilterable>();
		treeFilterable.AddTagToFilter(GameTags.Edible);
		Refrigerator refrigerator = go.AddOrGet<Refrigerator>();
		refrigerator.noFilterTint = new Color(0.5019608f, 0.5019608f, 0.5019608f, 1f);
		refrigerator.filterTint = new Color(1f, 1f, 1f, 1f);
		go.AddOrGet<UserMenu>();
		go.AddOrGet<DropAllWorkable>();
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
