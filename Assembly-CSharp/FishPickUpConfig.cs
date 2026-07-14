using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class FishPickUpConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("FishPickUp", 1, 3, "fishrelocator2_kanim", 10, 10f, global::TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, MATERIALS.RAW_METALS, 1600f, BuildLocationRule.Anywhere, global::TUNING.BUILDINGS.DECOR.PENALTY.TIER2, NOISE_POLLUTION.NOISY.TIER0, 0.2f);
		buildingDef.AudioCategory = "Metal";
		buildingDef.Entombable = true;
		buildingDef.Floodable = false;
		buildingDef.ForegroundLayer = Grid.SceneLayer.TileMain;
		buildingDef.ViewMode = OverlayModes.Rooms.ID;
		buildingDef.LogicInputPorts = new List<LogicPorts.Port> { LogicPorts.Port.InputPort("FishPickUpInput", new CellOffset(0, 0), global::STRINGS.BUILDINGS.PREFABS.FISHPICKUP.LOGIC_INPUT.DESC, global::STRINGS.BUILDINGS.PREFABS.FISHPICKUP.LOGIC_INPUT.LOGIC_PORT_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.FISHPICKUP.LOGIC_INPUT.LOGIC_PORT_INACTIVE, false, false) };
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
		go.AddOrGet<TreeFilterable>();
		BaggableCritterCapacityTracker baggableCritterCapacityTracker = go.AddOrGet<BaggableCritterCapacityTracker>();
		baggableCritterCapacityTracker.maximumCreatures = 20;
		baggableCritterCapacityTracker.cavityOffset = CellOffset.down;
		baggableCritterCapacityTracker.requireLiquidOffset = true;
		BuildingPointStraw buildingPointStraw = go.AddOrGet<BuildingPointStraw>();
		buildingPointStraw.canControlAnimStates = false;
		buildingPointStraw.usesSymbols = false;
		Prioritizable.AddRef(go);
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
		FixedCapturePoint.Def def = go.AddOrGetDef<FixedCapturePoint.Def>();
		def.onAnimName = "on";
		def.offAnimName = "off";
		def.isAmountStoredOverCapacity = delegate(FixedCapturePoint.Instance smi, FixedCapturableMonitor.Instance capturable)
		{
			TreeFilterable component = smi.GetComponent<TreeFilterable>();
			IUserControlledCapacity component2 = smi.GetComponent<IUserControlledCapacity>();
			float amountStored = component2.AmountStored;
			float userMaxCapacity = component2.UserMaxCapacity;
			return amountStored > userMaxCapacity && component.ContainsTag(capturable.PrefabTag);
		};
		def.allowBabies = true;
		def.captureCellOffset = new CellOffset(0, -1);
		def.rancherInteractOffset = new CellOffset(0, 1);
		def.postCaptureOffset = new CellOffset?(new CellOffset(0, 1));
		def.logicPortId = "FishPickUpInput";
		def.preCaptureAnimName = "working_pst";
		def.getPreCaptureAnimSuffix = delegate(FixedCapturePoint.Instance smi)
		{
			BuildingPointStraw component3 = smi.GetComponent<BuildingPointStraw>();
			if (!(component3 != null))
			{
				return "_1";
			}
			return component3.GetAnimSuffix();
		};
		def.getTargetCapturePoint = delegate(FixedCapturePoint.Instance smi)
		{
			int num = Grid.PosToCell(smi);
			BuildingPointStraw component4 = smi.GetComponent<BuildingPointStraw>();
			int num2 = ((component4 != null) ? component4.GetDepthOffset() : (-1));
			int num3 = Grid.OffsetCell(num, 0, num2);
			if (Grid.IsValidCell(num3) && smi.targetCapturable.Navigator.CanReach(num3))
			{
				return num3;
			}
			return num;
		};
	}

	public const string ID = "FishPickUp";

	public const string INPUT_PORT = "FishPickUpInput";
}
