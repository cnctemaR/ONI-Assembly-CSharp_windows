using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class FetchChore : Chore<FetchChore.StatesInstance>
{
	public FetchChore(ChoreType choreType, Storage destination, float amount, Tag[] tags, Tag[] required_tags = null, Tag[] forbidden_tags = null, ChoreProvider chore_provider = null, bool run_until_complete = true, Action<Chore> on_complete = null, Action<Chore> on_begin = null, Action<Chore> on_end = null, FetchOrder2.OperationalRequirement operational_requirement = FetchOrder2.OperationalRequirement.Operational, int priority_mod = 0, Tag[] chore_tags = null)
		: base(choreType, destination, chore_provider, run_until_complete, on_complete, on_begin, on_end, PriorityScreen.PriorityClass.basic, 5, false, true, priority_mod, chore_tags)
	{
		if (choreType == null)
		{
			Output.LogError("You must specify a chore type for fetching!");
		}
		if (amount <= 0f)
		{
			Output.LogError("Requesting an invalid FetchChore amount");
		}
		base.SetPrioritizable((!(destination.prioritizable != null)) ? destination.GetComponent<Prioritizable>() : destination.prioritizable);
		this.smi = new FetchChore.StatesInstance(this);
		this.smi.sm.requestedamount.Set(amount, this.smi);
		this.smi.sm.destination.Set(destination, this.smi);
		this.tags = tags;
		this.tagBits = new TagBits(tags);
		this.requiredTagBits = new TagBits(required_tags);
		this.forbiddenTagBits = new TagBits(forbidden_tags);
		this.tagBitsHash = this.tagBits.GetHashCode();
		DebugUtil.DevAssert(!this.tagBits.HasAny(ref FetchManager.disallowedTagBits), new object[] { "Fetch chore fetching invalid tags." });
		if (destination.GetOnlyFetchMarkedItems())
		{
			this.requiredTagBits.SetTag(GameTags.Garbage);
		}
		base.AddPrecondition(ChorePreconditions.instance.IsScheduledTime, Db.Get().ScheduleBlockTypes.Work);
		base.AddPrecondition(ChorePreconditions.instance.CanMoveTo, destination);
		base.AddPrecondition(FetchChore.IsFetchTargetAvailable, null);
		Deconstructable component = base.target.GetComponent<Deconstructable>();
		if (component != null)
		{
			base.AddPrecondition(ChorePreconditions.instance.IsNotMarkedForDeconstruction, component);
		}
		BuildingEnabledButton component2 = base.target.GetComponent<BuildingEnabledButton>();
		if (component2 != null)
		{
			base.AddPrecondition(ChorePreconditions.instance.IsNotMarkedForDisable, component2);
		}
		if (operational_requirement != FetchOrder2.OperationalRequirement.None && destination.gameObject.GetComponent<Operational>())
		{
			if (operational_requirement == FetchOrder2.OperationalRequirement.Operational)
			{
				Operational component3 = destination.GetComponent<Operational>();
				if (component3 != null)
				{
					base.AddPrecondition(ChorePreconditions.instance.IsOperational, component3);
				}
			}
			if (operational_requirement == FetchOrder2.OperationalRequirement.Functional)
			{
				Operational component4 = destination.GetComponent<Operational>();
				if (component4 != null)
				{
					base.AddPrecondition(ChorePreconditions.instance.IsFunctional, component4);
				}
			}
		}
		this.partitionerEntry = GameScenePartitioner.Instance.Add(destination.name, this, Grid.PosToCell(destination), GameScenePartitioner.Instance.fetchChoreLayer, null);
		destination.Subscribe(644822890, new Action<object>(this.OnOnlyFetchMarkedItemsSettingChanged));
		this.automatable = destination.GetComponent<Automatable>();
		if (this.automatable)
		{
			base.AddPrecondition(ChorePreconditions.instance.IsAllowedByAutomation, this.automatable);
		}
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

	public void FetchAreaBegin(Chore.Precondition.Context context, float amount_to_be_fetched)
	{
		this.amount = amount_to_be_fetched;
		this.smi.sm.fetcher.Set(context.consumerState.gameObject, this.smi);
		base.Begin(context);
	}

	public void FetchAreaEnd(ChoreDriver driver, Pickupable pickupable, bool is_success)
	{
		if (is_success)
		{
			this.fetchTarget = pickupable;
			base.driver = driver;
			this.fetcher = driver.gameObject;
			base.Succeed("FetchAreaEnd");
		}
		else
		{
			base.SetOverrideTarget(null);
			this.Fail("FetchAreaFail");
		}
	}

	public Pickupable FindFetchTarget(ChoreConsumerState consumer_state)
	{
		Pickupable pickupable = null;
		if (this.destination != null)
		{
			if (consumer_state.hasSolidTransferArm)
			{
				SolidTransferArm solidTransferArm = consumer_state.solidTransferArm;
				solidTransferArm.FindFetchTarget(this.destination, this.tagBits, this.requiredTagBits, this.forbiddenTagBits, this.originalAmount, ref pickupable);
			}
			else
			{
				pickupable = Game.Instance.fetchManager.FindFetchTarget(this.destination, ref this.tagBits, ref this.requiredTagBits, ref this.forbiddenTagBits, this.originalAmount);
			}
		}
		return pickupable;
	}

	public override void Begin(Chore.Precondition.Context context)
	{
		Pickupable pickupable = (Pickupable)context.data;
		if (pickupable == null)
		{
			pickupable = this.FindFetchTarget(context.consumerState);
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
		if (this.fetcher == null)
		{
			return this.originalAmount;
		}
		return this.amount;
	}

	private void OnOnlyFetchMarkedItemsSettingChanged(object data)
	{
		if (this.smi.sm.destination.Get<Storage>(this.smi).GetOnlyFetchMarkedItems())
		{
			this.requiredTagBits.SetTag(GameTags.Garbage);
		}
		else
		{
			this.requiredTagBits.Clear(GameTags.Garbage);
		}
	}

	private void OnMasterPriorityChanged(PriorityScreen.PriorityClass priorityClass, int priority_value)
	{
		this.masterPriority.priority_class = priorityClass;
		this.masterPriority.priority_value = priority_value;
	}

	public override void CollectChores(ChoreConsumerState consumer_state, List<Chore.Precondition.Context> succeeded_contexts, List<Chore.Precondition.Context> failed_contexts, bool is_attempting_override)
	{
	}

	public void CollectChoresFromGlobalChoreProvider(ChoreConsumerState consumer_state, List<Chore.Precondition.Context> succeeded_contexts, List<Chore.Precondition.Context> failed_contexts, bool is_attempting_override)
	{
		base.CollectChores(consumer_state, succeeded_contexts, failed_contexts, is_attempting_override);
	}

	public override void Cleanup()
	{
		base.Cleanup();
		GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
		Storage storage = this.smi.sm.destination.Get<Storage>(this.smi);
		if (storage != null)
		{
			storage.Unsubscribe(644822890, new Action<object>(this.OnOnlyFetchMarkedItemsSettingChanged));
		}
	}

	public Tag[] tags;

	public int tagBitsHash;

	public TagBits tagBits;

	public TagBits requiredTagBits;

	public TagBits forbiddenTagBits;

	public Automatable automatable;

	public bool allowMultifetch = true;

	private HandleVector<int>.Handle partitionerEntry;

	public static readonly Chore.Precondition IsFetchTargetAvailable = new Chore.Precondition
	{
		id = "IsFetchTargetAvailable",
		description = DUPLICANTS.CHORES.PRECONDITIONS.IS_FETCH_TARGET_AVAILABLE,
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			FetchChore fetchChore = (FetchChore)context.chore;
			Pickupable pickupable = (Pickupable)context.data;
			bool flag;
			if (pickupable == null)
			{
				pickupable = fetchChore.FindFetchTarget(context.consumerState);
				flag = pickupable != null;
			}
			else
			{
				flag = FetchManager.IsFetchablePickup(pickupable.KPrefabID, pickupable.storage, pickupable.UnreservedAmount, ref fetchChore.tagBits, ref fetchChore.requiredTagBits, ref fetchChore.forbiddenTagBits, context.consumerState.storage);
			}
			if (flag)
			{
				if (pickupable == null)
				{
					global::Debug.Log(string.Format("Failed to find fetch target for {0}", fetchChore.destination), null);
					return false;
				}
				context.data = pickupable;
				int num;
				if (context.consumerState.consumer.GetNavigationCost(pickupable, out num))
				{
					context.cost += num;
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
