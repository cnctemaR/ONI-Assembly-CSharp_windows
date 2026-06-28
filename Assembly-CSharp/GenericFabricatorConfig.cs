using System;
using TUNING;
using UnityEngine;

public class GenericFabricatorConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "GenericFabricator";
		int num = 3;
		int num2 = 3;
		string text2 = "fabricator_generic_kanim";
		float num3 = 1200f;
		int num4 = 30;
		float num5 = 30f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num6 = 800f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER3;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, num5, tier, all_METALS, num6, buildLocationRule, BUILDINGS.DECOR.PENALTY.TIER2, tier2);
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
