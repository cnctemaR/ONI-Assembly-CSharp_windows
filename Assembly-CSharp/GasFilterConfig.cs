using System;
using TUNING;
using UnityEngine;

public class GasFilterConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "GasFilter";
		int num = 3;
		int num2 = 1;
		string text2 = "filter_gas_kanim";
		int num3 = 30;
		float num4 = 10f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER1;
		string[] raw_METALS = MATERIALS.RAW_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.Anywhere;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER1;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, raw_METALS, num5, buildLocationRule, BUILDINGS.DECOR.PENALTY.TIER0, tier2, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.SelfHeatKilowattsWhenActive = 0f;
		buildingDef.ExhaustKilowattsWhenActive = 0f;
		buildingDef.InputConduitType = ConduitType.Gas;
		buildingDef.OutputConduitType = ConduitType.Gas;
		buildingDef.Floodable = false;
		buildingDef.ViewMode = SimViewMode.GasVentMap;
		buildingDef.MaterialCategory = MATERIALS.RAW_METALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(1, 0);
		buildingDef.PermittedRotations = PermittedRotations.R360;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddPrefabTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<Structure>();
		ElementFilter elementFilter = go.AddOrGet<ElementFilter>();
		elementFilter.conduitType = ConduitType.Gas;
		elementFilter.filterOffset = new CellOffset(1, 0);
		go.AddOrGet<GasFilterable>();
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

	public const string ID = "GasFilter";

	private const ConduitType CONDUIT_TYPE = ConduitType.Gas;
}
