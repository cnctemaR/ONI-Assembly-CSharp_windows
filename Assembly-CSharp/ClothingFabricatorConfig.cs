using System;
using TUNING;
using UnityEngine;

public class ClothingFabricatorConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("ClothingFabricator", 4, 3, "clothingfactory_kanim", 800f, 100, 240f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.NONE, null);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 240f;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.MaterialCategory = MATERIALS.ALL_METALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.PowerInputOffset = new CellOffset(2, 0);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGet<Prioritizable>();
		Fabricator fabricator = go.AddOrGet<Fabricator>();
		fabricator.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_clothingfactory_kanim") };
		fabricator.AnimOffset = new Vector3(-1f, 0f, 0f);
		BuildingTemplates.CreateFabricatorStorage(go, fabricator);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			PoweredActiveController.Instance instance = new PoweredActiveController.Instance(game_object.GetComponent<KPrefabID>());
			instance.StartSM();
		};
	}

	public const string ID = "ClothingFabricator";
}
