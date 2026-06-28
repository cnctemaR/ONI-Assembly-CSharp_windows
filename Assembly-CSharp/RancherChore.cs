using System;
using STRINGS;

public class RancherChore : Chore<RancherChore.RancherChoreStates.Instance>
{
	public RancherChore(KPrefabID rancher_station)
	{
		Chore.Precondition precondition = default(Chore.Precondition);
		precondition.id = "IsCreatureAvailableForRanching";
		precondition.description = DUPLICANTS.CHORES.PRECONDITIONS.IS_CREATURE_AVAILABLE_FOR_RANCHING;
		precondition.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			RanchStation.Instance instance = data as RanchStation.Instance;
			return instance.IsCreatureAvailableForRanching();
		};
		this.IsCreatureAvailableForRanching = precondition;
		base..ctor(Db.Get().ChoreTypes.Ranch, rancher_station, null, false, null, null, null, PriorityScreen.PriorityClass.basic, 0, false, true, 0, null);
		base.AddPrecondition(this.IsCreatureAvailableForRanching, rancher_station.GetSMI<RanchStation.Instance>());
		base.AddPrecondition(ChorePreconditions.instance.HasRolePerk, RoleManager.rolePerks.CanUseRanchStation.id);
		this.smi = new RancherChore.RancherChoreStates.Instance(rancher_station);
		base.SetPrioritizable(rancher_station.GetComponent<Prioritizable>());
	}

	public override void Begin(Chore.Precondition.Context context)
	{
		this.smi.sm.rancher.Set(context.consumerState.gameObject, this.smi);
		base.Begin(context);
	}

	public Chore.Precondition IsCreatureAvailableForRanching;

	public class RancherChoreStates : GameStateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.movetoranch;
			base.Target(this.rancher);
			this.root.Exit("TriggerRanchStationNoLongerAvailable", delegate(RancherChore.RancherChoreStates.Instance smi)
			{
				smi.TriggerRanchStationNoLongerAvailable();
			});
			this.movetoranch.MoveTo((RancherChore.RancherChoreStates.Instance smi) => Grid.PosToCell(smi.transform.GetPosition()), this.waitforcreature_pre, null, false).Target(this.masterTarget).EventTransition(GameHashes.CreatureAbandonedRanchStation, this.checkformoreranchables, null);
			this.waitforcreature_pre.Enter("CheckIfCreatureIsNull", delegate(RancherChore.RancherChoreStates.Instance smi)
			{
				if (smi.ranchStation == null)
				{
					smi.GoTo(null);
				}
				else if (smi.ranchStation.targetRanchable == null)
				{
					smi.GoTo(this.checkformoreranchables);
				}
				else
				{
					smi.GoTo(this.waitforcreature);
				}
			});
			this.waitforcreature.ToggleAnims("anim_interacts_rancherstation_kanim", 0f).PlayAnim("calling_loop", KAnim.PlayMode.Loop).Enter("FaceCreature", delegate(RancherChore.RancherChoreStates.Instance smi)
			{
				this.rancher.Get<Facing>(smi).Face(smi.ranchStation.targetRanchable.transform.GetPosition());
			})
				.Enter("TellCreatureToGoGetRanched", delegate(RancherChore.RancherChoreStates.Instance smi)
				{
					smi.ranchStation.SetRancherIsAvailableForRanching();
				})
				.Exit("ClearRancherIsAvailableForRanching", delegate(RancherChore.RancherChoreStates.Instance smi)
				{
					smi.ranchStation.ClearRancherIsAvailableForRanching();
				})
				.Target(this.masterTarget)
				.EventTransition(GameHashes.CreatureArrivedAtRanchStation, this.ranchcreature, null)
				.EventTransition(GameHashes.CreatureAbandonedRanchStation, this.checkformoreranchables, null);
			this.ranchcreature.ToggleAnims("anim_interacts_rancherstation_kanim", 0f).ToggleAnims("anim_disappointed_kanim", 0f).DefaultState(this.ranchcreature.pre)
				.EventTransition(GameHashes.CreatureAbandonedRanchStation, this.checkformoreranchables, null);
			this.ranchcreature.pre.Enter("FaceCreature", delegate(RancherChore.RancherChoreStates.Instance smi)
			{
				this.rancher.Get<Facing>(smi).Face(smi.ranchStation.targetRanchable.transform.GetPosition());
			}).QueueAnim("working_pre", false, null).OnAnimQueueComplete(this.ranchcreature.loop);
			this.ranchcreature.loop.Enter("TellCreatureRancherIsReady", delegate(RancherChore.RancherChoreStates.Instance smi)
			{
				smi.ranchStation.targetRanchable.Trigger(1084749845, null);
			}).QueueAnim("working_loop", false, null).OnAnimQueueComplete(this.ranchcreature.pst);
			this.ranchcreature.pst.Enter("RanchCreature", delegate(RancherChore.RancherChoreStates.Instance smi)
			{
				smi.ranchStation.RanchCreature();
			}).QueueAnim("sweat_wipe", false, null).OnAnimQueueComplete(this.checkformoreranchables);
			this.checkformoreranchables.Enter("FindRanchable", delegate(RancherChore.RancherChoreStates.Instance smi)
			{
				smi.CheckForMoreRanchables();
			});
		}

		public StateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.TargetParameter rancher;

		private GameStateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.State movetoranch;

		private GameStateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.State waitforcreature_pre;

		private GameStateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.State waitforcreature;

		private RancherChore.RancherChoreStates.RanchState ranchcreature;

		private GameStateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.State wavegoodbye;

		private GameStateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.State checkformoreranchables;

		private class RanchState : GameStateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.State
		{
			public GameStateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.State pre;

			public GameStateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.State loop;

			public GameStateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.State pst;
		}

		public new class Instance : GameStateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.GameInstance
		{
			public Instance(KPrefabID rancher_station)
				: base(rancher_station)
			{
				this.ranchStation = rancher_station.GetSMI<RanchStation.Instance>();
			}

			public void CheckForMoreRanchables()
			{
				this.ranchStation.FindRanchable();
				if (this.ranchStation.IsCreatureAvailableForRanching())
				{
					this.GoTo(base.sm.movetoranch);
				}
				else
				{
					this.GoTo(null);
				}
			}

			public void TriggerRanchStationNoLongerAvailable()
			{
				this.ranchStation.TriggerRanchStationNoLongerAvailable();
			}

			public RanchStation.Instance ranchStation;
		}
	}
}
