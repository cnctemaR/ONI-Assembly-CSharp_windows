using System;
using TUNING;
using UnityEngine;

public class ApothecaryConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "Apothecary";
		int num = 2;
		int num2 = 3;
		string text2 = "apothecary_kanim";
		float num3 = 400f;
		int num4 = 30;
		float num5 = 120f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num6 = 800f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, num5, tier, all_METALS, num6, buildLocationRule, BUILDINGS.DECOR.NONE, none);
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
