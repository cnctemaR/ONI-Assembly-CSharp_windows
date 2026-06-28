using System;
using STRINGS;
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
		float num3 = 200f;
		int num4 = 30;
		float num5 = 30f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num6 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER3;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, num5, tier, all_METALS, num6, buildLocationRule, global::TUNING.BUILDINGS.DECOR.NONE, tier2);
		buildingDef.GeneratorWattageRating = 400f;
		buildingDef.GeneratorBaseCapacity = 10000f;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.MaterialCategory = MATERIALS.ALL_METALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.Upgradeable = true;
		buildingDef.Breakable = true;
		buildingDef.ForegroundLayer = Grid.SceneLayer.BuildingFront;
		buildingDef.OperatingKilowatts = 1f;
		buildingDef.HotKey = global::Action.BuildMenuKeyG;
		return buildingDef;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, ManualGeneratorConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, ManualGeneratorConfig.INPUT_PORTS);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, ManualGeneratorConfig.INPUT_PORTS);
		go.AddOrGet<LogicOperationalController>();
		go.GetComponent<KPrefabID>().AddPrefabTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
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
		string prefabID = go.GetComponent<Building>().Def.PrefabID;
		Upgradable.UpgradableConfig.UpgradeModifier[] array = new Upgradable.UpgradableConfig.UpgradeModifier[]
		{
			new Upgradable.UpgradableConfig.UpgradeModifier(Upgradable.Upgrade.Target.EnergyGeneration, 1.2f)
		};
		Upgradable.AddToUpgradableConfigs(new Upgradable.UpgradableConfig(prefabID, 0, 1, 120f, "Metal", 100f, array));
		array = new Upgradable.UpgradableConfig.UpgradeModifier[]
		{
			new Upgradable.UpgradableConfig.UpgradeModifier(Upgradable.Upgrade.Target.Capacity, 2f)
		};
		Upgradable.AddToUpgradableConfigs(new Upgradable.UpgradableConfig(prefabID, 1, 1, 120f, "Metal", 200f, array));
		BuildingTemplates.DoPostConfigure(go);
	}

	public const string ID = "ManualGenerator";

	private static readonly LogicPorts.Port[] INPUT_PORTS = new LogicPorts.Port[]
	{
		new LogicPorts.Port(LogicOperationalController.PORT_ID, new CellOffset(0, 0), UI.LOGIC_PORTS.CONTROL_OPERATIONAL, false)
	};
}
