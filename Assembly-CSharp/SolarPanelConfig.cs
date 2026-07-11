using System;
using TUNING;
using UnityEngine;

public class SolarPanelConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "SolarPanel";
		int num = 7;
		int num2 = 3;
		string text2 = "solar_panel_kanim";
		int num3 = 100;
		float num4 = 120f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] glasses = MATERIALS.GLASSES;
		float num5 = 2400f;
		BuildLocationRule buildLocationRule = BuildLocationRule.Anywhere;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER5;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, glasses, num5, buildLocationRule, BUILDINGS.DECOR.PENALTY.TIER2, tier2, 0.2f);
		buildingDef.GeneratorWattageRating = 380f;
		buildingDef.GeneratorBaseCapacity = buildingDef.GeneratorWattageRating;
		buildingDef.ExhaustKilowattsWhenActive = 0f;
		buildingDef.SelfHeatKilowattsWhenActive = 0f;
		buildingDef.BuildLocationRule = BuildLocationRule.Anywhere;
		buildingDef.HitPoints = 10;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<LoopingSounds>();
		Prioritizable.AddRef(go);
		Tinkerable.MakePowerTinkerable(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		Repairable repairable = go.AddOrGet<Repairable>();
		repairable.expectedRepairTime = 52.5f;
		SolarPanel solarPanel = go.AddOrGet<SolarPanel>();
		solarPanel.powerDistributionOrder = 9;
		go.GetComponent<KPrefabID>().prefabInitFn += delegate(GameObject game_object)
		{
			PoweredActiveController.Instance instance = new PoweredActiveController.Instance(game_object.GetComponent<KPrefabID>());
			instance.StartSM();
		};
	}

	public const string ID = "SolarPanel";

	public const float WATTS_PER_LUX = 0.00053f;

	public const float MAX_WATTS = 380f;
}
