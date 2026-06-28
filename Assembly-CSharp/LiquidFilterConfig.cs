using System;
using TUNING;
using UnityEngine;

public class LiquidFilterConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "LiquidFilter";
		int num = 3;
		int num2 = 1;
		string text2 = "filter_liquid_kanim";
		float num3 = 200f;
		int num4 = 30;
		float num5 = 10f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] raw_METALS = MATERIALS.RAW_METALS;
		float num6 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.Anywhere;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER1;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, num5, tier, raw_METALS, num6, buildLocationRule, BUILDINGS.DECOR.PENALTY.TIER0, tier2);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.OperatingKilowatts = 4f;
		buildingDef.ExhaustKilowattsWhenActive = 0f;
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.OutputConduitType = ConduitType.Liquid;
		buildingDef.Floodable = false;
		buildingDef.ViewMode = SimViewMode.LiquidVentMap;
		buildingDef.MaterialCategory = MATERIALS.RAW_METALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(1, 0);
		buildingDef.PermittedRotations = PermittedRotations.R360;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.GetComponent<KPrefabID>().AddPrefabTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<Structure>();
		ElementFilter elementFilter = go.AddOrGet<ElementFilter>();
		elementFilter.conduitType = ConduitType.Liquid;
		elementFilter.filterOffset = new CellOffset(1, 0);
		go.AddOrGet<LiquidFilterable>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			new PoweredActiveController.Instance(game_object.GetComponent<KPrefabID>())
			{
				ShowWorkingStatus = true
			}.StartSM();
		};
	}

	public const string ID = "LiquidFilter";

	private const ConduitType CONDUIT_TYPE = ConduitType.Liquid;
}
