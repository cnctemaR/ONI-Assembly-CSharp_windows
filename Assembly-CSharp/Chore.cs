using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public abstract class Chore
{
	public abstract int id { get; protected set; }

	public abstract int priorityMod { get; protected set; }

	public abstract ChoreType choreType { get; protected set; }

	public abstract ChoreDriver driver { get; protected set; }

	public abstract ChoreDriver lastDriver { get; protected set; }

	public abstract bool isNull { get; }

	public abstract GameObject gameObject { get; }

	public abstract bool SatisfiesUrge(Urge urge);

	public abstract bool IsValid();

	public abstract IStateMachineTarget target { get; protected set; }

	public abstract bool isComplete { get; protected set; }

	public abstract bool IsPreemptable { get; protected set; }

	public abstract ChoreConsumer overrideTarget { get; protected set; }

	public abstract Prioritizable prioritizable { get; protected set; }

	public abstract ChoreProvider provider { get; set; }

	public abstract bool runUntilComplete { get; set; }

	public abstract bool isExpanded { get; protected set; }

	public abstract List<Chore.PreconditionInstance> GetPreconditions();

	public abstract bool CanPreempt(Chore.Precondition.Context context);

	public abstract void PrepareChore(ref Chore.Precondition.Context context);

	public abstract void Cancel(string reason);

	public abstract ReportManager.ReportType GetReportType();

	public abstract string GetReportName(string context = null);

	public abstract void AddPrecondition(Chore.Precondition precondition, object data = null);

	public abstract void CollectChores(ChoreConsumerState consumer_state, List<Chore.Precondition.Context> succeeded_contexts, List<Chore.Precondition.Context> incomplete_contexts, List<Chore.Precondition.Context> failed_contexts, bool is_attempting_override);

	public void CollectChores(ChoreConsumerState consumer_state, List<Chore.Precondition.Context> succeeded_contexts, List<Chore.Precondition.Context> failed_contexts, bool is_attempting_override)
	{
		this.CollectChores(consumer_state, succeeded_contexts, null, failed_contexts, is_attempting_override);
	}

	public abstract void Cleanup();

	public abstract void Fail(string reason);

	public abstract void Reserve(ChoreDriver reserver);

	public abstract void Begin(Chore.Precondition.Context context);

	public abstract bool InProgress();

	public virtual string ResolveString(string str)
	{
		return str;
	}

	public static int GetNextChoreID()
	{
		return ++Chore.nextId;
	}

	public PrioritySetting masterPriority;

	public bool showAvailabilityInHoverText = true;

	public Action<Chore> onExit;

	public Action<Chore> onComplete;

	private static int nextId;

	public const int MAX_PLAYER_BASIC_PRIORITY = 9;

	public const int MIN_PLAYER_BASIC_PRIORITY = 1;

	public const int MAX_PLAYER_HIGH_PRIORITY = 0;

	public const int MIN_PLAYER_HIGH_PRIORITY = 0;

	public const int MAX_PLAYER_EMERGENCY_PRIORITY = 1;

	public const int MIN_PLAYER_EMERGENCY_PRIORITY = 1;

	public const int DEFAULT_BASIC_PRIORITY = 5;

	public const int MAX_BASIC_PRIORITY = 10;

	public const int MIN_BASIC_PRIORITY = 0;

	public static bool ENABLE_PERSONAL_PRIORITIES = true;

	public static PrioritySetting DefaultPrioritySetting = new PrioritySetting(PriorityScreen.PriorityClass.basic, 5);

	public delegate bool PreconditionFn(ref Chore.Precondition.Context context, object data);

	public struct PreconditionInstance
	{
		public Chore.Precondition condition;

		public object data;
	}

	public struct Precondition
	{
		public string id;

		public string description;

		public int sortOrder;

		public Chore.PreconditionFn fn;

		public bool canExecuteOnAnyThread;

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
				this.skippedPreconditions = false;
				this.isAttemptingOverride = is_attempting_override;
				this.data = data;
				this.choreTypeForPermission = chore.choreType;
				this.skipMoreSatisfyingEarlyPrecondition = RootMenu.Instance != null && RootMenu.Instance.IsBuildingChorePanelActive();
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
				this.skippedPreconditions = false;
				this.isAttemptingOverride = is_attempting_override;
				this.data = data;
				this.choreTypeForPermission = chore.choreType;
				this.SetPriority(chore);
			}

			public void SetPriority(Chore chore)
			{
				this.priority = (Game.Instance.advancedPersonalPriorities ? chore.choreType.explicitPriority : chore.choreType.priority);
				this.priorityMod = chore.priorityMod;
				this.interruptPriority = chore.choreType.interruptPriority;
			}

			public bool IsSuccess()
			{
				return this.failedPreconditionId == -1 && !this.skippedPreconditions;
			}

			public bool IsComplete()
			{
				return !this.skippedPreconditions;
			}

			public bool IsPotentialSuccess()
			{
				if (this.IsSuccess())
				{
					return true;
				}
				if (this.chore.driver == this.consumerState.choreDriver)
				{
					return true;
				}
				if (this.failedPreconditionId != -1)
				{
					if (this.failedPreconditionId >= 0 && this.failedPreconditionId < this.chore.GetPreconditions().Count)
					{
						return this.chore.GetPreconditions()[this.failedPreconditionId].condition.id == ChorePreconditions.instance.IsMoreSatisfyingLate.id;
					}
					DebugUtil.DevLogErrorFormat("failedPreconditionId out of range {0}/{1}", new object[]
					{
						this.failedPreconditionId,
						this.chore.GetPreconditions().Count
					});
				}
				return false;
			}

			private void DoPreconditions(bool mainThreadOnly)
			{
				bool flag = Game.IsOnMainThread();
				List<Chore.PreconditionInstance> preconditions = this.chore.GetPreconditions();
				this.skippedPreconditions = false;
				int i = 0;
				while (i < preconditions.Count)
				{
					Chore.PreconditionInstance preconditionInstance = preconditions[i];
					if (preconditionInstance.condition.canExecuteOnAnyThread)
					{
						if (!mainThreadOnly)
						{
							goto IL_0043;
						}
					}
					else
					{
						if (flag)
						{
							goto IL_0043;
						}
						this.skippedPreconditions = true;
					}
					IL_006B:
					i++;
					continue;
					IL_0043:
					if (!preconditionInstance.condition.fn(ref this, preconditionInstance.data))
					{
						this.failedPreconditionId = i;
						this.skippedPreconditions = false;
						return;
					}
					goto IL_006B;
				}
			}

			public void RunPreconditions()
			{
				this.DoPreconditions(false);
			}

			public void FinishPreconditions()
			{
				this.DoPreconditions(true);
			}

			public int CompareTo(Chore.Precondition.Context obj)
			{
				bool flag = this.failedPreconditionId != -1;
				bool flag2 = obj.failedPreconditionId != -1;
				if (flag == flag2)
				{
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
					int num7 = obj.cost - this.cost;
					if (num7 != 0)
					{
						return num7;
					}
					if (this.chore == null && obj.chore == null)
					{
						return 0;
					}
					if (this.chore == null)
					{
						return -1;
					}
					if (obj.chore == null)
					{
						return 1;
					}
					return this.chore.id - obj.chore.id;
				}
				else
				{
					if (!flag)
					{
						return 1;
					}
					return -1;
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

			public static bool ShouldFilter(string filter, string text)
			{
				return !string.IsNullOrEmpty(filter) && (string.IsNullOrEmpty(text) || text.ToLower().IndexOf(filter) < 0);
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

			public bool skippedPreconditions;

			public object data;

			public bool isAttemptingOverride;

			public ChoreType choreTypeForPermission;

			public bool skipMoreSatisfyingEarlyPrecondition;
		}
	}
}
