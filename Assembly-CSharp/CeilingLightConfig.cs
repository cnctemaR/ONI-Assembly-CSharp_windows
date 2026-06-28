using System;
using TUNING;
using UnityEngine;

public class CeilingLightConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("CeilingLight", 1, 1, "ceilinglight_kanim", 100f, 10, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnCeiling, BUILDINGS.DECOR.NONE, none);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 10f;
		buildingDef.OperatingKilowatts = 0.5f;
		buildingDef.ViewMode = SimViewMode.Light;
		buildingDef.MaterialCategory = MATERIALS.ALL_METALS;
		buildingDef.AudioCategory = "Metal";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<LoopingSounds>();
		go.AddOrGet<EnergyConsumer>();
		Light2D light2D = go.AddOrGet<Light2D>();
		light2D.overlayColour = LIGHT2D.CEILINGLIGHT_OVERLAYCOLOR;
		light2D.Color = LIGHT2D.CEILINGLIGHT_COLOR;
		light2D.Range = 8f;
		light2D.Angle = 2.6f;
		light2D.Direction = LIGHT2D.CEILINGLIGHT_DIRECTION;
		light2D.Offset = LIGHT2D.CEILINGLIGHT_OFFSET;
		light2D.shape = LightShape.Cone;
		light2D.drawOverlay = true;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			LightController.Instance instance = new LightController.Instance(game_object.GetComponent<KPrefabID>());
			instance.StartSM();
		};
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject preview_go)
	{
		LightShapePreview lightShapePreview = preview_go.AddComponent<LightShapePreview>();
		lightShapePreview.radius = 8f;
		lightShapePreview.shape = LightShape.Cone;
	}
}
