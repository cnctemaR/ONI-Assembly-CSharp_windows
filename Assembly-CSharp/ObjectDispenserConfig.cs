using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class ObjectDispenserConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "ObjectDispenser";
		int num = 1;
		int num2 = 2;
		string text2 = "object_dispenser_kanim";
		int num3 = 30;
		float num4 = 10f;
		float[] tier = global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, all_METALS, num5, buildLocationRule, global::TUNING.BUILDINGS.DECOR.PENALTY.TIER1, none, 0.2f);
		buildingDef.Floodable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.Overheatable = false;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.RequiresPowerInput = true;
		buildingDef.PowerInputOffset = new CellOffset(0, 0);
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		buildingDef.EnergyConsumptionWhenActive = 60f;
		buildingDef.ExhaustKilowattsWhenActive = 0.125f;
		buildingDef.LogicInputPorts = new List<LogicPorts.Port> { LogicPorts.Port.InputPort(ObjectDispenser.PORT_ID, new CellOffset(0, 1), global::STRINGS.BUILDINGS.PREFABS.OBJECTDISPENSER.LOGIC_PORT, global::STRINGS.BUILDINGS.PREFABS.OBJECTDISPENSER.LOGIC_PORT_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.OBJECTDISPENSER.LOGIC_PORT_INACTIVE, false, false) };
		SoundEventVolumeCache.instance.AddVolume("ventliquid_kanim", "LiquidVent_squirt", NOISE_POLLUTION.NOISY.TIER0);
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<ObjectDispenser>().dropOffset = new CellOffset(1, 0);
		Prioritizable.AddRef(go);
		Storage storage = go.AddOrGet<Storage>();
		storage.showInUI = true;
		storage.allowItemRemoval = false;
		storage.showDescriptor = true;
		storage.storageFilters = STORAGEFILTERS.NOT_EDIBLE_SOLIDS;
		storage.storageFullMargin = STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
		storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
		go.AddOrGet<CopyBuildingSettings>().copyGroupTag = GameTags.StorageLocker;
		global::UnityEngine.Object.DestroyImmediate(go.GetComponent<LogicOperationalController>());
	}

	public const string ID = "ObjectDispenser";
}
