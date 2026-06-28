using System;
using TUNING;
using UnityEngine;

public class ManualGeneratorConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		EffectorValues tier = NOISE_POLLUTION.NOISY.TIER3;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("ManualGenerator", 2, 2, "generatormanual_kanim", 200f, 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.NONE, tier);
		buildingDef.GeneratorWattageRating = 400f;
		buildingDef.GeneratorBaseCapacity = 10000f;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.MaterialCategory = MATERIALS.ALL_METALS;
		buildingDef.AudioCategory = "Metal";
		buildingDef.Upgradeable = true;
		buildingDef.Breakable = true;
		buildingDef.ForegroundLayer = Grid.SceneLayer.BuildingFront;
		buildingDef.OperatingKilowatts = 1f;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		go.GetComponent<KPrefabID>().AddPrefabTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		go.AddOrGet<LoopingSounds>();
		Prioritizable.AddRef(go);
		Generator generator = go.AddOrGet<Generator>();
		generator.powerDistributionOrder = 10;
		ManualGenerator manualGenerator = go.AddOrGet<ManualGenerator>();
		manualGenerator.SingleSliderPercent = 0.5f;
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
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}

	public const string ID = "ManualGenerator";
}
