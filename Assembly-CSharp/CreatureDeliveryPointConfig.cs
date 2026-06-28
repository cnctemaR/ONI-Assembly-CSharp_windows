using System;
using TUNING;
using UnityEngine;

public class CreatureDeliveryPointConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("CreatureDeliveryPoint", 1, 3, "creaturetrap_dropoff_kanim", 400f, 10, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, MATERIALS.PLASTICS, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER2, NOISE_POLLUTION.NOISY.TIER0);
		buildingDef.AudioCategory = "Metal";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		Storage storage = go.AddOrGet<Storage>();
		storage.allowItemRemoval = false;
		storage.showDescriptor = true;
		storage.storageFilters = STORAGEFILTERS.BAGABLE_CREATURES;
		storage.workAnims = new HashedString[]
		{
			new HashedString("place"),
			new HashedString("release")
		};
		storage.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_restrain_creature_kanim") };
		storage.synchronizeAnims = false;
		storage.useGunForDelivery = false;
		CreatureDeliveryPoint creatureDeliveryPoint = go.UpdateComponentRequirement<CreatureDeliveryPoint>(true);
		creatureDeliveryPoint.noFilterTint = new Color(0.5019608f, 0.5019608f, 0.5019608f, 1f);
		creatureDeliveryPoint.filterTint = new Color(1f, 1f, 1f, 1f);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BuildingTemplates.DoPostConfigure(go);
	}

	public const string ID = "CreatureDeliveryPoint";
}
