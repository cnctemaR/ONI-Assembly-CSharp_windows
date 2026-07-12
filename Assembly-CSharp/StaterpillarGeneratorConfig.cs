using System;
using Klei.AI;
using TUNING;
using UnityEngine;

public class StaterpillarGeneratorConfig : IBuildingConfig
{
	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		string id = StaterpillarGeneratorConfig.ID;
		int num = 1;
		int num2 = 2;
		string text = "egg_caterpillar_kanim";
		int num3 = 1000;
		float num4 = 10f;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] array = all_METALS;
		float num5 = 9999f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFoundationRotatable;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER0;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, num, num2, text, num3, num4, tier, array, num5, buildLocationRule, BUILDINGS.DECOR.NONE, tier2, 0.2f);
		buildingDef.GeneratorWattageRating = 1600f;
		buildingDef.GeneratorBaseCapacity = 5000f;
		buildingDef.ExhaustKilowattsWhenActive = 2f;
		buildingDef.SelfHeatKilowattsWhenActive = 4f;
		buildingDef.Overheatable = false;
		buildingDef.Floodable = false;
		buildingDef.OverheatTemperature = 423.15f;
		buildingDef.PermittedRotations = PermittedRotations.FlipV;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "Plastic";
		buildingDef.RequiresPowerOutput = true;
		buildingDef.PowerOutputOffset = new CellOffset(0, 1);
		buildingDef.PlayConstructionSounds = false;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<StaterpillarGenerator>().powerDistributionOrder = 9;
		go.GetComponent<Deconstructable>().SetAllowDeconstruction(false);
		go.AddOrGet<Modifiers>();
		go.AddOrGet<Effects>();
		go.GetComponent<KSelectable>().IsSelectable = false;
	}

	public static readonly string ID = "StaterpillarGenerator";

	private const int WIDTH = 1;

	private const int HEIGHT = 2;
}
