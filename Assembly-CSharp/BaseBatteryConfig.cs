using System;
using UnityEngine;

public abstract class BaseBatteryConfig : IBuildingConfig
{
	public BuildingDef CreateBuildingDef(string id, int width, int height, string anim, float mass, float construction_time, float[] construction_mass, string[] construction_materials, float melting_point, float generator_wattage_rating, float generator_base_capacity, float temperature_modification_when_active, float operating_temperature, DecorValues decor)
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, mass, construction_time, construction_mass, construction_materials, melting_point, BuildLocationRule.OnFloor, decor, null);
		buildingDef.ExplosionSize = Overheatable.ExplosionSize.Large;
		buildingDef.GeneratorWattageRating = generator_wattage_rating;
		buildingDef.GeneratorBaseCapacity = generator_base_capacity;
		buildingDef.TemperatureModificationWhenActive = temperature_modification_when_active;
		buildingDef.OperatingTemperature = operating_temperature;
		buildingDef.Entombable = false;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.MaterialCategory = construction_materials;
		buildingDef.AudioCategory = "Metal";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddComponent<RequireInputs>();
		Battery battery = go.AddOrGet<Battery>();
		battery.powerSortOrder = 1000;
		Generator generator = go.AddOrGet<Generator>();
		generator.powerDistributionOrder = 10;
	}

	public override void DoPostConfigure(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			PoweredActiveController.Instance instance = new PoweredActiveController.Instance(game_object.GetComponent<KPrefabID>());
			instance.StartSM();
		};
	}
}
