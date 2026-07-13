using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class ResearchClusterModuleConfig : IBuildingConfig
{
	public override string[] GetRequiredDlcIds()
	{
		return new string[] { "EXPANSION1_ID" };
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("ResearchClusterModule", 3, 2, "rocket_research_module_small_kanim", 1000, 60f, global::TUNING.BUILDINGS.ROCKETRY_MASS_KG.DENSE_TIER2, new string[] { SimHashes.Steel.ToString() }, 9999f, BuildLocationRule.Anywhere, DECOR.NONE, NOISE_POLLUTION.NONE, 0.2f);
		buildingDef.InputConduitType = ConduitType.None;
		buildingDef.OutputConduitType = ConduitType.None;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
		buildingDef.RequiresPowerInput = false;
		buildingDef.RequiresPowerOutput = false;
		buildingDef.PowerInputOffset = new CellOffset(0, 0);
		buildingDef.PowerOutputOffset = new CellOffset(0, 0);
		buildingDef.UseHighEnergyParticleInputPort = false;
		buildingDef.UseHighEnergyParticleOutputPort = false;
		buildingDef.HighEnergyParticleInputOffset = new CellOffset(0, 0);
		buildingDef.HighEnergyParticleOutputOffset = new CellOffset(0, 0);
		buildingDef.PermittedRotations = PermittedRotations.Unrotatable;
		buildingDef.DragBuild = false;
		buildingDef.Replaceable = true;
		buildingDef.ExhaustKilowattsWhenActive = 0f;
		buildingDef.SelfHeatKilowattsWhenActive = 0f;
		buildingDef.Overheatable = true;
		buildingDef.Floodable = false;
		buildingDef.Disinfectable = true;
		buildingDef.Entombable = true;
		buildingDef.Repairable = true;
		buildingDef.IsFoundation = false;
		BuildingTemplates.CreateRocketBuildingDef(buildingDef);
		buildingDef.OverheatTemperature = 2273.15f;
		buildingDef.AttachmentSlotTag = GameTags.Rocket;
		buildingDef.ObjectLayer = ObjectLayer.Building;
		buildingDef.CanMove = true;
		buildingDef.Cancellable = false;
		buildingDef.attachablePosition = new CellOffset(0, 0);
		buildingDef.ShowInBuildMenu = false;
		buildingDef.AddSearchTerms(SEARCH_TERMS.TRANSPORT);
		buildingDef.AudioCategory = "Metal";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
		go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[]
		{
			new BuildingAttachPoint.HardPoint(new CellOffset(0, 2), GameTags.Rocket, null)
		};
		go.AddOrGet<LoopingSounds>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 50f;
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		storage.showCapacityStatusItem = true;
		storage.storageFilters = STORAGEFILTERS.SO_DATABANKS;
		storage.allowSettingOnlyFetchMarkedItems = false;
		storage.allowItemRemoval = false;
		BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, null, ROCKETRY.BURDEN.MINOR_PLUS, 0f, 0f);
		RocketModuleHexCellCollector.Def def = go.AddOrGetDef<RocketModuleHexCellCollector.Def>();
		def.collectSpeed = 0.083333336f;
		def.formatCapacityBarAsUnits = true;
		go.AddOrGetDef<ResearchClusterModule.Def>();
	}

	public const string ID = "ResearchClusterModule";

	public const float CAPACITY = 50f;

	public const int CYCLE_COUNT_TO_FILL_STORAGE = 1;

	public const float COLLECT_SPEED = 0.083333336f;

	private const int WIDTH = 3;

	private const int HEIGHT = 2;
}
