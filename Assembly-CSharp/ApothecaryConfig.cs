using System;
using TUNING;
using UnityEngine;

public class ApothecaryConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("Apothecary", 2, 3, "apothecary_kanim", 400f, 30, 120f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.NONE, none);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 240f;
		buildingDef.ExhaustKilowattsWhenActive = 0.25f;
		buildingDef.OperatingKilowatts = 0.5f;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.MaterialCategory = MATERIALS.ALL_METALS;
		buildingDef.AudioCategory = "Glass";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<DropAllWorkable>();
		Prioritizable.AddRef(go);
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		Fabricator fabricator = go.AddOrGet<Fabricator>();
		BuildingTemplates.CreateFabricatorStorage(go, fabricator);
		fabricator.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_apothecary_kanim") };
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			PoweredActiveStoppableController.Instance instance = new PoweredActiveStoppableController.Instance(game_object.GetComponent<KPrefabID>());
			instance.StartSM();
			Fabricator component = game_object.GetComponent<Fabricator>();
			component.SetAttributeConverter(Db.Get().AttributeConverters.CompoundingSpeed);
		};
	}

	public const string ID = "Apothecary";
}
