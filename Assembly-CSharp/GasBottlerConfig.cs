using System;
using TUNING;
using UnityEngine;

public class GasBottlerConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("GasBottler", 2, 3, "gas_bottler_kanim", 100, 120f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 800f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER1, NOISE_POLLUTION.NOISY.TIER0, 0.2f);
		buildingDef.InputConduitType = ConduitType.Gas;
		buildingDef.Floodable = false;
		buildingDef.ViewMode = SimViewMode.GasVentMap;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.UtilityInputOffset = new CellOffset(1, 2);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
		storage.showDescriptor = true;
		storage.storageFilters = STORAGEFILTERS.GASES;
		storage.capacityKg = 10f;
		storage.allowItemRemoval = false;
		go.AddOrGet<DropAllWorkable>();
		GasBottler gasBottler = go.AddOrGet<GasBottler>();
		gasBottler.storage = storage;
		gasBottler.workTime = 9f;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.storage = storage;
		conduitConsumer.conduitType = ConduitType.Gas;
		conduitConsumer.ignoreMinMassCheck = true;
		conduitConsumer.forceAlwaysSatisfied = true;
		conduitConsumer.alwaysConsume = true;
		conduitConsumer.capacityKG = storage.capacityKg;
		conduitConsumer.keepZeroMassObject = false;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}

	public const string ID = "GasBottler";

	private const ConduitType CONDUIT_TYPE = ConduitType.Gas;

	private const int WIDTH = 2;

	private const int HEIGHT = 3;
}
