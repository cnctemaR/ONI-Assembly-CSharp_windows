using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public abstract class Chore
{
	public Chore(ChoreType chore_type, ChoreProvider chore_provider, Tag[] chore_tags, bool run_until_complete, Action<Chore> on_complete, Action<Chore> on_begin, Action<Chore> on_end, PriorityScreen.PriorityClass priority_class, int priority_value, bool is_preemptable, bool allow_in_context_menu, int priority_mod)
	{
		if (priority_value == 2147483647)
		{
			priority_class = PriorityScreen.PriorityClass.emergency;
			priority_value = 2;
		}
		this.masterPriority = new PrioritySetting(priority_class, priority_value);
		this.priorityMod = priority_mod;
		this.id = ++Chore.nextId;
		if (chore_provider == null)
		{
			chore_provider = GlobalChoreProvider.Instance;
			DebugUtil.Assert(chore_provider != null, "Assert!", string.Empty, string.Empty);
		}
		this.choreType = chore_type;
		this.choreTags = chore_tags;
		this.runUntilComplete = run_until_complete;
		this.onComplete = on_complete;
		this.onEnd = on_end;
		this.onBegin = on_begin;
		this.IsPreemptable = is_preemptable;
		this.AddPrecondition(ChorePreconditions.instance.IsValid, null);
		this.AddPrecondition(ChorePreconditions.instance.IsPermitted, null);
		this.AddPrecondition(ChorePreconditions.instance.IsPreemptable, null);
		this.AddPrecondition(ChorePreconditions.instance.HasUrge, null);
		this.AddPrecondition(ChorePreconditions.instance.IsMoreSatisfying, null);
		this.AddPrecondition(ChorePreconditions.instance.IsOverrideTargetNullOrMe, null);
		chore_provider.AddChore(this);
	}

	public int id { get; private set; }

	public ChoreDriver driver { get; set; }

	public ChoreDriver lastDriver { get; set; }

	protected abstract StateMachine.Instance GetSMI();

	public ChoreType choreType { get; set; }

	public ChoreProvider provider { get; set; }

	public ChoreConsumer overrideTarget { get; private set; }

	public bool isComplete { get; protected set; }

	public IStateMachineTarget target { get; protected set; }

	public Tag[] choreTags { get; private set; }

	public bool runUntilComplete { get; set; }

	public int priorityMod { get; set; }

	public bool InProgress()
	{
		return this.driver != null;
	}

	public abstract GameObject gameObject { get; }

	public abstract bool isNull { get; }

	public bool IsValid()
	{
		return this.provider != null;
	}

	public bool IsPreemptable { get; protected set; }

	public virtual void Cleanup()
	{
		this.ClearPrioritizable();
	}

	public void SetPriorityMod(int priorityMod)
	{
		this.priorityMod = priorityMod;
	}

	public List<Chore.PreconditionInstance> GetPreconditions()
	{
		if (this.arePreconditionsDirty)
		{
			this.preconditions.Sort((Chore.PreconditionInstance x, Chore.PreconditionInstance y) => x.sortOrder.CompareTo(y.sortOrder));
			this.arePreconditionsDirty = false;
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
		if (chore_consumer != null)
		{
			string name = chore_consumer.name;
		}
		this.overrideTarget = chore_consumer;
		this.Fail("New override target");
	}

	public void AddPrecondition(Chore.Precondition precondition, object data = null)
	{
		this.arePreconditionsDirty = true;
		this.preconditions.Add(new Chore.PreconditionInstance
		{
			id = precondition.id,
			description = precondition.description,
			sortOrder = precondition.sortOrder,
			fn = precondition.fn,
			data = data
		});
	}

	public virtual void CollectChores(ChoreConsumerState consumer_state, List<Chore.Precondition.Context> succeeded_contexts, List<Chore.Precondition.Context> failed_contexts, bool is_attempting_override)
	{
		Chore.Precondition.Context context = new Chore.Precondition.Context(this, consumer_state, is_attempting_override, null);
		context.RunPreconditions();
		if (context.IsSuccess())
		{
			succeeded_contexts.Add(context);
		}
		else
		{
			failed_contexts.Add(context);
		}
	}

	public bool SatisfiesUrge(Urge urge)
	{
		return urge == this.choreType.urge;
	}

	public virtual void PrepareChore(ref Chore.Precondition.Context context)
	{
	}

	public virtual string ResolveString(string str)
	{
		return str;
	}

	public virtual void Begin(Chore.Precondition.Context context)
	{
		DebugUtil.Assert(this.driver == null, "Assert!", string.Empty, string.Empty);
		if (this.provider == null)
		{
			global::Debug.LogError(string.Concat(new object[]
			{
				"Chore has null provider: ",
				base.GetType(),
				" ",
				this.choreType.Id
			}), null);
		}
		this.driver = context.consumerState.choreDriver;
		StateMachine.Instance smi = this.GetSMI();
		StateMachine.Instance instance = smi;
		instance.OnStop = (Action<string, StateMachine.Status>)Delegate.Combine(instance.OnStop, new Action<string, StateMachine.Status>(this.OnStateMachineStop));
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
		StateMachine.Instance instance = smi;
		instance.OnStop = (Action<string, StateMachine.Status>)Delegate.Remove(instance.OnStop, new Action<string, StateMachine.Status>(this.OnStateMachineStop));
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
		this.End(reason);
		this.Cleanup();
	}

	protected virtual StatusItem GetStatusItem()
	{
		return this.choreType.statusItem;
	}

	public virtual void Fail(string reason)
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

	public void Cancel(string reason)
	{
		if (!this.RemoveFromProvider())
		{
			return;
		}
		this.End(reason);
		this.Cleanup();
	}

	protected virtual void OnStateMachineStop(string reason, StateMachine.Status status)
	{
		if (status == StateMachine.Status.Success)
		{
			this.Succeed(reason);
		}
		else
		{
			this.Fail(reason);
		}
	}

	private bool RemoveFromProvider()
	{
		if (this.provider != null)
		{
			this.provider.RemoveChore(this);
			this.provider = null;
			return true;
		}
		return false;
	}

	public virtual bool CanPreempt(Chore.Precondition.Context context)
	{
		return this.IsPreemptable;
	}

	protected virtual void ShowCustomEditor(string filter, int width)
	{
	}

	public virtual string GetReportName()
	{
		return this.choreType.Name;
	}

	private static int nextId;

	public bool isExpanded;

	public bool showAvailabilityInHoverText = true;

	public PrioritySetting masterPriority;

	public Action<Chore> onExit;

	public Action<Chore> onComplete;

	private Action<Chore> onBegin;

	private Action<Chore> onEnd;

	public Action<Chore> onCleanup;

	public bool debug;

	private List<Chore.PreconditionInstance> preconditions = new List<Chore.PreconditionInstance>();

	private bool arePreconditionsDirty;

	private Prioritizable prioritizable;

	public const int MAX_PLAYER_BASIC_PRIORITY = 9;

	public const int MIN_PLAYER_BASIC_PRIORITY = 1;

	public const int MAX_PLAYER_HIGH_PRIORITY = 9;

	public const int MIN_PLAYER_HIGH_PRIORITY = 1;

	public const int MAX_PLAYER_EMERGENCY_PRIORITY = 1;

	public const int MIN_PLAYER_EMERGENCY_PRIORITY = 1;

	public const int DEFAULT_BASIC_PRIORITY = 5;

	public const int MAX_BASIC_PRIORITY = 10;

	public const int MIN_BASIC_PRIORITY = 0;

	public static bool ENABLE_PERSONAL_PRIORITIES = true;

	public delegate bool PreconditionFn(ref Chore.Precondition.Context context, object data);

	public struct PreconditionInstance
	{
		public string id;

		public string description;

		public int sortOrder;

		public Chore.PreconditionFn fn;

		public object data;
	}

	public struct Precondition
	{
		public string id;

		public string description;

		public int sortOrder;

		public Chore.PreconditionFn fn;

		[DebuggerDisplay("{chore.GetType()}, {chore.gameObject.name}")]
		public struct Context : IComparable<Chore.Precondition.Context>, IEquatable<Chore.Precondition.Context>
		{
			public Context(Chore chore, ChoreConsumerState consumer_state, bool is_attempting_override, object data = null)
			{
				this.masterPriority = chore.masterPriority;
				this.personalPriority = consumer_state.consumer.GetPersonalPriority(chore.choreType);
				this.priority = 0;
				this.priorityMod = chore.priorityMod;
				this.consumerPriority = 0;
				this.interruptPriority = 0;
				this.cost = 0;
				this.chore = chore;
				this.consumerState = consumer_state;
				this.failedPreconditionId = -1;
				this.isAttemptingOverride = is_attempting_override;
				this.data = data;
				this.choreTypeForPermission = chore.choreType;
				this.SetPriority(chore);
			}

			public void Set(Chore chore, ChoreConsumerState consumer_state, bool is_attempting_override, object data = null)
			{
				this.masterPriority = chore.masterPriority;
				this.priority = 0;
				this.priorityMod = chore.priorityMod;
				this.consumerPriority = 0;
				this.interruptPriority = 0;
				this.cost = 0;
				this.chore = chore;
				this.consumerState = consumer_state;
				this.failedPreconditionId = -1;
				this.isAttemptingOverride = is_attempting_override;
				this.data = data;
				this.choreTypeForPermission = chore.choreType;
				this.SetPriority(chore);
			}

			public void SetPriority(Chore chore)
			{
				this.priority = ((!Game.Instance.advancedPersonalPriorities) ? chore.choreType.priority : chore.choreType.explicitPriority);
				this.priorityMod = chore.priorityMod;
				this.interruptPriority = chore.choreType.interruptPriority;
			}

			public bool IsSuccess()
			{
				return this.failedPreconditionId == -1;
			}

			public void RunPreconditions()
			{
				if (this.chore.debug)
				{
					int num = 0;
					num++;
					if (this.consumerState.consumer.debug)
					{
						num++;
					}
				}
				for (int i = 0; i < this.chore.preconditions.Count; i++)
				{
					Chore.PreconditionInstance preconditionInstance = this.chore.preconditions[i];
					if (!preconditionInstance.fn(ref this, preconditionInstance.data))
					{
						this.failedPreconditionId = i;
						break;
					}
				}
			}

			public int CompareTo(Chore.Precondition.Context obj)
			{
				bool flag = this.failedPreconditionId != -1;
				bool flag2 = obj.failedPreconditionId != -1;
				if (flag != flag2)
				{
					return (!flag) ? 1 : (-1);
				}
				int num = this.masterPriority.priority_class - obj.masterPriority.priority_class;
				if (num != 0)
				{
					return num;
				}
				int num2 = this.personalPriority - obj.personalPriority;
				if (num2 != 0)
				{
					return num2;
				}
				int num3 = this.masterPriority.priority_value - obj.masterPriority.priority_value;
				if (num3 != 0)
				{
					return num3;
				}
				int num4 = this.priority - obj.priority;
				if (num4 != 0)
				{
					return num4;
				}
				int num5 = this.priorityMod - obj.priorityMod;
				if (num5 != 0)
				{
					return num5;
				}
				int num6 = this.consumerPriority - obj.consumerPriority;
				if (num6 != 0)
				{
					return num6;
				}
				return obj.cost - this.cost;
			}

			public override bool Equals(object obj)
			{
				Chore.Precondition.Context context = (Chore.Precondition.Context)obj;
				return this.CompareTo(context) == 0;
			}

			public bool Equals(Chore.Precondition.Context other)
			{
				return this.CompareTo(other) == 0;
			}

			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			public static bool operator ==(Chore.Precondition.Context x, Chore.Precondition.Context y)
			{
				return x.CompareTo(y) == 0;
			}

			public static bool operator !=(Chore.Precondition.Context x, Chore.Precondition.Context y)
			{
				return x.CompareTo(y) != 0;
			}

			public PrioritySetting masterPriority;

			public int personalPriority;

			public int priority;

			public int priorityMod;

			public int interruptPriority;

			public int cost;

			public int consumerPriority;

			public Chore chore;

			public ChoreConsumerState consumerState;

			public int failedPreconditionId;

			public object data;

			public bool isAttemptingOverride;

			public ChoreType choreTypeForPermission;
		}
	}
}
