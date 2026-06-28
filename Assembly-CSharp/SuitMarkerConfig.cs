using System;
using TUNING;
using UnityEngine;

public class SuitMarkerConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string[] array = new string[] { "Metal" };
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("SuitMarker", 1, 3, "changingarea_arrow_kanim", 50f, 30, 30f, new float[]
		{
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER2[0],
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER1[0]
		}, array, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.BONUS.TIER1, none);
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		buildingDef.PreventIdlingInFrontOfBuilding = true;
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SuitIDs, "SuitMarker");
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
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
