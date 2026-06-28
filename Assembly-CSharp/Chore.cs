using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Chore
{
	public Chore(ChoreType chore_type, ChoreProvider chore_provider, bool run_until_complete, Action<Chore> on_complete, Action<Chore> on_begin, Action<Chore> on_end, int master_priority, bool is_preemptable, bool allow_in_context_menu, int priority_mod)
	{
		if (master_priority == 2147483647)
		{
			master_priority = 10;
		}
		this.masterPriority = master_priority;
		this.priorityMod = priority_mod;
		this.id = ++Chore.nextId;
		if (chore_provider == null)
		{
			chore_provider = GlobalChoreProvider.Instance;
		}
		this.choreType = chore_type;
		this.runUntilComplete = run_until_complete;
		this.onComplete = on_complete;
		this.onEnd = on_end;
		this.onBegin = on_begin;
		this.IsPreemptable = is_preemptable;
		this.AddPrecondition(ChorePreconditions.IsValid, null);
		this.AddPrecondition(ChorePreconditions.IsPermitted, null);
		this.AddPrecondition(ChorePreconditions.ChoreDriverIsNull, null);
		this.AddPrecondition(ChorePreconditions.HasUrge, null);
		this.AddPrecondition(ChorePreconditions.IsMoreSatisfying, null);
		this.AddPrecondition(ChorePreconditions.IsOverrideTargetNullOrMe, null);
		if (is_preemptable)
		{
			this.AddPrecondition(ChorePreconditions.CanPreempt, null);
		}
		chore_provider.AddChore(this);
	}

	public int id { get; private set; }

	public ChoreDriver driver { get; set; }

	public ChoreDriver lastDriver { get; set; }

	protected abstract StateMachine.Instance GetSMI();

	public ChoreType choreType { get; set; }

	public ChoreProvider provider { get; set; }

	public ChoreConsumer overrideTarget { get; private set; }

	public bool runUntilComplete { get; set; }

	public int masterPriority { get; set; }

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

	protected void SetPrioritizable(Prioritizable prioritizable)
	{
		if (prioritizable != null)
		{
			this.prioritizable = prioritizable;
			this.masterPriority = prioritizable.GetMasterPriority();
			prioritizable.onPriorityChanged = (Action<int>)Delegate.Combine(prioritizable.onPriorityChanged, new Action<int>(this.OnMasterPriorityChanged));
		}
	}

	private void ClearPrioritizable()
	{
		if (this.prioritizable != null)
		{
			Prioritizable prioritizable = this.prioritizable;
			prioritizable.onPriorityChanged = (Action<int>)Delegate.Remove(prioritizable.onPriorityChanged, new Action<int>(this.OnMasterPriorityChanged));
		}
	}

	private void OnMasterPriorityChanged(int priority)
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
		this.preconditions.Add(new Chore.PreconditionInstance
		{
			id = precondition.id,
			fn = precondition.fn,
			data = data
		});
	}

	public bool CheckPrecondition(bool is_valid, string condition_name)
	{
		return is_valid;
	}

	public virtual void CollectChores(ChoreConsumer consumer, List<Chore.Precondition.Context> contexts, bool is_attempting_override)
	{
		Chore.Precondition.Context context = new Chore.Precondition.Context(this, consumer, is_attempting_override, null);
		context.RunPreconditions();
		if (Chore.enableChoreDebugging || context.IsSuccess())
		{
			contexts.Add(context);
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
		DebugUtil.Assert(this.driver == null, "Assert!");
		DebugUtil.Assert(this.provider != null, "Assert!");
		this.driver = context.consumer.GetComponent<ChoreDriver>();
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

	protected virtual void Succeed(string reason)
	{
		if (!this.RemoveFromProvider())
		{
			return;
		}
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

	protected virtual void ShowCustomEditor(string filter)
	{
	}

	public const int MAX_PLAYER_MASTER_PRIORITY = 9;

	public const int MIN_PLAYER_MASTER_PRIORITY = 1;

	public const int DEFAULT_MASTER_PRIORITY = 5;

	public const int MAX_MASTER_PRIORITY = 10;

	public const int MIN_MASTER_PRIORITY = 0;

	private static int nextId;

	public static bool enableChoreDebugging;

	public bool isExpanded;

	public Action<Chore> onExit;

	private Action<Chore> onComplete;

	private Action<Chore> onBegin;

	private Action<Chore> onEnd;

	public Action<Chore> onCleanup;

	public bool debug;

	public List<Chore.PreconditionInstance> preconditions = new List<Chore.PreconditionInstance>();

	private Prioritizable prioritizable;

	public struct PreconditionInstance
	{
		public string id;

		public Chore.PreconditionFn fn;

		public object data;
	}

	public struct Precondition
	{
		public string id;

		public Chore.PreconditionFn fn;

		public struct Context : IComparable<Chore.Precondition.Context>, IEquatable<Chore.Precondition.Context>
		{
			public Context(Chore chore, ChoreConsumer consumer, bool is_attempting_override, object data = null)
			{
				this.masterPriority = chore.masterPriority;
				this.priority = 0;
				this.priorityMod = chore.priorityMod;
				this.interruptPriority = 0;
				this.cost = 0;
				this.chore = chore;
				this.consumer = consumer;
				this.failedPreconditionId = -1;
				this.isAttemptingOverride = is_attempting_override;
				this.data = data;
				this.SetPriority(chore);
			}

			public void Set(Chore chore, ChoreConsumer consumer, bool is_attempting_override, object data = null)
			{
				this.masterPriority = chore.masterPriority;
				this.priority = 0;
				this.priorityMod = chore.priorityMod;
				this.interruptPriority = 0;
				this.cost = 0;
				this.chore = chore;
				this.consumer = consumer;
				this.failedPreconditionId = -1;
				this.isAttemptingOverride = is_attempting_override;
				this.data = data;
				this.SetPriority(chore);
			}

			public void SetPriority(Chore chore)
			{
				this.priority = chore.choreType.priority;
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
				if (flag == flag2)
				{
					int num = this.masterPriority - obj.masterPriority;
					if (num != 0)
					{
						return num;
					}
					int num2 = this.priority - obj.priority;
					if (num2 != 0)
					{
						return num2;
					}
					int num3 = this.priorityMod - obj.priorityMod;
					if (num3 == 0)
					{
						return obj.cost - this.cost;
					}
					return num3;
				}
				else
				{
					if (flag)
					{
						return -1;
					}
					return 1;
				}
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

			public int masterPriority;

			public int priority;

			public int priorityMod;

			public int interruptPriority;

			public int cost;

			public Chore chore;

			public ChoreConsumer consumer;

			public int failedPreconditionId;

			public bool isAttemptingOverride;

			public object data;
		}
	}

	public delegate bool PreconditionFn(ref Chore.Precondition.Context context, object data);
}
