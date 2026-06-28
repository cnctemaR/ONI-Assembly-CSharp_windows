using System;
using TUNING;
using UnityEngine;

public class CookingStationConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("CookingStation", 3, 2, "cookstation_kanim", 200f, 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.NONE, null);
		BuildingTemplates.CreateElectricalBuildingDef(buildingDef);
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "large";
		buildingDef.EnergyConsumptionWhenActive = 60f;
		buildingDef.ExhaustKilowattsWhenActive = 0.5f;
		buildingDef.OperatingKilowatts = 1f;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		CookingStation cookingStation = go.AddOrGet<CookingStation>();
		cookingStation.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_cookstation_kanim") };
		go.AddOrGet<Prioritizable>();
		go.AddOrGet<DropAllWorkable>();
		BuildingTemplates.CreateFabricatorStorage(go, cookingStation);
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

	public const string ID = "CookingStation";
}
