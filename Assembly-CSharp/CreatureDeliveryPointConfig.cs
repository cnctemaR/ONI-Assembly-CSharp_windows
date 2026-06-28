using System;
using TUNING;
using UnityEngine;

public class CreatureDeliveryPointConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("CreatureDeliveryPoint", 1, 2, "creaturetrap_dropoff_kanim", 400f, 10, 10f, new float[] { 1f }, MATERIALS.PLASTICS, 1600f, BuildLocationRule.OnFloor, BUILDINGS.DECOR.PENALTY.TIER2, NOISE_POLLUTION.NOISY.TIER0);
		buildingDef.AudioCategory = "Metal";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go)
	{
		Storage storage = go.AddOrGet<Storage>();
		storage.disableOnStore = true;
		storage.allowItemRemoval = false;
		storage.showDescriptor = true;
		storage.storageFilters = STORAGEFILTERS.BAGABLE_CREATURES;
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
