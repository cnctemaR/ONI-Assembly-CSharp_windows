using System;
using UnityEngine;

public class FetchChore : Chore<FetchChore.StatesInstance>
{
	public FetchChore(Storage destination, float amount, Tag[] tags, ChoreProvider chore_provider = null, bool run_until_complete = true, Action<Chore> on_complete = null, Action<Chore> on_begin = null, Action<Chore> on_end = null, bool only_when_operational = true)
		: base(destination.choreType, destination, chore_provider, run_until_complete, on_complete, on_begin, on_end, int.MaxValue, false, true)
	{
		if (amount <= 0f)
		{
			Output.LogError(new object[] { "Requesting an invalid FetchChore amount" });
		}
		base.SetPrioritizable(destination.GetComponent<Prioritizable>());
		this.smi = new FetchChore.StatesInstance(this);
		this.smi.sm.requestedamount.Set(amount, this.smi);
		this.smi.sm.destination.Set(destination, this.smi);
		this.tags = tags;
		if (destination.GetOnlyFetchMarkedItems())
		{
			this.requiredTags = new Tag[] { GameTags.Garbage };
		}
		base.AddPrecondition(ChorePreconditions.CanMoveTo, destination);
		base.AddPrecondition(FetchChore.IsFetchTargetAvailable, null);
		base.AddPrecondition(ChorePreconditions.IsMarkedForDeconstruction, this.target.gameObject);
		base.AddPrecondition(ChorePreconditions.IsMarkedForDisable, this.target.gameObject);
		if (only_when_operational && destination.gameObject.GetComponent<Operational>())
		{
			base.AddPrecondition(ChorePreconditions.IsOperational, destination.gameObject);
		}
		this.partitionerEntry = GameScenePartitioner.Instance.Add(destination.name, this, Grid.PosToCell(destination), GameScenePartitioner.Instance.fetchChores.mask, null);
		destination.onPriorityChanged = (global::System.Action)Delegate.Combine(destination.onPriorityChanged, new global::System.Action(this.OnPriorityChanged));
	}

	// Note: this type is marked as 'beforefieldinit'.
	static FetchChore()
	{
		Chore.Precondition precondition = default(Chore.Precondition);
		precondition.id = "IsFetchTargetAvailable";
		precondition.fn = delegate(ref Chore.Precondition.Context context, object data)
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
				flag = FetchManagerUpdater.IsFetchablePickup(pickupable.GetComponent<KPrefabID>(), pickupable.storage, pickupable.UnreservedAmount, pickupable.MinTakeAmount, fetchChore.originalAmount, fetchChore.tags, fetchChore.requiredTags, context.consumer.GetComponent<Storage>());
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
		};
		FetchChore.IsFetchTargetAvailable = precondition;
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
			FetchManager.Instance.FindFetchTarget(consumer.GetComponent<Worker>(), this.destination, this.tags, this.requiredTags, this.originalAmount, ref pickupable);
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
		KPrefabID component = pickupable.GetComponent<KPrefabID>();
		component.onTagsChanged = (global::System.Action)Delegate.Combine(component.onTagsChanged, new global::System.Action(this.OnTagsChanged));
		base.Begin(context);
	}

	protected override void End(string reason)
	{
		Pickupable pickupable = this.smi.sm.source.Get<Pickupable>(this.smi);
		if (pickupable != null)
		{
			KPrefabID component = pickupable.GetComponent<KPrefabID>();
			component.onTagsChanged = (global::System.Action)Delegate.Remove(component.onTagsChanged, new global::System.Action(this.OnTagsChanged));
		}
		base.End(reason);
	}

	private void OnTagsChanged()
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

	private void OnMasterPriorityChanged(int priority)
	{
		base.masterPriority = this.destination.GetComponent<Prioritizable>().GetMasterPriority();
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

	private GameScenePartitionerEntry partitionerEntry;

	public static Chore.Precondition IsFetchTargetAvailable;

	public class StatesInstance : GameStateMachine<FetchChore.States, FetchChore.StatesInstance, FetchChore>.GameInstance
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

		public StateMachine<FetchChore.States, FetchChore.StatesInstance, FetchChore>.TargetParameter fetcher;

		public StateMachine<FetchChore.States, FetchChore.StatesInstance, FetchChore>.TargetParameter source;

		public StateMachine<FetchChore.States, FetchChore.StatesInstance, FetchChore>.TargetParameter chunk;

		public StateMachine<FetchChore.States, FetchChore.StatesInstance, FetchChore>.TargetParameter destination;

		public StateMachine<FetchChore.States, FetchChore.StatesInstance, FetchChore>.FloatParameter requestedamount;

		public StateMachine<FetchChore.States, FetchChore.StatesInstance, FetchChore>.FloatParameter actualamount;
	}
}
