using System;
using Klei.AI;
using STRINGS;
using TUNING;

public class RancherChore : Chore<RancherChore.RancherChoreStates.Instance>
{
	public RancherChore(KPrefabID rancher_station)
		: base(Db.Get().ChoreTypes.Ranch, rancher_station, null, false, null, null, null, PriorityScreen.PriorityClass.basic, 5, false, true, 0, false, ReportManager.ReportType.WorkTime)
	{
		this.AddPrecondition(RancherChore.IsOpenForRanching, rancher_station.GetSMI<RanchStation.Instance>());
		SkillPerkMissingComplainer component = base.GetComponent<SkillPerkMissingComplainer>();
		MultiSkillPerkMissingComplainer component2 = base.GetComponent<MultiSkillPerkMissingComplainer>();
		Debug.Assert(component != null || component2 != null, "Rancher chore can only have a skill perk or multi skill perk not both");
		if (component != null)
		{
			this.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, component.requiredSkillPerk);
		}
		else if (component2 != null)
		{
			foreach (string text in component2.requiredSkillPerks)
			{
				this.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, text);
			}
		}
		this.AddPrecondition(ChorePreconditions.instance.IsScheduledTime, Db.Get().ScheduleBlockTypes.Work);
		this.AddPrecondition(ChorePreconditions.instance.CanMoveTo, rancher_station.GetComponent<Building>());
		Operational component3 = rancher_station.GetComponent<Operational>();
		this.AddPrecondition(ChorePreconditions.instance.IsOperational, component3);
		Deconstructable component4 = rancher_station.GetComponent<Deconstructable>();
		this.AddPrecondition(ChorePreconditions.instance.IsNotMarkedForDeconstruction, component4);
		BuildingEnabledButton component5 = rancher_station.GetComponent<BuildingEnabledButton>();
		this.AddPrecondition(ChorePreconditions.instance.IsNotMarkedForDisable, component5);
		base.smi = new RancherChore.RancherChoreStates.Instance(rancher_station);
		base.SetPrioritizable(rancher_station.GetComponent<Prioritizable>());
	}

	public override void Begin(Chore.Precondition.Context context)
	{
		base.smi.sm.rancher.Set(context.consumerState.gameObject, base.smi, false);
		base.Begin(context);
	}

	protected override void End(string reason)
	{
		base.End(reason);
		base.smi.sm.rancher.Set(null, base.smi);
	}

	public static Chore.Precondition IsOpenForRanching = new Chore.Precondition
	{
		id = "IsCreatureAvailableForRanching",
		description = DUPLICANTS.CHORES.PRECONDITIONS.IS_CREATURE_AVAILABLE_FOR_RANCHING,
		sortOrder = -3,
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			RanchStation.Instance instance = data as RanchStation.Instance;
			return !instance.HasRancher && instance.IsCritterAvailableForRanching;
		}
	};

	public class RancherChoreStates : GameStateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.moveToRanch;
			base.Target(this.rancher);
			this.root.Exit("TriggerRanchStationNoLongerAvailable", delegate(RancherChore.RancherChoreStates.Instance smi)
			{
				smi.ranchStation.TriggerRanchStationNoLongerAvailable();
			});
			this.moveToRanch.MoveTo((RancherChore.RancherChoreStates.Instance smi) => Grid.PosToCell(smi.transform.GetPosition()), this.waitForAvailableRanchable, null, false);
			this.waitForAvailableRanchable.Enter("FindRanchable", delegate(RancherChore.RancherChoreStates.Instance smi)
			{
				smi.WaitForAvailableRanchable(0f);
			}).Update("FindRanchable", delegate(RancherChore.RancherChoreStates.Instance smi, float dt)
			{
				smi.WaitForAvailableRanchable(dt);
			}, UpdateRate.SIM_200ms, false);
			this.ranchCritter.ScheduleGoTo(0.5f, this.ranchCritter.callForCritter).EventTransition(GameHashes.CreatureAbandonedRanchStation, this.waitForAvailableRanchable, null);
			this.ranchCritter.callForCritter.ToggleAnims(new Func<RancherChore.RancherChoreStates.Instance, HashedString>(RancherChore.RancherChoreStates.GetRancherCallingAndWipeBrowAnim)).PlayAnim("calling_loop", KAnim.PlayMode.Loop).ScheduleActionNextFrame("TellCreatureRancherIsReady", delegate(RancherChore.RancherChoreStates.Instance smi)
			{
				smi.ranchStation.MessageRancherReady();
			})
				.Target(this.masterTarget)
				.EventTransition(GameHashes.CreatureArrivedAtRanchStation, this.ranchCritter.working, null);
			this.ranchCritter.working.ToggleWork<RancherChore.RancherWorkable>(this.masterTarget, this.ranchCritter.pst, this.waitForAvailableRanchable, null);
			this.ranchCritter.pst.Enter(delegate(RancherChore.RancherChoreStates.Instance smi)
			{
				if (!RancherChore.RancherChoreStates.HasWipeBrowAnim(smi))
				{
					smi.GoTo(this.waitForAvailableRanchable);
				}
			}).ToggleAnims(new Func<RancherChore.RancherChoreStates.Instance, HashedString>(RancherChore.RancherChoreStates.GetRancherCallingAndWipeBrowAnim)).QueueAnim("wipe_brow", false, null)
				.OnAnimQueueComplete(this.waitForAvailableRanchable);
		}

		private static HashedString GetRancherInteractAnim(RancherChore.RancherChoreStates.Instance smi)
		{
			return smi.ranchStation.def.RancherInteractAnim;
		}

		private static HashedString GetRancherCallingAndWipeBrowAnim(RancherChore.RancherChoreStates.Instance smi)
		{
			return smi.ranchStation.def.RancherCallingAndWipeBrowAnim;
		}

		private static bool HasWipeBrowAnim(RancherChore.RancherChoreStates.Instance smi)
		{
			return smi.ranchStation.def.RancherWipesBrowAnim;
		}

		public static bool TryRanchCreature(RancherChore.RancherChoreStates.Instance smi)
		{
			Debug.Assert(smi.ranchStation != null, "smi.ranchStation was null");
			RanchedStates.Instance activeRanchable = smi.ranchStation.ActiveRanchable;
			if (activeRanchable.IsNullOrStopped())
			{
				return false;
			}
			KPrefabID component = activeRanchable.GetComponent<KPrefabID>();
			smi.sm.rancher.Get(smi).Trigger(937885943, component.PrefabTag.Name);
			smi.ranchStation.RanchCreature();
			return true;
		}

		public StateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.TargetParameter rancher;

		private GameStateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.State moveToRanch;

		private RancherChore.RancherChoreStates.RanchState ranchCritter;

		private GameStateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.State waitForAvailableRanchable;

		private class RanchState : GameStateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.State
		{
			public GameStateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.State callForCritter;

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

			public void WaitForAvailableRanchable(float dt)
			{
				this.waitTime += dt;
				GameStateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.State state = (this.ranchStation.IsCritterAvailableForRanching ? base.sm.ranchCritter : null);
				if (state != null || this.waitTime >= 2f)
				{
					this.waitTime = 0f;
					this.GoTo(state);
				}
			}

			private const float WAIT_FOR_RANCHABLE_TIMEOUT = 2f;

			public RanchStation.Instance ranchStation;

			private float waitTime;
		}
	}

	public class RancherWorkable : Workable
	{
		protected override void OnPrefabInit()
		{
			base.OnPrefabInit();
			this.ranch = base.gameObject.GetSMI<RanchStation.Instance>();
			this.overrideAnims = new KAnimFile[] { Assets.GetAnim(this.ranch.def.RancherInteractAnim) };
			base.SetWorkTime(this.ranch.def.WorkTime);
			base.SetWorkerStatusItem(this.ranch.def.RanchingStatusItem);
			this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
			this.skillExperienceSkillGroup = Db.Get().SkillGroups.Ranching.Id;
			this.skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
			this.shouldShowSkillPerkStatusItem = true;
			this.lightEfficiencyBonus = false;
		}

		public override Klei.AI.Attribute GetWorkAttribute()
		{
			return Db.Get().Attributes.Ranching;
		}

		protected override void OnStartWork(WorkerBase worker)
		{
			if (this.ranch == null)
			{
				return;
			}
			if (this.ranch.def.OnRanchWorkBegins != null)
			{
				this.ranch.def.OnRanchWorkBegins(this.ranch.ActiveRanchable, this);
			}
			this.critterAnimController = this.ranch.ActiveRanchable.AnimController;
			this.critterAnimController.Play(this.ranch.def.RanchedPreAnim, KAnim.PlayMode.Once, 1f, 0f);
			this.critterAnimController.Queue(this.ranch.def.RanchedLoopAnim, KAnim.PlayMode.Loop, 1f, 0f);
		}

		protected override bool OnWorkTick(WorkerBase worker, float dt)
		{
			if (this.ranch.def.OnRanchWorkTick != null)
			{
				this.ranch.def.OnRanchWorkTick(this.ranch.ActiveRanchable.gameObject, dt, this);
			}
			return base.OnWorkTick(worker, dt);
		}

		public override void OnPendingCompleteWork(WorkerBase work)
		{
			RancherChore.RancherChoreStates.Instance smi = base.gameObject.GetSMI<RancherChore.RancherChoreStates.Instance>();
			if (this.ranch == null || smi == null)
			{
				return;
			}
			if (RancherChore.RancherChoreStates.TryRanchCreature(smi))
			{
				this.critterAnimController.Play(this.ranch.def.RanchedPstAnim, KAnim.PlayMode.Once, 1f, 0f);
			}
		}

		protected override void OnAbortWork(WorkerBase worker)
		{
			if (this.ranch == null || this.critterAnimController == null)
			{
				return;
			}
			this.critterAnimController.Play(this.ranch.def.RanchedAbortAnim, KAnim.PlayMode.Once, 1f, 0f);
		}

		private RanchStation.Instance ranch;

		private KBatchedAnimController critterAnimController;
	}
}
