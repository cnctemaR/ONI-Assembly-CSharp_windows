using System;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class RancherChore : Chore<RancherChore.RancherChoreStates.Instance>
{
	public RancherChore(KPrefabID rancher_station)
	{
		Chore.Precondition precondition = default(Chore.Precondition);
		precondition.id = "IsCreatureAvailableForRanching";
		precondition.description = DUPLICANTS.CHORES.PRECONDITIONS.IS_CREATURE_AVAILABLE_FOR_RANCHING;
		precondition.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return (data as RanchStation.Instance).IsCreatureAvailableForRanching();
		};
		this.IsCreatureAvailableForRanching = precondition;
		base..ctor(Db.Get().ChoreTypes.Ranch, rancher_station, null, false, null, null, null, PriorityScreen.PriorityClass.basic, 5, false, true, 0, false, ReportManager.ReportType.WorkTime);
		base.AddPrecondition(this.IsCreatureAvailableForRanching, rancher_station.GetSMI<RanchStation.Instance>());
		base.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, Db.Get().SkillPerks.CanUseRanchStation.Id);
		base.AddPrecondition(ChorePreconditions.instance.IsScheduledTime, Db.Get().ScheduleBlockTypes.Work);
		base.AddPrecondition(ChorePreconditions.instance.CanMoveTo, rancher_station.GetComponent<Building>());
		Operational component = rancher_station.GetComponent<Operational>();
		base.AddPrecondition(ChorePreconditions.instance.IsOperational, component);
		Deconstructable component2 = rancher_station.GetComponent<Deconstructable>();
		base.AddPrecondition(ChorePreconditions.instance.IsNotMarkedForDeconstruction, component2);
		BuildingEnabledButton component3 = rancher_station.GetComponent<BuildingEnabledButton>();
		base.AddPrecondition(ChorePreconditions.instance.IsNotMarkedForDisable, component3);
		base.smi = new RancherChore.RancherChoreStates.Instance(rancher_station);
		base.SetPrioritizable(rancher_station.GetComponent<Prioritizable>());
	}

	public override void Begin(Chore.Precondition.Context context)
	{
		base.smi.sm.rancher.Set(context.consumerState.gameObject, base.smi);
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
			this.movetoranch.MoveTo((RancherChore.RancherChoreStates.Instance smi) => Grid.PosToCell(smi.transform.GetPosition()), this.waitforcreature_pre, null, false).Transition(this.checkformoreranchables, new StateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(RancherChore.RancherChoreStates.HasCreatureLeft), UpdateRate.SIM_1000ms);
			this.waitforcreature_pre.EnterTransition(null, (RancherChore.RancherChoreStates.Instance smi) => smi.ranchStation.IsNullOrStopped()).Transition(this.checkformoreranchables, new StateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(RancherChore.RancherChoreStates.HasCreatureLeft), UpdateRate.SIM_1000ms).EnterTransition(this.waitforcreature, (RancherChore.RancherChoreStates.Instance smi) => true);
			this.waitforcreature.Transition(this.checkformoreranchables, new StateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(RancherChore.RancherChoreStates.HasCreatureLeft), UpdateRate.SIM_1000ms).ToggleAnims("anim_interacts_rancherstation_kanim", 0f, "").PlayAnim("calling_loop", KAnim.PlayMode.Loop)
				.Enter(new StateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.State.Callback(RancherChore.RancherChoreStates.FaceCreature))
				.Enter("TellCreatureToGoGetRanched", delegate(RancherChore.RancherChoreStates.Instance smi)
				{
					smi.ranchStation.SetRancherIsAvailableForRanching();
				})
				.Exit("ClearRancherIsAvailableForRanching", delegate(RancherChore.RancherChoreStates.Instance smi)
				{
					smi.ranchStation.ClearRancherIsAvailableForRanching();
				})
				.Target(this.masterTarget)
				.EventTransition(GameHashes.CreatureArrivedAtRanchStation, this.ranchcreature, null);
			this.ranchcreature.Transition(this.checkformoreranchables, new StateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(RancherChore.RancherChoreStates.HasCreatureLeft), UpdateRate.SIM_1000ms).DefaultState(this.ranchcreature.working).EventTransition(GameHashes.CreatureAbandonedRanchStation, this.checkformoreranchables, null)
				.Enter(new StateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.State.Callback(RancherChore.RancherChoreStates.SetCreatureLayer))
				.Exit(new StateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.State.Callback(RancherChore.RancherChoreStates.ClearCreatureLayer));
			this.ranchcreature.working.Enter("TellCreatureRancherIsReady", delegate(RancherChore.RancherChoreStates.Instance smi)
			{
				smi.TellCreatureRancherIsReady();
			}).ToggleWork<RancherChore.RancherWorkable>(this.masterTarget, this.ranchcreature.pst, this.checkformoreranchables, null);
			this.ranchcreature.pst.ToggleAnims(new Func<RancherChore.RancherChoreStates.Instance, HashedString>(RancherChore.RancherChoreStates.GetRancherInteractAnim)).QueueAnim("wipe_brow", false, null).OnAnimQueueComplete(this.checkformoreranchables);
			this.checkformoreranchables.Enter("FindRanchable", delegate(RancherChore.RancherChoreStates.Instance smi)
			{
				smi.CheckForMoreRanchables();
			}).Update("FindRanchable", delegate(RancherChore.RancherChoreStates.Instance smi, float dt)
			{
				smi.CheckForMoreRanchables();
			}, UpdateRate.SIM_200ms, false);
		}

		private static bool HasCreatureLeft(RancherChore.RancherChoreStates.Instance smi)
		{
			return smi.ranchStation.targetRanchable.IsNullOrStopped() || !smi.ranchStation.targetRanchable.GetComponent<ChoreConsumer>().IsChoreEqualOrAboveCurrentChorePriority<RanchedStates>();
		}

		private static void SetCreatureLayer(RancherChore.RancherChoreStates.Instance smi)
		{
			if (smi.ranchStation.targetRanchable.IsNullOrStopped())
			{
				return;
			}
			smi.ranchStation.targetRanchable.Get<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.BuildingUse);
		}

		private static void ClearCreatureLayer(RancherChore.RancherChoreStates.Instance smi)
		{
			if (smi.ranchStation.targetRanchable.IsNullOrStopped())
			{
				return;
			}
			smi.ranchStation.targetRanchable.Get<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.Creatures);
		}

		private static HashedString GetRancherInteractAnim(RancherChore.RancherChoreStates.Instance smi)
		{
			return smi.ranchStation.def.rancherInteractAnim;
		}

		private static void FaceCreature(RancherChore.RancherChoreStates.Instance smi)
		{
			Facing facing = smi.sm.rancher.Get<Facing>(smi);
			Vector3 position = smi.ranchStation.targetRanchable.transform.GetPosition();
			facing.Face(position);
		}

		public static void RanchCreature(RancherChore.RancherChoreStates.Instance smi)
		{
			global::Debug.Assert(smi.ranchStation != null, "smi.ranchStation was null");
			RanchableMonitor.Instance targetRanchable = smi.ranchStation.targetRanchable;
			if (targetRanchable.IsNullOrStopped())
			{
				return;
			}
			KPrefabID component = targetRanchable.GetComponent<KPrefabID>();
			smi.sm.rancher.Get(smi).Trigger(937885943, component.PrefabTag.Name);
			smi.ranchStation.RanchCreature();
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
			public GameStateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.State working;

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
					return;
				}
				this.GoTo(null);
			}

			public void TriggerRanchStationNoLongerAvailable()
			{
				this.ranchStation.TriggerRanchStationNoLongerAvailable();
			}

			public void TellCreatureRancherIsReady()
			{
				if (!this.ranchStation.targetRanchable.IsNullOrStopped())
				{
					this.ranchStation.targetRanchable.Trigger(1084749845, null);
				}
			}

			public RanchStation.Instance ranchStation;
		}
	}

	public class RancherWorkable : Workable
	{
		protected override void OnPrefabInit()
		{
			RanchStation.Instance smi = base.gameObject.GetSMI<RanchStation.Instance>();
			this.overrideAnims = new KAnimFile[] { Assets.GetAnim(smi.def.rancherInteractAnim) };
			base.SetWorkTime(smi.def.worktime);
			base.SetWorkerStatusItem(smi.def.ranchingStatusItem);
			this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
			this.skillExperienceSkillGroup = Db.Get().SkillGroups.Ranching.Id;
			this.skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
			this.lightEfficiencyBonus = false;
		}

		public override Klei.AI.Attribute GetWorkAttribute()
		{
			return Db.Get().Attributes.Ranching;
		}

		protected override void OnStartWork(Worker worker)
		{
			RanchStation.Instance smi = base.gameObject.GetSMI<RanchStation.Instance>();
			if (smi != null)
			{
				smi.targetRanchable.Get<KBatchedAnimController>().Play(smi.def.ranchedPreAnim, KAnim.PlayMode.Once, 1f, 0f);
				smi.targetRanchable.Get<KBatchedAnimController>().Queue(smi.def.ranchedLoopAnim, KAnim.PlayMode.Loop, 1f, 0f);
			}
		}

		public override void OnPendingCompleteWork(Worker work)
		{
			RanchStation.Instance smi = base.gameObject.GetSMI<RanchStation.Instance>();
			if (smi != null)
			{
				smi.targetRanchable.Get<KBatchedAnimController>().Play(smi.def.ranchedPstAnim, KAnim.PlayMode.Once, 1f, 0f);
				RancherChore.RancherChoreStates.Instance smi2 = base.gameObject.GetSMI<RancherChore.RancherChoreStates.Instance>();
				if (smi2 != null)
				{
					RancherChore.RancherChoreStates.RanchCreature(smi2);
				}
			}
		}
	}
}
