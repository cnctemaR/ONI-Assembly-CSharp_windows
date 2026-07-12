using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class RailGunConfig : IBuildingConfig
{
	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		string text = "RailGun";
		int num = 5;
		int num2 = 6;
		string text2 = "rail_gun_kanim";
		int num3 = 250;
		float num4 = 30f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER5;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, all_METALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.NONE, tier2, 0.2f);
		buildingDef.Floodable = false;
		buildingDef.Overheatable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.BaseTimeUntilRepair = 400f;
		buildingDef.DefaultAnimState = "off";
		buildingDef.RequiresPowerInput = true;
		buildingDef.PowerInputOffset = new CellOffset(-2, 0);
		buildingDef.EnergyConsumptionWhenActive = 240f;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.ExhaustKilowattsWhenActive = 0.5f;
		buildingDef.SelfHeatKilowattsWhenActive = 2f;
		buildingDef.UseHighEnergyParticleInputPort = true;
		buildingDef.HighEnergyParticleInputOffset = new CellOffset(-2, 1);
		buildingDef.LogicInputPorts = new List<LogicPorts.Port> { LogicPorts.Port.InputPort(RailGun.PORT_ID, new CellOffset(-2, 2), global::STRINGS.BUILDINGS.PREFABS.RAILGUN.LOGIC_PORT, global::STRINGS.BUILDINGS.PREFABS.RAILGUN.LOGIC_PORT_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.RAILGUN.LOGIC_PORT_INACTIVE, false, false) };
		return buildingDef;
	}

	private void AttachPorts(GameObject go)
	{
		go.AddComponent<ConduitSecondaryInput>().portInfo = this.liquidInputPort;
		go.AddComponent<ConduitSecondaryInput>().portInfo = this.gasInputPort;
		go.AddComponent<ConduitSecondaryInput>().portInfo = this.solidInputPort;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		RailGun railGun = go.AddOrGet<RailGun>();
		railGun.FreeStartHex = true;
		railGun.FreeDestinationHex = true;
		go.AddOrGet<LoopingSounds>();
		ClusterDestinationSelector clusterDestinationSelector = go.AddOrGet<ClusterDestinationSelector>();
		clusterDestinationSelector.assignable = true;
		clusterDestinationSelector.requireAsteroidDestination = true;
		railGun.liquidPortInfo = this.liquidInputPort;
		railGun.gasPortInfo = this.gasInputPort;
		railGun.solidPortInfo = this.solidInputPort;
		HighEnergyParticleStorage highEnergyParticleStorage = go.AddOrGet<HighEnergyParticleStorage>();
		highEnergyParticleStorage.capacity = 210f;
		highEnergyParticleStorage.autoStore = true;
		highEnergyParticleStorage.showInUI = false;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		List<Tag> list = new List<Tag>();
		list.AddRange(STORAGEFILTERS.NOT_EDIBLE_SOLIDS);
		list.AddRange(STORAGEFILTERS.GASES);
		list.AddRange(STORAGEFILTERS.FOOD);
		Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
		storage.showInUI = true;
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		storage.storageFilters = list;
		storage.allowSettingOnlyFetchMarkedItems = false;
		storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
		storage.capacityKg = 1200f;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		base.DoPostConfigurePreview(def, go);
		this.AttachPorts(go);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		this.AttachPorts(go);
	}

	public const string ID = "RailGun";

	public const int RANGE = 20;

	public const float BASE_PARTICLE_COST = 10f;

	public const float HEX_PARTICLE_COST = 10f;

	public const float HEP_CAPACITY = 210f;

	public const float TAKEOFF_VELOCITY = 35f;

	public const int MAINTENANCE_AFTER_NUM_PAYLOADS = 6;

	public const int MAINTENANCE_COOLDOWN = 30;

	public const float CAPACITY = 1200f;

	private ConduitPortInfo solidInputPort = new ConduitPortInfo(ConduitType.Solid, new CellOffset(-1, 0));

	private ConduitPortInfo liquidInputPort = new ConduitPortInfo(ConduitType.Liquid, new CellOffset(0, 0));

	private ConduitPortInfo gasInputPort = new ConduitPortInfo(ConduitType.Gas, new CellOffset(1, 0));
}
