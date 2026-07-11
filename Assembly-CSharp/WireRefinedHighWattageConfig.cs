using System;
using TUNING;
using UnityEngine;

public class WireRefinedHighWattageConfig : BaseWireConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "WireRefinedHighWattage";
		string text2 = "utilities_electric_conduct_hiwatt_kanim";
		float num = 3f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
		float num2 = 0.05f;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = base.CreateBuildingDef(text, text2, num, tier, num2, BUILDINGS.DECOR.PENALTY.TIER3, none);
		buildingDef.MaterialCategory = MATERIALS.REFINED_METALS;
		buildingDef.BuildLocationRule = BuildLocationRule.NotInTiles;
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		base.DoPostConfigureComplete(Wire.WattageRating.Max20000, go);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		Constructable component = go.GetComponent<Constructable>();
		component.requiredSkillPerk = Db.Get().SkillPerks.CanPowerTinker.Id;
	}

	public const string ID = "WireRefinedHighWattage";
}
