using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class UnderwaterVentDrillConfig : IBuildingConfig
{
	public override string[] GetRequiredDlcIds()
	{
		return DlcManager.DLC5;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("UnderwaterVentDrill", 4, 4, "underwater_vent_drill_kanim", 100, 120f, new float[]
		{
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER4[0],
			1f
		}, new string[] { "RefinedMetal", "BuildingGasket" }, 1600f, BuildLocationRule.BuildingAttachPoint, DECOR.PENALTY.TIER1, NOISE_POLLUTION.NOISY.TIER2, 0.2f);
		buildingDef.AttachmentSlotTag = GameTags.UnderwaterVentDrill;
		buildingDef.BuildLocationRule = BuildLocationRule.BuildingAttachPoint;
		buildingDef.ObjectLayer = ObjectLayer.AttachableBuilding;
		buildingDef.SceneLayer = Grid.SceneLayer.Building;
		buildingDef.ForegroundLayer = Grid.SceneLayer.BuildingUse;
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
		buildingDef.RequiresPowerInput = true;
		buildingDef.PowerInputOffset = new CellOffset(0, 3);
		buildingDef.EnergyConsumptionWhenActive = 960f;
		buildingDef.SelfHeatKilowattsWhenActive = 2f;
		buildingDef.Floodable = false;
		buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(1, 3));
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "small";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
		Tag tag = SimHashes.Diamond.CreateTag();
		Storage storage = go.AddOrGet<Storage>();
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		storage.storageFilters = new List<Tag> { tag };
		storage.allowItemRemoval = false;
		storage.showInUI = true;
		storage.capacityKg = 200f;
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.RequestedItemTag = tag;
		manualDeliveryKG.capacity = 200f;
		manualDeliveryKG.refillMass = 80f;
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
		UnderwaterVentDrill.Def def = go.AddOrGetDef<UnderwaterVentDrill.Def>();
		def.DiamondTag = tag;
		def.DiamondConsumptionRate = 1f;
		def.WorkDuration = 100f;
		def.ProgressBarOffset = UnderwaterVentDrillConfig.PROGRESS_BAR_OFFSET;
		go.AddOrGet<LoopingSounds>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}

	public const string ID = "UnderwaterVentDrill";

	public const float DRILL_DURATION = 100f;

	public const float DIAMOND_USAGE_PER_DRILL = 100f;

	public const float DIAMOND_STORAGE_CAPACITY = 200f;

	public const float DIAMOND_CONSUMPTION_RATE = 1f;

	public static readonly Vector3 PROGRESS_BAR_OFFSET = new Vector3(0f, 1.75f, 0f);
}
