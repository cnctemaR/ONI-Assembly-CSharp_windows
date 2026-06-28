using System;
using TUNING;
using UnityEngine;

public class LiquidHeaterConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(LiquidHeaterConfig.ID, 4, 1, "boiler_kanim", 800f, 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 3200f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.PENALTY.TIER1, null);
		buildingDef.RequiresPowerInput = true;
		buildingDef.Floodable = false;
		buildingDef.EnergyConsumptionWhenActive = 960f;
		buildingDef.ExhaustKilowattsWhenActive = 4000f;
		buildingDef.OperatingKilowatts = 64f;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.MaterialCategory = MATERIALS.ALL_METALS;
		buildingDef.AudioCategory = "SolidMetal";
		buildingDef.OverheatTemperature = 398.15f;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<LoopingSounds>();
		SpaceHeater spaceHeater = go.AddOrGet<SpaceHeater>();
		spaceHeater.SetLiquidHeater();
		spaceHeater.targetTemperature = 358.15f;
		spaceHeater.minimumCellMass = 400f;
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

	public const float CONSUMPTION_RATE = 1f;

	public static string ID = "LiquidHeater";
}
