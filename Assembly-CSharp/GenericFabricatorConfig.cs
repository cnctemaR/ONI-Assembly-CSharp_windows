using System;
using TUNING;
using UnityEngine;

public class GenericFabricatorConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		EffectorValues tier = NOISE_POLLUTION.NOISY.TIER3;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("GenericFabricator", 3, 3, "fabricator_generic_kanim", 1200f, 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER2, tier);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 240f;
		buildingDef.ExhaustKilowattsWhenActive = 0.5f;
		buildingDef.OperatingKilowatts = 2f;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.MaterialCategory = MATERIALS.ALL_METALS;
		buildingDef.AudioCategory = "Glass";
		buildingDef.AudioSize = "large";
		buildingDef.Deprecated = true;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.UpdateComponentRequirement<DropAllWorkable>(true);
		Prioritizable.AddRef(go);
		go.UpdateComponentRequirement<BuildingComplete>(true).isManuallyOperated = true;
		Fabricator fabricator = go.UpdateComponentRequirement<Fabricator>(true);
		fabricator.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_fabricator_generic_kanim") };
		BuildingTemplates.CreateFabricatorStorage(go, fabricator);
		go.UpdateComponentRequirement<LoopingSounds>(true);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			PoweredActiveStoppableController.Instance instance = new PoweredActiveStoppableController.Instance(game_object.GetComponent<KPrefabID>());
			instance.StartSM();
		};
	}

	public const string ID = "GenericFabricator";
}
