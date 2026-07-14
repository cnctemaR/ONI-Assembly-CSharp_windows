using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class FishDeliveryPointConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("FishDeliveryPoint", 1, 3, "fishrelocator_kanim", 10, 10f, global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, MATERIALS.RAW_METALS, 1600f, BuildLocationRule.Anywhere, global::TUNING.BUILDINGS.DECOR.PENALTY.TIER2, NOISE_POLLUTION.NOISY.TIER0, 0.2f);
		buildingDef.AudioCategory = "Metal";
		buildingDef.Entombable = true;
		buildingDef.Floodable = false;
		buildingDef.ForegroundLayer = Grid.SceneLayer.TileMain;
		buildingDef.ViewMode = OverlayModes.Rooms.ID;
		buildingDef.LogicInputPorts = new List<LogicPorts.Port> { LogicPorts.Port.InputPort("CritterDropOffInput", new CellOffset(0, 0), global::STRINGS.BUILDINGS.PREFABS.FISHDELIVERYPOINT.LOGIC_INPUT.DESC, global::STRINGS.BUILDINGS.PREFABS.FISHDELIVERYPOINT.LOGIC_INPUT.LOGIC_PORT_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.FISHDELIVERYPOINT.LOGIC_INPUT.LOGIC_PORT_INACTIVE, false, false) };
		buildingDef.AddSearchTerms(SEARCH_TERMS.RANCHING);
		buildingDef.AddSearchTerms(SEARCH_TERMS.CRITTER);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		KPrefabID component = go.GetComponent<KPrefabID>();
		component.AddTag(GameTags.CodexCategories.CreatureRelocator, false);
		Storage storage = go.AddOrGet<Storage>();
		storage.allowItemRemoval = false;
		storage.showDescriptor = true;
		storage.storageFilters = STORAGEFILTERS.SWIMMING_CREATURES;
		storage.workAnims = new HashedString[]
		{
			new HashedString("working_pre")
		};
		storage.workAnimPlayMode = KAnim.PlayMode.Once;
		storage.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_interacts_fishrelocator_kanim") };
		storage.synchronizeAnims = false;
		storage.useGunForDelivery = false;
		storage.allowSettingOnlyFetchMarkedItems = false;
		storage.faceTargetWhenWorking = false;
		CreatureDeliveryPoint creatureDeliveryPoint = go.AddOrGet<CreatureDeliveryPoint>();
		creatureDeliveryPoint.deliveryOffsets = new CellOffset[]
		{
			new CellOffset(0, 1)
		};
		creatureDeliveryPoint.spawnOffset = new CellOffset(0, -1);
		creatureDeliveryPoint.largeCritterSpawnOffset = new CellOffset(0, -2);
		creatureDeliveryPoint.playAnimsOnFetch = true;
		BaggableCritterCapacityTracker baggableCritterCapacityTracker = go.AddOrGet<BaggableCritterCapacityTracker>();
		baggableCritterCapacityTracker.maximumCreatures = 20;
		baggableCritterCapacityTracker.cavityOffset = CellOffset.down;
		baggableCritterCapacityTracker.requireLiquidOffset = true;
		go.AddOrGet<TreeFilterable>();
		BuildingPointStraw buildingPointStraw = go.AddOrGet<BuildingPointStraw>();
		buildingPointStraw.canControlAnimStates = false;
		buildingPointStraw.usesSymbols = false;
		buildingPointStraw.maxDepth = 4;
		component.prefabInitFn += this.OnPrefabInit;
	}

	private void OnPrefabInit(GameObject instance)
	{
		foreach (KBatchedAnimController kbatchedAnimController in instance.GetComponentsInChildrenOnly<KBatchedAnimController>())
		{
			kbatchedAnimController.SetBlendValue(KBatchedAnimInstanceData.BlendActiveOptions.LiquidVisibilityLayer, false);
			kbatchedAnimController.SetBlendValue(KBatchedAnimInstanceData.BlendActiveOptions.WaterProof, true);
		}
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGetDef<MakeBaseSolid.Def>().solidOffsets = new CellOffset[]
		{
			new CellOffset(0, 0)
		};
	}

	public const string ID = "FishDeliveryPoint";

	public const int STRAW_LENGTH = 4;

	public const string INPUT_PORT = "FishDeliveryPointInput";
}
