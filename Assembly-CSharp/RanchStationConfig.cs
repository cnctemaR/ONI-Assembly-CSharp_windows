using System;
using Klei.AI;
using TUNING;
using UnityEngine;

public class RanchStationConfig : IBuildingConfig
{
	public override BuildingDef CreateBuildingDef()
	{
		string text = "RanchStation";
		int num = 2;
		int num2 = 3;
		string text2 = "rancherstation_kanim";
		int num3 = 30;
		float num4 = 30f;
		float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] all_METALS = MATERIALS.ALL_METALS;
		float num5 = 1600f;
		BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
		EffectorValues tier2 = NOISE_POLLUTION.NOISY.TIER1;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(text, num, num2, text2, num3, num4, tier, all_METALS, num5, buildLocationRule, BUILDINGS.DECOR.NONE, tier2, 0.2f);
		buildingDef.ViewMode = OverlayModes.Rooms.ID;
		buildingDef.Overheatable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "large";
		buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.RanchStation, false);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.AddOrGet<LogicOperationalController>();
		RanchStation.Def def = go.AddOrGetDef<RanchStation.Def>();
		def.isCreatureEligibleToBeRanchedCb = (GameObject creature_go, RanchStation.Instance ranch_station_smi) => !creature_go.GetComponent<Effects>().HasEffect("Ranched");
		def.onRanchCompleteCb = delegate(GameObject creature_go)
		{
			RanchStation.Instance targetRanchStation = creature_go.GetSMI<RanchableMonitor.Instance>().targetRanchStation;
			RancherChore.RancherChoreStates.Instance smi2 = targetRanchStation.GetSMI<RancherChore.RancherChoreStates.Instance>();
			GameObject gameObject = targetRanchStation.GetSMI<RancherChore.RancherChoreStates.Instance>().sm.rancher.Get(smi2);
			float num = 1f + gameObject.GetAttributes().Get(Db.Get().Attributes.Ranching.Id).GetTotalValue() * 0.1f;
			creature_go.GetComponent<Effects>().Add("Ranched", true).timeRemaining *= num;
		};
		def.ranchedPreAnim = "grooming_pre";
		def.ranchedLoopAnim = "grooming_loop";
		def.ranchedPstAnim = "grooming_pst";
		def.worktime = 12f;
		def.getTargetRanchCell = delegate(RanchStation.Instance smi)
		{
			int num2 = Grid.InvalidCell;
			if (!smi.IsNullOrStopped())
			{
				num2 = Grid.CellRight(Grid.PosToCell(smi.transform.GetPosition()));
				if (!smi.targetRanchable.IsNullOrStopped() && smi.targetRanchable.HasTag(GameTags.Creatures.Flyer))
				{
					num2 = Grid.CellAbove(num2);
				}
			}
			return num2;
		};
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.CreaturePen.Id;
		roomTracker.requirement = RoomTracker.Requirement.Required;
		go.AddOrGet<SkillPerkMissingComplainer>().requiredSkillPerk = Db.Get().SkillPerks.CanWrangleCreatures.Id;
		Prioritizable.AddRef(go);
	}

	public const string ID = "RanchStation";
}
