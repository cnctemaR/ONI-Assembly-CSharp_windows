using System;
using TUNING;
using UnityEngine;

public class ManualGeneratorConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "ManualGenerator";
		int num = 2;
		int num2 = 2;
		string text2 = "generatormanual_kanim";
		int num3 = 30;
		float num4 = 30f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER3;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, all_METALS, num5, buildLocationRule, BUILDINGS.DECOR.NONE, tier2, 0.2f);
		buildingDef.GeneratorWattageRating = 400f;
		buildingDef.GeneratorBaseCapacity = 10000f;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.Breakable = true;
		buildingDef.ForegroundLayer = Grid.SceneLayer.BuildingFront;
		buildingDef.SelfHeatKilowattsWhenActive = 1f;
		return buildingDef;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, LogicOperationalController.INPUT_PORTS_0_0);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, LogicOperationalController.INPUT_PORTS_0_0);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, LogicOperationalController.INPUT_PORTS_0_0);
		go.AddOrGet<LogicOperationalController>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		go.AddOrGet<LoopingSounds>();
		Prioritizable.AddRef(go);
		Generator generator = go.AddOrGet<Generator>();
		generator.powerDistributionOrder = 10;
		ManualGenerator manualGenerator = go.AddOrGet<ManualGenerator>();
		manualGenerator.SetSliderValue(50f, 0);
		manualGenerator.workLayer = Grid.SceneLayer.BuildingFront;
		KBatchedAnimController kbatchedAnimController = go.AddOrGet<KBatchedAnimController>();
		kbatchedAnimController.fgLayer = Grid.SceneLayer.BuildingFront;
		kbatchedAnimController.initialAnim = "off";
		Tinkerable.MakePowerTinkerable(go);
	}

	public const string ID = "ManualGenerator";
}
