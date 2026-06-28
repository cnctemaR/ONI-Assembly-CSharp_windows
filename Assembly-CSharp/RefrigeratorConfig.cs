using System;
using TUNING;
using UnityEngine;

public class RefrigeratorConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		EffectorValues tier = NOISE_POLLUTION.NOISY.TIER0;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("Refrigerator", 1, 2, "fridge_kanim", 100f, 30, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.RAW_MINERALS, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.BONUS.TIER1, tier);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.ExhaustKilowattsWhenActive = 0.5f;
		buildingDef.Floodable = false;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.AudioCategory = "Metal";
		SoundEventVolumeCache.instance.AddVolume("fridge_kanim", "Refrigerator_open", NOISE_POLLUTION.NOISY.TIER1);
		SoundEventVolumeCache.instance.AddVolume("fridge_kanim", "Refrigerator_close", NOISE_POLLUTION.NOISY.TIER1);
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
		Prioritizable.AddRef(go);
		TreeFilterable treeFilterable = go.AddOrGet<TreeFilterable>();
		treeFilterable.AddTagToFilter(GameTags.Edible);
		treeFilterable.AddTagToFilter(GameTags.CookingIngredient);
		Refrigerator refrigerator = go.AddOrGet<Refrigerator>();
		refrigerator.noFilterTint = new Color(0.5019608f, 0.5019608f, 0.5019608f, 1f);
		refrigerator.filterTint = new Color(1f, 1f, 1f, 1f);
		go.AddOrGet<UserMenu>();
		go.AddOrGet<DropAllWorkable>();
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

	public const string ID = "Refrigerator";
}
