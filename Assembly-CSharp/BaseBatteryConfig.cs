using System;
using UnityEngine;

public abstract class BaseBatteryConfig : IBuildingConfig
{
	public BuildingDef CreateBuildingDef(string id, int width, int height, int hitpoints, string anim, float mass, float construction_time, float[] construction_mass, string[] construction_materials, float melting_point, float exhaust_temperature_active, float operating_kilowatts, DecorValues decor)
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, mass, hitpoints, construction_time, construction_mass, construction_materials, melting_point, BuildLocationRule.OnFloor, decor, null);
		buildingDef.ExhaustKilowattsWhenActive = exhaust_temperature_active;
		buildingDef.OperatingKilowatts = operating_kilowatts;
		buildingDef.Entombable = false;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.MaterialCategory = construction_materials;
		buildingDef.AudioCategory = "Metal";
		buildingDef.RequiresPowerOutput = true;
		buildingDef.UseWhitePowerOutputConnectorColour = true;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddComponent<RequireInputs>();
		Battery battery = go.AddOrGet<Battery>();
		battery.powerSortOrder = 1000;
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
}
