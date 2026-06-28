using System;
using TUNING;
using UnityEngine;

public class WireRefinedBridgeHighWattageConfig : WireBridgeHighWattageConfig
{
	protected override string GetID()
	{
		return "WireRefinedBridgeHighWattage";
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = base.CreateBuildingDef();
		buildingDef.AnimFiles = new KAnimFile[] { Assets.GetAnim("heavywatttile_conductive_kanim") };
		buildingDef.Mass = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
		buildingDef.MaterialCategory = MATERIALS.REFINED_METALS;
		buildingDef.HotKey = global::Action.BuildMenuKeyE;
		return buildingDef;
	}

	protected override WireUtilityNetworkLink AddNetworkLink(GameObject go)
	{
		WireUtilityNetworkLink wireUtilityNetworkLink = base.AddNetworkLink(go);
		wireUtilityNetworkLink.maxWattageRating = Wire.WattageRating.Max50000;
		return wireUtilityNetworkLink;
	}

	public new const string ID = "WireRefinedBridgeHighWattage";
}
