using System;
using TUNING;
using UnityEngine;

public class ShearingStationConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "ShearingStation";
		int num = 3;
		int num2 = 3;
		string text2 = "shearing_station_kanim";
		int num3 = 100;
		float num4 = 10f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] raw_MINERALS = MATERIALS.RAW_MINERALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.Tile;
		EffectorValues none = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, raw_MINERALS, num5, buildLocationRule, BUILDINGS.DECOR.NONE, none, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 60f;
		buildingDef.ExhaustKilowattsWhenActive = 0.125f;
		buildingDef.SelfHeatKilowattsWhenActive = 0.5f;
		buildingDef.Floodable = true;
		buildingDef.Entombable = true;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "large";
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
		buildingDef.DefaultAnimState = "on";
		buildingDef.ShowInBuildMenu = true;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		go.GetComponent<KPrefabID>().AddPrefabTag(RoomConstraints.ConstraintTags.RanchStation);
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.CreaturePen.Id;
		roomTracker.requirement = RoomTracker.Requirement.Required;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		RanchStation.Def def = go.AddOrGetDef<RanchStation.Def>();
		def.isCreatureEligibleToBeRanchedCb = delegate(GameObject creature_go, RanchStation.Instance ranch_station_smi)
		{
			ScaleGrowthMonitor.Instance smi = creature_go.GetSMI<ScaleGrowthMonitor.Instance>();
			return smi != null && smi.IsFullyGrown();
		};
		def.onRanchCompleteCb = delegate(GameObject creature_go)
		{
			creature_go.GetSMI<ScaleGrowthMonitor.Instance>().Shear();
		};
		def.interactLoopCount = 6;
		def.rancherInteractAnim = "anim_interacts_shearingstation_kanim";
		def.synchronizeBuilding = true;
		Prioritizable.AddRef(go);
	}

	public const string ID = "ShearingStation";
}
