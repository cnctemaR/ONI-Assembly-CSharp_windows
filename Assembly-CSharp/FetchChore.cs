using System;
using UnityEngine;

public class FetchChore : Chore<FetchChore.StatesInstance>
{
	public FetchChore(Storage destination, float amount, Tag[] tags, Tag[] forbidden_tags = null, ChoreProvider chore_provider = null, bool run_until_complete = true, Action<Chore> on_complete = null, Action<Chore> on_begin = null, Action<Chore> on_end = null, FetchOrder2.OperationalRequirement operational_requirement = FetchOrder2.OperationalRequirement.Operational, int priority_mod = 0)
	{
		ChoreType choreType = destination.choreType;
		base..ctor(choreType, destination, chore_provider, run_until_complete, on_complete, on_begin, on_end, PriorityScreen.PriorityClass.basic, int.MaxValue, false, true, priority_mod);
		if (amount <= 0f)
		{
			Output.LogError(new object[] { "Requesting an invalid FetchChore amount" });
		}
		base.SetPrioritizable(destination.GetComponent<Prioritizable>());
		this.smi = new FetchChore.StatesInstance(this);
		this.smi.sm.requestedamount.Set(amount, this.smi);
		this.smi.sm.destination.Set(destination, this.smi);
		this.tags = tags;
		this.forbiddenTags = forbidden_tags;
		if (destination.GetOnlyFetchMarkedItems())
		{
			this.requiredTags = new Tag[] { GameTags.Garbage };
		}
		base.AddPrecondition(ChorePreconditions.CanMoveTo, destination);
		base.AddPrecondition(FetchChore.IsFetchTargetAvailable, null);
		base.AddPrecondition(ChorePreconditions.IsMarkedForDeconstruction, this.target.gameObject);
		base.AddPrecondition(ChorePreconditions.IsMarkedForDisable, this.target.gameObject);
		if (operational_requirement != FetchOrder2.OperationalRequirement.None && destination.gameObject.GetComponent<Operational>())
		{
			if (operational_requirement == FetchOrder2.OperationalRequirement.Operational)
			{
				base.AddPrecondition(ChorePreconditions.IsOperational, destination.gameObject);
			}
			if (operational_requirement == FetchOrder2.OperationalRequirement.Functional)
			{
				base.AddPrecondition(ChorePreconditions.IsFunctional, destination.gameObject);
			}
		}
		this.partitionerEntry = GameScenePartitioner.Instance.Add(destination.name, this, Grid.PosToCell(destination), GameScenePartitioner.Instance.fetchChoreLayer, null);
		destination.onPriorityChanged = (global::System.Action)Delegate.Combine(destination.onPriorityChanged, new global::System.Action(this.OnPriorityChanged));
	}

	public float originalAmount
	{
		get
		{
			return this.smi.sm.requestedamount.Get(this.smi);
		}
	}

	public float amount
	{
		get
		{
			return this.smi.sm.actualamount.Get(this.smi);
		}
		set
		{
			this.smi.sm.actualamount.Set(value, this.smi);
		}
	}

	public Pickupable fetchTarget
	{
		get
		{
			return this.smi.sm.chunk.Get<Pickupable>(this.smi);
		}
		set
		{
			this.smi.sm.chunk.Set(value, this.smi);
		}
	}

	public GameObject fetcher
	{
		get
		{
			return this.smi.sm.fetcher.Get(this.smi);
		}
		set
		{
			this.smi.sm.fetcher.Set(value, this.smi);
		}
	}

	public Storage destination
	{
		get
		{
			return this.smi.sm.destination.Get<Storage>(this.smi);
		}
	}

	public Tag[] tags { get; private set; }

	public Tag[] requiredTags { get; private set; }

	public Tag[] forbiddenTags { get; private set; }

	public void FetchAreaBegin(Chore.Precondition.Context context, float amount_to_be_fetched)
	{
		this.amount = amount_to_be_fetched;
		this.smi.sm.fetcher.Set(context.consumer.gameObject, this.smi);
		base.Begin(context);
	}

	public void FetchAreaEnd(ChoreDriver driver, Pickupable pickupable, bool is_success)
	{
		if (is_success)
		{
			this.fetchTarget = pickupable;
			base.driver = driver;
			this.fetcher = driver.gameObject;
			this.Succeed("FetchAreaEnd");
		}
		else
		{
			base.SetOverrideTarget(null);
			this.Fail("FetchAreaFail");
		}
	}

	public Pickupable FindFetchTarget(ChoreConsumer consumer)
	{
		Pickupable pickupable = null;
		if (this.destination != null)
		{
			FetchManager.Instance.FindFetchTarget(consumer.GetComponent<Worker>(), this.destination, this.tags, this.requiredTags, this.forbiddenTags, this.originalAmount, ref pickupable);
		}
		return pickupable;
	}

	public override void Begin(Chore.Precondition.Context context)
	{
		Pickupable pickupable = (Pickupable)context.data;
		if (pickupable == null)
		{
			pickupable = this.FindFetchTarget(context.consumer);
		}
		this.smi.sm.source.Set(pickupable.gameObject, this.smi);
		pickupable.Subscribe(-1582839653, new Action<object>(this.OnTagsChanged));
		base.Begin(context);
	}

	protected override void End(string reason)
	{
		Pickupable pickupable = this.smi.sm.source.Get<Pickupable>(this.smi);
		if (pickupable != null)
		{
			pickupable.Unsubscribe(-1582839653, new Action<object>(this.OnTagsChanged));
		}
		base.End(reason);
	}

	private void OnTagsChanged(object data)
	{
		if (this.smi.sm.chunk.Get(this.smi) != null)
		{
			this.Fail("Tags changed");
		}
	}

	public override void PrepareChore(ref Chore.Precondition.Context context)
	{
		context.chore = new FetchAreaChore(context);
	}

	public float AmountWaitingToFetch()
	{
		float num;
		if (this.fetcher == null)
		{
			num = this.originalAmount;
		}
		else
		{
			num = this.amount;
		}
		return num;
	}

	private void OnPriorityChanged()
	{
		if (this.smi.sm.destination.Get<Storage>(this.smi).GetOnlyFetchMarkedItems())
		{
			this.requiredTags = new Tag[] { GameTags.Garbage };
		}
		else
		{
			this.requiredTags = null;
		}
	}

	private void OnMasterPriorityChanged(PriorityScreen.PriorityClass priorityClass, int priority_value)
	{
		this.masterPriority.priority_class = priorityClass;
		this.masterPriority.priority_value = priority_value;
	}

	public override void Cleanup()
	{
		base.Cleanup();
		this.partitionerEntry.Release();
		Storage storage = this.smi.sm.destination.Get<Storage>(this.smi);
		if (storage != null)
		{
			Storage storage2 = storage;
			storage2.onPriorityChanged = (global::System.Action)Delegate.Remove(storage2.onPriorityChanged, new global::System.Action(this.OnPriorityChanged));
		}
	}

	public void RefreshChoreType()
	{
		base.choreType = this.smi.sm.destination.Get<Storage>(this.smi).choreType;
	}

	public bool allowMultifetch = true;

	private GameScenePartitionerEntry partitionerEntry;

	public static Chore.Precondition IsFetchTargetAvailable = new Chore.Precondition
	{
		id = "IsFetchTargetAvailable",
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			FetchChore fetchChore = (FetchChore)context.chore;
			Pickupable pickupable = (Pickupable)context.data;
			bool flag;
			if (pickupable == null)
			{
				pickupable = fetchChore.FindFetchTarget(context.consumer);
				flag = pickupable != null;
			}
			else
			{
				flag = FetchManagerUpdater.IsFetchablePickup(pickupable.GetComponent<KPrefabID>(), pickupable.storage, pickupable.UnreservedAmount, pickupable.MinTakeAmount, fetchChore.originalAmount, fetchChore.tags, fetchChore.requiredTags, fetchChore.forbiddenTags, context.consumer.GetComponent<Storage>());
			}
			if (flag)
			{
				context.data = pickupable;
				int navigationCost = context.consumer.GetComponent<Navigator>().GetNavigationCost(pickupable);
				if (navigationCost != PathProber.InvalidCost)
				{
					context.cost += navigationCost;
					return true;
				}
			}
			return false;
		}
	};

	public class StatesInstance : GameStateMachine<FetchChore.States, FetchChore.StatesInstance, FetchChore, object>.GameInstance
	{
		public StatesInstance(FetchChore master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<FetchChore.States, FetchChore.StatesInstance, FetchChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.root;
		}

		public StateMachine<FetchChore.States, FetchChore.StatesInstance, FetchChore, object>.TargetParameter fetcher;

		public StateMachine<FetchChore.States, FetchChore.StatesInstance, FetchChore, object>.TargetParameter source;

		public StateMachine<FetchChore.States, FetchChore.StatesInstance, FetchChore, object>.TargetParameter chunk;

		public StateMachine<FetchChore.States, FetchChore.StatesInstance, FetchChore, object>.TargetParameter destination;

		public StateMachine<FetchChore.States, FetchChore.StatesInstance, FetchChore, object>.FloatParameter requestedamount;

		public StateMachine<FetchChore.States, FetchChore.StatesInstance, FetchChore, object>.FloatParameter actualamount;
	}
}
