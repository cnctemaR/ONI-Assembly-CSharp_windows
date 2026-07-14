using System;
using TUNING;
using UnityEngine;

public class MilkingStationConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "MilkingStation";
		int num = 2;
		int num2 = 4;
		string text2 = "milking_station_kanim";
		int num3 = 30;
		float num4 = 60f;
		float[] array = new float[]
		{
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER4[0],
			4f
		};
		string[] array2 = new string[] { "RefinedMetal", "BuildingGasket" };
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier = NOISE_POLLUTION.NOISY.TIER1;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, array, array2, num5, buildLocationRule, BUILDINGS.DECOR.PENALTY.TIER2, tier, 0.2f);
		buildingDef.ViewMode = OverlayModes.Rooms.ID;
		buildingDef.OutputConduitType = ConduitType.Liquid;
		buildingDef.UtilityOutputOffset = new CellOffset(1, 0);
		buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
		buildingDef.Overheatable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "large";
		buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
		buildingDef.OutputConduitType = ConduitType.Liquid;
		buildingDef.UtilityOutputOffset = new CellOffset(1, 1);
		buildingDef.RequiredSkillPerkID = Db.Get().SkillPerks.CanUseMilkingStation.Id;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.RanchStationType, false);
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = Mathf.Max(MooTuning.MILK_AMOUNT_AT_MILKING, MooTuning.DIESEL_PER_CYCLE) * 2f;
		storage.showInUI = true;
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicOperationalController>();
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.CreaturePen.Id;
		roomTracker.requirement = RoomTracker.Requirement.Required;
		go.AddOrGet<SkillPerkMissingComplainer>().requiredSkillPerk = Db.Get().SkillPerks.CanUseMilkingStation.Id;
		RanchStation.Def def = go.AddOrGetDef<RanchStation.Def>();
		def.IsCritterEligibleToBeRanchedCb = delegate(GameObject creature_go, RanchStation.Instance ranch_station_smi)
		{
			IMilkable smi2 = creature_go.GetSMI<IMilkable>();
			return smi2 != null && smi2.IsReadyToBeMilked();
		};
		def.RancherInteractAnim = "anim_interacts_milking_station_kanim";
		def.RanchedPreAnim = "mooshake_pre";
		def.RanchedLoopAnim = "mooshake_loop";
		def.RanchedPstAnim = "mooshake_pst";
		def.WorkTime = 20f;
		def.CreatureRanchingStatusItem = Db.Get().CreatureStatusItems.GettingMilked;
		def.RancherWipesBrowAnim = false;
		def.GetTargetRanchCell = delegate(RanchStation.Instance smi)
		{
			int num = Grid.InvalidCell;
			if (!smi.IsNullOrStopped())
			{
				num = Grid.PosToCell(smi.transform.GetPosition());
			}
			return num;
		};
		def.OnRanchCompleteCb = delegate(GameObject creature_go, WorkerBase rancher_wb)
		{
			RanchStation.Instance targetRanchStation = creature_go.GetSMI<RanchableMonitor.Instance>().TargetRanchStation;
			creature_go.GetSMI<IMilkable>().MilkingComplete(targetRanchStation.GetComponent<Storage>());
		};
		def.OnRanchWorkBegins = delegate(RanchedStates.Instance creature, Workable workable)
		{
			IMilkable smi3 = creature.gameObject.GetSMI<IMilkable>();
			if (smi3 == null)
			{
				return;
			}
			Color color = ElementLoader.FindElementByHash(smi3.GetMilkElement()).substance.colour;
			color.a = 1f;
			workable.GetComponent<KBatchedAnimController>().SetSymbolTint(new KAnimHashedString("gushfx"), color);
		};
		ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
		conduitDispenser.conduitType = ConduitType.Liquid;
		conduitDispenser.alwaysDispense = true;
		conduitDispenser.elementFilter = null;
	}

	public const string ID = "MilkingStation";
}
