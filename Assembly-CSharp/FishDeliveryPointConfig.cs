using System;
using TUNING;
using UnityEngine;

public class FishDeliveryPointConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("FishDeliveryPoint", 1, 3, "fishrelocator_kanim", 10, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, MATERIALS.RAW_METALS, 1600f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.PENALTY.TIER2, NOISE_POLLUTION.NOISY.TIER0, 0.2f);
		buildingDef.AudioCategory = "Metal";
		buildingDef.Entombable = true;
		buildingDef.Floodable = true;
		buildingDef.ForegroundLayer = Grid.SceneLayer.TileFront;
		buildingDef.ViewMode = SimViewMode.Rooms;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.CreatureRelocator);
		Storage storage = go.AddOrGet<Storage>();
		storage.allowItemRemoval = false;
		storage.showDescriptor = true;
		storage.storageFilters = STORAGEFILTERS.SWIMMING_CREATURES;
		storage.workAnims = new HashedString[]
		{
			new HashedString("place"),
			new HashedString("release")
		};
		storage.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_restrain_creature_kanim") };
		storage.synchronizeAnims = false;
		storage.useGunForDelivery = false;
		storage.allowSettingOnlyFetchMarkedItems = false;
		CreatureDeliveryPoint creatureDeliveryPoint = go.AddOrGet<CreatureDeliveryPoint>();
		creatureDeliveryPoint.deliveryOffsets = new CellOffset[]
		{
			new CellOffset(0, 1)
		};
		creatureDeliveryPoint.spawnOffset = new CellOffset(0, -1);
		go.AddOrGet<TreeFilterable>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
		go.AddOrGetDef<MakeBaseSolid.Def>();
	}

	public const string ID = "FishDeliveryPoint";
}
