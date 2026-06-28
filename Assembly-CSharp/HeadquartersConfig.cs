using System;
using TUNING;
using UnityEngine;

public class HeadquartersConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("Headquarters", 4, 4, "hqbase_kanim", 200f, 250, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER7, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.BONUS.TIER5, null);
		buildingDef.Floodable = false;
		buildingDef.Relocatable = false;
		buildingDef.MaterialCategory = MATERIALS.ALL_METALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.BaseTimeUntilRepair = 400f;
		buildingDef.ShowInBuildMenu = false;
		buildingDef.DefaultAnimState = "idle";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.AddOrGet<Telepad>();
		Light2D light2D = go.AddOrGet<Light2D>();
		light2D.Color = LIGHT2D.HEADQUARTERS_COLOR;
		light2D.Range = 5f;
		light2D.Offset = LIGHT2D.HEADQUARTERS_OFFSET;
		light2D.overlayColour = LIGHT2D.HEADQUARTERS_OVERLAYCOLOR;
		light2D.shape = LightShape.Circle;
		light2D.Intensity = 4f;
		light2D.drawOverlay = true;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}
}
