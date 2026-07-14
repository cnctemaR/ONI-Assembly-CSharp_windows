using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class WireRubberBridgeConfig : WireBridgeConfig
{
	protected override string GetID()
	{
		return "WireRubberBridge";
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = base.CreateBuildingDef();
		buildingDef.AnimFiles = new KAnimFile[] { Assets.GetAnim("utilityelectricbridgerubber_kanim") };
		buildingDef.Mass = new float[]
		{
			global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0[0],
			global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER_SMALL[0]
		};
		buildingDef.MaterialCategory = new string[] { "RefinedMetal", "Rubber&Plastic" };
		buildingDef.AddSearchTerms(SEARCH_TERMS.POWER);
		buildingDef.AddSearchTerms(SEARCH_TERMS.WIRE);
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.WireIDs, "WireRubberBridge");
		return buildingDef;
	}

	protected override WireUtilityNetworkLink AddNetworkLink(GameObject go)
	{
		WireUtilityNetworkLink wireUtilityNetworkLink = base.AddNetworkLink(go);
		wireUtilityNetworkLink.maxWattageRating = Wire.WattageRating.Max4000;
		return wireUtilityNetworkLink;
	}

	public new const string ID = "WireRubberBridge";
}
