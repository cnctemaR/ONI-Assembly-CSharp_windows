using System;
using System.Diagnostics;
using STRINGS;

public class ChoreDriver : StateMachineComponent<ChoreDriver.StatesInstance>
{
	public Chore GetCurrentChore()
	{
		return base.smi.GetCurrentChore();
	}

	public bool HasChore()
	{
		return base.smi.GetCurrentChore() != null;
	}

	public void StopChore()
	{
		base.smi.sm.stop.Trigger(base.smi);
	}

	public void SetChore(Chore.Precondition.Context context)
	{
		Chore currentChore = base.smi.GetCurrentChore();
		if (currentChore != context.chore)
		{
			this.StopChore();
			if (context.chore.IsValid())
			{
				context.chore.PrepareChore(ref context);
				this.context = context;
				base.smi.sm.nextChore.Set(context.chore, base.smi, false);
				return;
			}
			string text = "Null";
			string text2 = "Null";
			if (currentChore != null)
			{
				text = currentChore.GetType().Name;
			}
			if (context.chore != null)
			{
				text2 = context.chore.GetType().Name;
			}
			global::Debug.LogWarning(string.Concat(new string[] { "Stopping chore ", text, " to start ", text2, " but stopping the first chore cancelled the second one." }));
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	[MyCmpAdd]
	private User user;

	private Chore.Precondition.Context context;

	public class StatesInstance : GameStateMachine<ChoreDriver.States, ChoreDriver.StatesInstance, ChoreDriver, object>.GameInstance
	{
		public string masterProperName { get; private set; }

		public KPrefabID masterPrefabId { get; private set; }

		public Navigator navigator { get; private set; }

		public WorkerBase worker { get; private set; }

		[Conditional("ENABLE_LOGGER")]
		public void Log(string name, string param)
		{
		}

		public StatesInstance(ChoreDriver master)
			: base(master)
		{
			this.masterProperName = base.master.GetProperName();
			this.masterPrefabId = base.master.GetComponent<KPrefabID>();
			this.navigator = base.master.GetComponent<Navigator>();
			this.worker = base.master.GetComponent<WorkerBase>();
			this.choreConsumer = base.GetComponent<ChoreConsumer>();
			ChoreConsumer choreConsumer = this.choreConsumer;
			choreConsumer.choreRulesChanged = (global::System.Action)Delegate.Combine(choreConsumer.choreRulesChanged, new global::System.Action(this.OnChoreRulesChanged));
		}

		public void BeginChore()
		{
			Chore nextChore = this.GetNextChore();
			Chore chore = base.smi.sm.currentChore.Set(nextChore, base.smi, false);
			if (chore != null && chore.IsPreemptable && chore.driver != null)
			{
				chore.Fail("Preemption!");
			}
			base.smi.sm.nextChore.Set(null, base.smi, false);
			Chore chore2 = chore;
			chore2.onExit = (Action<Chore>)Delegate.Combine(chore2.onExit, new Action<Chore>(this.OnChoreExit));
			chore.Begin(base.master.context);
			base.Trigger(-1988963660, chore);
		}

		public void EndChore(string reason)
		{
			if (this.GetCurrentChore() != null)
			{
				Chore currentChore = this.GetCurrentChore();
				base.smi.sm.currentChore.Set(null, base.smi, false);
				Chore chore = currentChore;
				chore.onExit = (Action<Chore>)Delegate.Remove(chore.onExit, new Action<Chore>(this.OnChoreExit));
				currentChore.Fail(reason);
				base.Trigger(1745615042, currentChore);
			}
			if (base.smi.choreConsumer.prioritizeBrainIfNoChore)
			{
				Game.BrainScheduler.PrioritizeBrain(this.brain);
			}
		}

		private void OnChoreExit(Chore chore)
		{
			base.smi.sm.stop.Trigger(base.smi);
		}

		public Chore GetNextChore()
		{
			return base.smi.sm.nextChore.Get(base.smi);
		}

		public Chore GetCurrentChore()
		{
			return base.smi.sm.currentChore.Get(base.smi);
		}

		private void OnChoreRulesChanged()
		{
			Chore currentChore = this.GetCurrentChore();
			if (currentChore != null && !this.choreConsumer.IsPermittedOrEnabled(currentChore.choreType, currentChore))
			{
				this.EndChore("Permissions changed");
			}
		}

		private ChoreConsumer choreConsumer;

		[MyCmpGet]
		private Brain brain;
	}

	public class States : GameStateMachine<ChoreDriver.States, ChoreDriver.StatesInstance, ChoreDriver>
	{
		private static bool IsLiveMinion(ChoreDriver.StatesInstance smi)
		{
			return smi.masterPrefabId.HasTag(GameTags.BaseMinion) && !smi.masterPrefabId.HasTag(GameTags.Dead);
		}

		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.nochore;
			this.saveHistory = true;
			this.nochore.Update(delegate(ChoreDriver.StatesInstance smi, float dt)
			{
				if (!ChoreDriver.States.IsLiveMinion(smi))
				{
					return;
				}
				ReportManager.Instance.ReportValueWithPrefabInstanceContext(ReportManager.ReportType.WorkTime, dt, smi.masterPrefabId, string.Format(UI.ENDOFDAYREPORT.NOTES.TIME_SPENT, DUPLICANTS.CHORES.THINKING.NAME));
			}, UpdateRate.SIM_200ms, false).ParamTransition<Chore>(this.nextChore, this.haschore, (ChoreDriver.StatesInstance smi, Chore next_chore) => next_chore != null);
			this.haschore.Enter("BeginChore", delegate(ChoreDriver.StatesInstance smi)
			{
				smi.BeginChore();
			}).Update(delegate(ChoreDriver.StatesInstance smi, float dt)
			{
				if (!ChoreDriver.States.IsLiveMinion(smi))
				{
					return;
				}
				Chore chore = this.currentChore.Get(smi);
				if (chore == null)
				{
					return;
				}
				ReportManager.ReportType reportType = chore.GetReportType();
				string text;
				if (smi.navigator.IsMoving())
				{
					reportType = ReportManager.ReportType.TravelTime;
					text = GameUtil.GetChoreName(chore, null);
				}
				else
				{
					Workable workable = smi.worker.GetWorkable();
					if (workable != null)
					{
						reportType = workable.GetReportType();
					}
					text = string.Format(UI.ENDOFDAYREPORT.NOTES.WORK_TIME, GameUtil.GetChoreName(chore, null));
				}
				ReportManager.Instance.ReportValueWithPrefabInstanceContext(reportType, dt, smi.masterPrefabId, text);
			}, UpdateRate.SIM_200ms, false).Exit("EndChore", delegate(ChoreDriver.StatesInstance smi)
			{
				smi.EndChore("ChoreDriver.SignalStop");
			})
				.OnSignal(this.stop, this.nochore);
		}

		public StateMachine<ChoreDriver.States, ChoreDriver.StatesInstance, ChoreDriver, object>.ObjectParameter<Chore> currentChore;

		public StateMachine<ChoreDriver.States, ChoreDriver.StatesInstance, ChoreDriver, object>.ObjectParameter<Chore> nextChore;

		public StateMachine<ChoreDriver.States, ChoreDriver.StatesInstance, ChoreDriver, object>.Signal stop;

		public GameStateMachine<ChoreDriver.States, ChoreDriver.StatesInstance, ChoreDriver, object>.State nochore;

		public GameStateMachine<ChoreDriver.States, ChoreDriver.StatesInstance, ChoreDriver, object>.State haschore;
	}
}
