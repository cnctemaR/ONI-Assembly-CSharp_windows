using System;
using Klei.AI;
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
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER3[0]
		};
		string[] array2 = new string[] { "RefinedMetal", "Plastic" };
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
		storage.capacityKg = MooTuning.MILK_AMOUNT_AT_MILKING * 2f;
		storage.showInUI = true;
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		Prioritizable.AddRef(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.CreaturePen.Id;
		roomTracker.requirement = RoomTracker.Requirement.Required;
		go.AddOrGet<SkillPerkMissingComplainer>().requiredSkillPerk = Db.Get().SkillPerks.CanUseMilkingStation.Id;
		RanchStation.Def ranch_station = go.AddOrGetDef<RanchStation.Def>();
		ranch_station.IsCritterEligibleToBeRanchedCb = (GameObject creature_go, RanchStation.Instance ranch_station_smi) => !(creature_go.PrefabID() != "Moo") && creature_go.GetComponent<KPrefabID>().HasTag(GameTags.Creatures.RequiresMilking);
		ranch_station.RancherInteractAnim = "anim_interacts_milking_station_kanim";
		ranch_station.RanchedPreAnim = "mooshake_pre";
		ranch_station.RanchedLoopAnim = "mooshake_loop";
		ranch_station.RanchedPstAnim = "mooshake_pst";
		ranch_station.WorkTime = 20f;
		ranch_station.CreatureRanchingStatusItem = Db.Get().CreatureStatusItems.GettingMilked;
		ranch_station.GetTargetRanchCell = delegate(RanchStation.Instance smi)
		{
			int num = Grid.InvalidCell;
			if (!smi.IsNullOrStopped())
			{
				num = Grid.PosToCell(smi.transform.GetPosition());
			}
			return num;
		};
		ranch_station.OnRanchCompleteCb = delegate(GameObject creature_go, WorkerBase rancher_wb)
		{
			RanchStation.Instance targetRanchStation = creature_go.GetSMI<RanchableMonitor.Instance>().TargetRanchStation;
			AmountInstance amountInstance = creature_go.GetAmounts().Get(Db.Get().Amounts.MilkProduction.Id);
			if (amountInstance.value > 0f)
			{
				float num2 = amountInstance.value * (MooTuning.MILK_AMOUNT_AT_MILKING / amountInstance.GetMax());
				targetRanchStation.GetComponent<Storage>().AddLiquid(SimHashes.Milk, num2, 310.15f, byte.MaxValue, 0, false, true);
				amountInstance.SetValue(0f);
			}
			creature_go.GetComponent<KPrefabID>().RemoveTag(GameTags.Creatures.RequiresMilking);
		};
		ranch_station.OnRanchWorkTick = delegate(GameObject creature_go, float dt, Workable workable)
		{
			if (creature_go.GetComponent<KAnimControllerBase>().CurrentAnim.name == ranch_station.RanchedPstAnim)
			{
				StateMachine.Instance ranchStation = creature_go.GetSMI<RanchedStates.Instance>().GetRanchStation();
				AmountInstance amountInstance2 = creature_go.GetAmounts().Get(Db.Get().Amounts.MilkProduction.Id);
				float num3 = amountInstance2.GetMax() * dt / workable.workTime;
				float num4 = num3 * (MooTuning.MILK_AMOUNT_AT_MILKING / amountInstance2.GetMax());
				float temperature = creature_go.GetComponent<PrimaryElement>().Temperature;
				ranchStation.GetComponent<Storage>().AddLiquid(SimHashes.Milk, num4, temperature, byte.MaxValue, 0, false, true);
				amountInstance2.ApplyDelta(-num3);
			}
		};
		ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
		conduitDispenser.conduitType = ConduitType.Liquid;
		conduitDispenser.alwaysDispense = true;
		conduitDispenser.elementFilter = null;
	}

	public const string ID = "MilkingStation";
}
