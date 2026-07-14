using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class ReefGeneratorConfig : IBuildingConfig
{
	public override string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC5;
	}

	public override BuildingDef CreateBuildingDef()
	{
		string text = "ReefGenerator";
		int num = 3;
		int num2 = 3;
		string text2 = "turbine_reef_kanim";
		int num3 = 100;
		float num4 = 60f;
		float[] array = new float[]
		{
			global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5[0],
			1f
		};
		string[] array2 = new string[] { "Metal", "BuildingGasket" };
		float num5 = 2400f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier = NOISE_POLLUTION.NOISY.TIER5;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, array, array2, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.PENALTY.TIER2, tier, 0.2f);
		buildingDef.SceneLayer = Grid.SceneLayer.BuildingFront;
		buildingDef.ForegroundLayer = Grid.SceneLayer.BuildingBack;
		buildingDef.GeneratorWattageRating = 300f;
		buildingDef.GeneratorBaseCapacity = buildingDef.GeneratorWattageRating;
		buildingDef.ExhaustKilowattsWhenActive = 0.125f;
		buildingDef.SelfHeatKilowattsWhenActive = 0.5f;
		buildingDef.RequiresPowerOutput = true;
		buildingDef.PowerOutputOffset = new CellOffset(0, 0);
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.AudioSize = "large";
		buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(-1, 2));
		buildingDef.AddSearchTerms(SEARCH_TERMS.POWER);
		buildingDef.AddSearchTerms(SEARCH_TERMS.GENERATOR);
		buildingDef.AttachmentSlotTag = GameTags.ReefGenerator;
		buildingDef.BuildLocationRule = BuildLocationRule.BuildingAttachPoint;
		buildingDef.ObjectLayer = ObjectLayer.AttachableBuilding;
		buildingDef.PowerOutputOffset = new CellOffset(1, 2);
		buildingDef.Floodable = false;
		buildingDef.POIUnlockable = true;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		KPrefabID component = go.GetComponent<KPrefabID>();
		component.AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
		component.AddTag(RoomConstraints.ConstraintTags.PowerBuilding, false);
		component.AddTag(RoomConstraints.ConstraintTags.GeneratorType, false);
		component.AddTag(RoomConstraints.ConstraintTags.HeavyDutyGeneratorType, false);
		go.AddOrGet<ReefGeneratorPower>().powerDistributionOrder = 9;
		go.AddOrGet<LoopingSounds>();
		Prioritizable.AddRef(go);
		go.AddOrGetDef<ReefGenerator.Def>();
		Tinkerable.MakePowerTinkerable(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicOperationalController>();
	}

	public const string ID = "ReefGenerator";

	private const float WATTS_PRODUCED = 300f;

	private const int GASKET_UNITS = 1;
}
