using System;
using TUNING;
using UnityEngine;

public class SuitMarkerConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "SuitMarker";
		int num = 1;
		int num2 = 3;
		string text2 = "changingarea_arrow_kanim";
		int num3 = 30;
		float num4 = 30f;
		string[] refined_METALS = MATERIALS.REFINED_METALS;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, new float[]
		{
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER2[0],
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER1[0]
		}, refined_METALS, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.BONUS.TIER1, none, 0.2f);
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		buildingDef.PreventIdlingInFrontOfBuilding = true;
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SuitIDs, "SuitMarker");
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<SuitMarker>();
		AnimTileable animTileable = go.AddOrGet<AnimTileable>();
		animTileable.tags = new Tag[]
		{
			new Tag("SuitMarker"),
			new Tag("SuitLocker")
		};
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}

	public const string ID = "SuitMarker";
}
