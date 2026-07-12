using System;
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
			Debug.LogWarning(string.Concat(new string[] { "Stopping chore ", text, " to start ", text2, " but stopping the first chore cancelled the second one." }));
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

		public Worker worker { get; private set; }

		public StatesInstance(ChoreDriver master)
			: base(master)
		{
			this.masterProperName = base.master.GetProperName();
			this.masterPrefabId = base.master.GetComponent<KPrefabID>();
			this.navigator = base.master.GetComponent<Navigator>();
			this.worker = base.master.GetComponent<Worker>();
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
	}

	public class States : GameStateMachine<ChoreDriver.States, ChoreDriver.StatesInstance, ChoreDriver>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.nochore;
			this.saveHistory = true;
			this.nochore.Update(delegate(ChoreDriver.StatesInstance smi, float dt)
			{
				if (smi.masterPrefabId.IsPrefabID(GameTags.Minion) && !smi.masterPrefabId.HasTag(GameTags.Dead))
				{
					ReportManager.Instance.ReportValue(ReportManager.ReportType.WorkTime, dt, string.Format(UI.ENDOFDAYREPORT.NOTES.TIME_SPENT, DUPLICANTS.CHORES.THINKING.NAME), smi.master.GetProperName());
				}
			}, UpdateRate.SIM_200ms, false).ParamTransition<Chore>(this.nextChore, this.haschore, (ChoreDriver.StatesInstance smi, Chore next_chore) => next_chore != null);
			this.haschore.Enter("BeginChore", delegate(ChoreDriver.StatesInstance smi)
			{
				smi.BeginChore();
			}).Update(delegate(ChoreDriver.StatesInstance smi, float dt)
			{
				if (smi.masterPrefabId.IsPrefabID(GameTags.Minion) && !smi.masterPrefabId.HasTag(GameTags.Dead))
				{
					Chore chore = this.currentChore.Get(smi);
					if (chore == null)
					{
						return;
					}
					if (smi.navigator.IsMoving())
					{
						ReportManager.Instance.ReportValue(ReportManager.ReportType.TravelTime, dt, GameUtil.GetChoreName(chore, null), smi.master.GetProperName());
						return;
					}
					ReportManager.ReportType reportType = chore.GetReportType();
					Workable workable = smi.worker.workable;
					if (workable != null)
					{
						ReportManager.ReportType reportType2 = workable.GetReportType();
						if (reportType != reportType2)
						{
							reportType = reportType2;
						}
					}
					ReportManager.Instance.ReportValue(reportType, dt, string.Format(UI.ENDOFDAYREPORT.NOTES.WORK_TIME, GameUtil.GetChoreName(chore, null)), smi.master.GetProperName());
				}
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
