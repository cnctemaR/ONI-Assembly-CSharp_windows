using System;
using System.Collections.Generic;

public abstract class StandardChoreBase : Chore
{
	public override int id { get; protected set; }

	public override int priorityMod { get; protected set; }

	public override ChoreType choreType { get; protected set; }

	public override ChoreDriver driver { get; protected set; }

	public override ChoreDriver lastDriver { get; protected set; }

	public override bool SatisfiesUrge(Urge urge)
	{
		return urge == this.choreType.urge;
	}

	public override bool IsValid()
	{
		return this.provider != null && this.gameObject.GetMyWorldId() != -1;
	}

	public override IStateMachineTarget target { get; protected set; }

	public override bool isComplete { get; protected set; }

	public override bool IsPreemptable { get; protected set; }

	public override ChoreConsumer overrideTarget { get; protected set; }

	public override Prioritizable prioritizable { get; protected set; }

	public override ChoreProvider provider { get; set; }

	public override bool runUntilComplete { get; set; }

	public override bool isExpanded { get; protected set; }

	public override bool CanPreempt(Chore.Precondition.Context context)
	{
		return this.IsPreemptable;
	}

	public override void PrepareChore(ref Chore.Precondition.Context context)
	{
	}

	public override string GetReportName(string context = null)
	{
		if (context == null || this.choreType.reportName == null)
		{
			return this.choreType.Name;
		}
		return string.Format(this.choreType.reportName, context);
	}

	public override void Cancel(string reason)
	{
		if (!this.RemoveFromProvider())
		{
			return;
		}
		if (this.addToDailyReport)
		{
			ReportManager.Instance.ReportValue(ReportManager.ReportType.ChoreStatus, -1f, this.choreType.Name, GameUtil.GetChoreName(this, null));
			SaveGame.Instance.ColonyAchievementTracker.LogSuitChore((this.driver != null) ? this.driver : this.lastDriver);
		}
		this.End(reason);
		this.Cleanup();
	}

	public override void Cleanup()
	{
		this.ClearPrioritizable();
	}

	public override ReportManager.ReportType GetReportType()
	{
		return this.reportType;
	}

	public override void AddPrecondition(Chore.Precondition precondition, object data = null)
	{
		this.arePreconditionsDirty = true;
		this.preconditions.Add(new Chore.PreconditionInstance
		{
			condition = precondition,
			data = data
		});
	}

	public override void CollectChores(ChoreConsumerState consumer_state, List<Chore.Precondition.Context> succeeded_contexts, List<Chore.Precondition.Context> incomplete_contexts, List<Chore.Precondition.Context> failed_contexts, bool is_attempting_override)
	{
		Chore.Precondition.Context context = new Chore.Precondition.Context(this, consumer_state, is_attempting_override, null);
		context.RunPreconditions();
		if (!context.IsComplete())
		{
			incomplete_contexts.Add(context);
			return;
		}
		if (context.IsSuccess())
		{
			succeeded_contexts.Add(context);
			return;
		}
		failed_contexts.Add(context);
	}

	public override void Fail(string reason)
	{
		if (this.provider == null)
		{
			return;
		}
		if (this.driver == null)
		{
			return;
		}
		if (!this.runUntilComplete)
		{
			this.Cancel(reason);
			return;
		}
		this.End(reason);
	}

	public override void Reserve(ChoreDriver reserver)
	{
		if (this.driver != null && this.driver != reserver && reserver != null)
		{
			Debug.LogErrorFormat("Chore.Reserve: driver already set {0} {1} {2}, provider {3}, driver {4} -> {5}", new object[]
			{
				this.id,
				base.GetType(),
				this.choreType.Id,
				this.provider,
				this.driver,
				reserver
			});
		}
		this.driver = reserver;
	}

	public override void Begin(Chore.Precondition.Context context)
	{
		if (this.driver != null && this.driver != context.consumerState.choreDriver)
		{
			Debug.LogErrorFormat("Chore.Begin driver already set {0} {1} {2}, provider {3}, driver {4} -> {5}", new object[]
			{
				this.id,
				base.GetType(),
				this.choreType.Id,
				this.provider,
				this.driver,
				context.consumerState.choreDriver
			});
		}
		if (this.provider == null)
		{
			Debug.LogErrorFormat("Chore.Begin provider is null {0} {1} {2}, provider {3}, driver {4}", new object[]
			{
				this.id,
				base.GetType(),
				this.choreType.Id,
				this.provider,
				this.driver
			});
		}
		this.driver = context.consumerState.choreDriver;
		StateMachine.Instance smi = this.GetSMI();
		smi.OnStop = (Action<string, StateMachine.Status>)Delegate.Combine(smi.OnStop, new Action<string, StateMachine.Status>(this.OnStateMachineStop));
		KSelectable component = this.driver.GetComponent<KSelectable>();
		if (component != null)
		{
			component.SetStatusItem(Db.Get().StatusItemCategories.Main, this.GetStatusItem(), this);
		}
		smi.StartSM();
		if (this.onBegin != null)
		{
			this.onBegin(this);
		}
	}

	public override bool InProgress()
	{
		return this.driver != null;
	}

	protected abstract StateMachine.Instance GetSMI();

	public StandardChoreBase(ChoreType chore_type, IStateMachineTarget target, ChoreProvider chore_provider, bool run_until_complete, Action<Chore> on_complete, Action<Chore> on_begin, Action<Chore> on_end, PriorityScreen.PriorityClass priority_class, int priority_value, bool is_preemptable, bool allow_in_context_menu, int priority_mod, bool add_to_daily_report, ReportManager.ReportType report_type)
	{
		this.target = target;
		if (priority_value == 2147483647)
		{
			priority_class = PriorityScreen.PriorityClass.topPriority;
			priority_value = 2;
		}
		if (priority_value < 1 || priority_value > 9)
		{
			Debug.LogErrorFormat("Priority Value Out Of Range: {0}", new object[] { priority_value });
		}
		this.masterPriority = new PrioritySetting(priority_class, priority_value);
		this.priorityMod = priority_mod;
		this.id = Chore.GetNextChoreID();
		if (chore_provider == null)
		{
			chore_provider = GlobalChoreProvider.Instance;
			DebugUtil.Assert(chore_provider != null);
		}
		this.choreType = chore_type;
		this.runUntilComplete = run_until_complete;
		this.onComplete = on_complete;
		this.onEnd = on_end;
		this.onBegin = on_begin;
		this.IsPreemptable = is_preemptable;
		this.AddPrecondition(ChorePreconditions.instance.IsValid, null);
		this.AddPrecondition(ChorePreconditions.instance.IsPermitted, null);
		this.AddPrecondition(ChorePreconditions.instance.IsPreemptable, null);
		this.AddPrecondition(ChorePreconditions.instance.HasUrge, null);
		this.AddPrecondition(ChorePreconditions.instance.IsMoreSatisfyingEarly, null);
		this.AddPrecondition(ChorePreconditions.instance.IsMoreSatisfyingLate, null);
		this.AddPrecondition(ChorePreconditions.instance.IsOverrideTargetNullOrMe, null);
		chore_provider.AddChore(this);
	}

	public virtual void SetPriorityMod(int priorityMod)
	{
		this.priorityMod = priorityMod;
	}

	public override List<Chore.PreconditionInstance> GetPreconditions()
	{
		if (this.arePreconditionsDirty)
		{
			List<Chore.PreconditionInstance> list = this.preconditions;
			lock (list)
			{
				if (this.arePreconditionsDirty)
				{
					this.preconditions.Sort((Chore.PreconditionInstance x, Chore.PreconditionInstance y) => x.condition.sortOrder.CompareTo(y.condition.sortOrder));
					this.arePreconditionsDirty = false;
				}
			}
		}
		return this.preconditions;
	}

	protected void SetPrioritizable(Prioritizable prioritizable)
	{
		if (prioritizable != null && prioritizable.IsPrioritizable())
		{
			this.prioritizable = prioritizable;
			this.masterPriority = prioritizable.GetMasterPriority();
			prioritizable.onPriorityChanged = (Action<PrioritySetting>)Delegate.Combine(prioritizable.onPriorityChanged, new Action<PrioritySetting>(this.OnMasterPriorityChanged));
		}
	}

	private void ClearPrioritizable()
	{
		if (this.prioritizable != null)
		{
			Prioritizable prioritizable = this.prioritizable;
			prioritizable.onPriorityChanged = (Action<PrioritySetting>)Delegate.Remove(prioritizable.onPriorityChanged, new Action<PrioritySetting>(this.OnMasterPriorityChanged));
		}
	}

	private void OnMasterPriorityChanged(PrioritySetting priority)
	{
		this.masterPriority = priority;
	}

	public void SetOverrideTarget(ChoreConsumer chore_consumer)
	{
		this.overrideTarget = chore_consumer;
		this.Fail("New override target");
	}

	protected virtual void End(string reason)
	{
		if (this.driver != null)
		{
			KSelectable component = this.driver.GetComponent<KSelectable>();
			if (component != null)
			{
				component.SetStatusItem(Db.Get().StatusItemCategories.Main, null, null);
			}
		}
		StateMachine.Instance smi = this.GetSMI();
		smi.OnStop = (Action<string, StateMachine.Status>)Delegate.Remove(smi.OnStop, new Action<string, StateMachine.Status>(this.OnStateMachineStop));
		smi.StopSM(reason);
		if (this.driver == null)
		{
			return;
		}
		this.lastDriver = this.driver;
		this.driver = null;
		if (this.onEnd != null)
		{
			this.onEnd(this);
		}
		if (this.onExit != null)
		{
			this.onExit(this);
		}
		this.driver = null;
	}

	protected void Succeed(string reason)
	{
		if (!this.RemoveFromProvider())
		{
			return;
		}
		this.isComplete = true;
		if (this.onComplete != null)
		{
			this.onComplete(this);
		}
		if (this.addToDailyReport)
		{
			ReportManager.Instance.ReportValue(ReportManager.ReportType.ChoreStatus, -1f, this.choreType.Name, GameUtil.GetChoreName(this, null));
			SaveGame.Instance.ColonyAchievementTracker.LogSuitChore((this.driver != null) ? this.driver : this.lastDriver);
		}
		this.End(reason);
		this.Cleanup();
	}

	protected virtual StatusItem GetStatusItem()
	{
		return this.choreType.statusItem;
	}

	protected virtual void OnStateMachineStop(string reason, StateMachine.Status status)
	{
		if (status == StateMachine.Status.Success)
		{
			this.Succeed(reason);
			return;
		}
		this.Fail(reason);
	}

	private bool RemoveFromProvider()
	{
		if (this.provider != null)
		{
			this.provider.RemoveChore(this);
			return true;
		}
		return false;
	}

	private Action<Chore> onBegin;

	private Action<Chore> onEnd;

	public Action<Chore> onCleanup;

	private List<Chore.PreconditionInstance> preconditions = new List<Chore.PreconditionInstance>();

	private bool arePreconditionsDirty;

	public bool addToDailyReport;

	public ReportManager.ReportType reportType;
}
