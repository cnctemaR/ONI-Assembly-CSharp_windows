using System;
using TUNING;
using UnityEngine;

public class AdvancedResearchCenterConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("AdvancedResearchCenter", 3, 3, "research_center2_kanim", 200f, 30f, BUILDINGS.CONSTRUCTION_MASS.TIER4, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.NONE, null);
		buildingDef.RequiresPower = true;
		buildingDef.EnergyConsumptionWhenActive = 60f;
		buildingDef.TemperatureModificationWhenActive = 4f;
		buildingDef.OperatingTemperature = 400f;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.MaterialCategory = MATERIALS.ALL_METALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "large";
		buildingDef.MinionEffect = "RestfulSleep";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		ResearchCenter researchCenter = go.AddOrGet<ResearchCenter>();
		researchCenter.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_research2_kanim") };
		go.AddOrGet<Prioritizable>();
		BuildingTemplates.CreateFabricatorStorage(go, researchCenter);
	}

	public override void DoPostConfigure(GameObject go)
	{
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			PoweredActiveController.Instance instance = new PoweredActiveController.Instance(game_object.GetComponent<KPrefabID>());
			instance.StartSM();
		};
		BuildingTemplates.DoPostConfigure(go);
	}
}
