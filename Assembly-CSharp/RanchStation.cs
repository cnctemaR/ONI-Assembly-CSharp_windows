using System;
using UnityEngine;

public class RanchStation : GameStateMachine<RanchStation, RanchStation.Instance, IStateMachineTarget, RanchStation.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.operational;
		this.unoperational.TagTransition(GameTags.Operational, this.operational, false);
		this.operational.TagTransition(GameTags.Operational, this.unoperational, true).ToggleChore((RanchStation.Instance smi) => smi.CreateChore(), this.unoperational, this.unoperational).Update("FindRanachable", delegate(RanchStation.Instance smi, float dt)
		{
			smi.FindRanchable();
		}, UpdateRate.SIM_1000ms, false);
	}

	public GameStateMachine<RanchStation, RanchStation.Instance, IStateMachineTarget, RanchStation.Def>.State unoperational;

	public RanchStation.OperationalState operational;

	public class Def : StateMachine.BaseDef
	{
		public Func<GameObject, RanchStation.Instance, bool> isCreatureEligibleToBeRanchedCb;

		public Action<GameObject> onRanchCompleteCb;

		public HashedString ranchedPreAnim = "idle_loop";

		public HashedString ranchedLoopAnim = "idle_loop";

		public HashedString ranchedPstAnim = "idle_loop";

		public HashedString rancherInteractAnim = "anim_interacts_rancherstation_kanim";

		public int interactLoopCount = 1;

		public bool synchronizeBuilding;

		public Func<RanchStation.Instance, int> getTargetRanchCell = (RanchStation.Instance smi) => Grid.PosToCell(smi);
	}

	public class OperationalState : GameStateMachine<RanchStation, RanchStation.Instance, IStateMachineTarget, RanchStation.Def>.State
	{
	}

	public new class Instance : GameStateMachine<RanchStation, RanchStation.Instance, IStateMachineTarget, RanchStation.Def>.GameInstance
	{
		public RanchableMonitor.Instance targetRanchable { get; private set; }

		public bool shouldCreatureGoGetRanched { get; private set; }

		public Instance(IStateMachineTarget master, RanchStation.Def def)
			: base(master, def)
		{
		}

		public Chore CreateChore()
		{
			return new RancherChore(base.GetComponent<KPrefabID>());
		}

		public int GetTargetRanchCell()
		{
			return base.def.getTargetRanchCell(this);
		}

		public bool IsCreatureAvailableForRanching()
		{
			if (this.targetRanchable != null)
			{
				int targetRanchCell = this.GetTargetRanchCell();
				CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(targetRanchCell);
				return RanchStation.Instance.CanRanchableBeRanchedAtRanchStation(this.targetRanchable, this, cavityForCell, targetRanchCell);
			}
			return false;
		}

		public void SetRancherIsAvailableForRanching()
		{
			this.shouldCreatureGoGetRanched = true;
		}

		public void ClearRancherIsAvailableForRanching()
		{
			this.shouldCreatureGoGetRanched = false;
		}

		private static bool CanRanchableBeRanchedAtRanchStation(RanchableMonitor.Instance ranchable, RanchStation.Instance ranch_station, CavityInfo ranch_cavity_info, int ranch_cell)
		{
			if (!ranchable.IsRunning())
			{
				return false;
			}
			if (ranchable.targetRanchStation != ranch_station && ranchable.targetRanchStation != null && ranchable.targetRanchStation.IsRunning())
			{
				return false;
			}
			if (!ranch_station.def.isCreatureEligibleToBeRanchedCb(ranchable.gameObject, ranch_station))
			{
				return false;
			}
			if (!ranchable.GetComponent<ChoreConsumer>().IsChoreEqualOrAboveCurrentChorePriority<RanchedStates>())
			{
				return false;
			}
			int num = Grid.PosToCell(ranchable.transform.GetPosition());
			CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(num);
			return cavityForCell != null && cavityForCell == ranch_cavity_info && ranchable.GetComponent<Navigator>().GetNavigationCost(ranch_cell) != -1;
		}

		public void FindRanchable()
		{
			int targetRanchCell = this.GetTargetRanchCell();
			CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(targetRanchCell);
			if (cavityForCell == null || cavityForCell.room == null || cavityForCell.room.roomType != Db.Get().RoomTypes.CreaturePen)
			{
				this.TriggerRanchStationNoLongerAvailable();
				return;
			}
			if (this.targetRanchable != null && !RanchStation.Instance.CanRanchableBeRanchedAtRanchStation(this.targetRanchable, this, cavityForCell, targetRanchCell))
			{
				this.TriggerRanchStationNoLongerAvailable();
			}
			if (this.targetRanchable.IsNullOrStopped())
			{
				CavityInfo cavityForCell2 = Game.Instance.roomProber.GetCavityForCell(targetRanchCell);
				RanchableMonitor.Instance instance = null;
				if (cavityForCell2 != null && cavityForCell2.creatures != null)
				{
					foreach (KPrefabID kprefabID in cavityForCell2.creatures)
					{
						if (!(kprefabID == null))
						{
							RanchableMonitor.Instance smi = kprefabID.GetSMI<RanchableMonitor.Instance>();
							if (!smi.IsNullOrStopped() && RanchStation.Instance.CanRanchableBeRanchedAtRanchStation(smi, this, cavityForCell2, targetRanchCell))
							{
								instance = smi;
								break;
							}
						}
					}
				}
				this.targetRanchable = instance;
				if (!this.targetRanchable.IsNullOrStopped())
				{
					this.targetRanchable.targetRanchStation = this;
				}
			}
		}

		public void TriggerRanchStationNoLongerAvailable()
		{
			if (!this.targetRanchable.IsNullOrStopped())
			{
				this.targetRanchable.targetRanchStation = null;
				this.targetRanchable.Trigger(1689625967, null);
				this.targetRanchable = null;
			}
		}

		public void RanchCreature()
		{
			if (!this.targetRanchable.IsNullOrStopped())
			{
				global::Debug.Assert(this.targetRanchable != null, "targetRanchable was null");
				global::Debug.Assert(this.targetRanchable.GetMaster() != null, "GetMaster was null");
				global::Debug.Assert(base.def != null, "def was null");
				global::Debug.Assert(base.def.onRanchCompleteCb != null, "onRanchCompleteCb cb was null");
				base.def.onRanchCompleteCb(this.targetRanchable.gameObject);
				this.targetRanchable.Trigger(1827504087, null);
			}
		}
	}
}
