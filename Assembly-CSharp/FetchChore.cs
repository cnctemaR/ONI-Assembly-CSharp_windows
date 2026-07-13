using System;
using System.Collections.Generic;
using System.Linq;
using STRINGS;
using UnityEngine;

public class FetchChore : Chore<FetchChore.StatesInstance>
{
	public float originalAmount
	{
		get
		{
			return base.smi.sm.requestedamount.Get(base.smi);
		}
	}

	public float amount
	{
		get
		{
			return base.smi.sm.actualamount.Get(base.smi);
		}
		set
		{
			base.smi.sm.actualamount.Set(value, base.smi, false);
		}
	}

	public Pickupable fetchTarget
	{
		get
		{
			return base.smi.sm.chunk.Get<Pickupable>(base.smi);
		}
		set
		{
			base.smi.sm.chunk.Set(value, base.smi);
		}
	}

	public GameObject fetcher
	{
		get
		{
			return base.smi.sm.fetcher.Get(base.smi);
		}
		set
		{
			base.smi.sm.fetcher.Set(value, base.smi, false);
		}
	}

	public Storage destination { get; private set; }

	public void FetchAreaBegin(Chore.Precondition.Context context, float amount_to_be_fetched)
	{
		this.amount = amount_to_be_fetched;
		base.smi.sm.fetcher.Set(context.consumerState.gameObject, base.smi, false);
		ReportManager.Instance.ReportValue(ReportManager.ReportType.ChoreStatus, 1f, context.chore.choreType.Name, GameUtil.GetChoreName(this, context.data));
		base.Begin(context);
	}

	public void FetchAreaEnd(ChoreDriver driver, Pickupable pickupable, bool is_success)
	{
		if (is_success)
		{
			ReportManager.Instance.ReportValue(ReportManager.ReportType.ChoreStatus, -1f, this.choreType.Name, GameUtil.GetChoreName(this, pickupable));
			this.fetchTarget = pickupable;
			this.driver = driver;
			this.fetcher = driver.gameObject;
			base.Succeed("FetchAreaEnd");
			SaveGame.Instance.ColonyAchievementTracker.LogFetchChore(this.fetcher, this.choreType);
			return;
		}
		base.SetOverrideTarget(null);
		this.Fail("FetchAreaFail");
	}

	public Pickupable FindFetchTarget(ChoreConsumerState consumer_state)
	{
		if (!(this.destination != null))
		{
			return null;
		}
		if (consumer_state.hasSolidTransferArm)
		{
			return consumer_state.solidTransferArm.FindFetchTarget(this.destination, this);
		}
		return Game.Instance.fetchManager.FindFetchTarget(this.destination, this);
	}

	public override void Begin(Chore.Precondition.Context context)
	{
		Pickupable pickupable = (Pickupable)context.data;
		if (pickupable == null)
		{
			pickupable = this.FindFetchTarget(context.consumerState);
		}
		base.smi.sm.source.Set(pickupable.gameObject, base.smi, false);
		pickupable.Subscribe(-1582839653, new Action<object>(this.OnTagsChanged));
		base.Begin(context);
	}

	protected override void End(string reason)
	{
		Pickupable pickupable = base.smi.sm.source.Get<Pickupable>(base.smi);
		if (pickupable != null)
		{
			pickupable.Unsubscribe(-1582839653, new Action<object>(this.OnTagsChanged));
		}
		base.End(reason);
	}

	private void OnTagsChanged(object _)
	{
		if (base.smi.sm.chunk.Get(base.smi) != null)
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

	public static float GetMinimumFetchAmount(HashSet<Tag> match_tags)
	{
		float num = 1f;
		foreach (Tag tag in match_tags)
		{
			GameObject prefab = Assets.GetPrefab(tag);
			if (prefab != null)
			{
				PrimaryElement component = prefab.GetComponent<PrimaryElement>();
				if (component != null && component.MassPerUnit > 1f)
				{
					num = Mathf.Max(num, component.MassPerUnit);
				}
			}
			else
			{
				foreach (GameObject gameObject in Assets.GetPrefabsWithTag(tag))
				{
					PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
					if (component2 != null && component2.MassPerUnit > 1f)
					{
						num = Mathf.Max(num, component2.MassPerUnit);
					}
				}
			}
		}
		return num;
	}

	public static float GetMinimumFetchAmount(Tag requested_tag, float requested_amount)
	{
		float num = requested_amount;
		GameObject gameObject = Assets.TryGetPrefab(requested_tag);
		if (gameObject != null)
		{
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			if (component != null && component.MassPerUnit > 1f)
			{
				return Mathf.Max(num, component.MassPerUnit);
			}
		}
		foreach (GameObject gameObject2 in Assets.GetPrefabsWithTag(requested_tag))
		{
			PrimaryElement component2 = gameObject2.GetComponent<PrimaryElement>();
			if (component2 != null && component2.MassPerUnit > 1f)
			{
				num = Mathf.Max(num, component2.MassPerUnit);
			}
		}
		return num;
	}

	public FetchChore(ChoreType choreType, Storage destination, float amount, HashSet<Tag> tags, FetchChore.MatchCriteria criteria, Tag required_tag, Tag[] forbidden_tags = null, ChoreProvider chore_provider = null, bool run_until_complete = true, Action<Chore> on_complete = null, Action<Chore> on_begin = null, Action<Chore> on_end = null, Operational.State operational_requirement = Operational.State.Operational, int priority_mod = 0)
		: base(choreType, destination, chore_provider, run_until_complete, on_complete, on_begin, on_end, PriorityScreen.PriorityClass.basic, 5, false, true, priority_mod, false, ReportManager.ReportType.WorkTime)
	{
		if (choreType == null)
		{
			global::Debug.LogError("You must specify a chore type for fetching!");
		}
		this.tagsFirst = ((tags.Count > 0) ? tags.First<Tag>() : Tag.Invalid);
		if (amount <= PICKUPABLETUNING.MINIMUM_PICKABLE_AMOUNT)
		{
			DebugUtil.LogWarningArgs(new object[] { string.Format("Chore {0} is requesting {1} {2} to {3}", new object[]
			{
				choreType.Id,
				this.tagsFirst,
				amount,
				(destination != null) ? destination.name : "to nowhere"
			}) });
		}
		base.SetPrioritizable((destination.prioritizable != null) ? destination.prioritizable : destination.GetComponent<Prioritizable>());
		base.smi = new FetchChore.StatesInstance(this);
		base.smi.sm.requestedamount.Set(amount, base.smi, false);
		this.destination = destination;
		DebugUtil.DevAssert(criteria != FetchChore.MatchCriteria.MatchTags || tags.Count <= 1, "For performance reasons fetch chores are limited to one tag when matching tags!", null);
		this.tags = tags;
		this.criteria = criteria;
		this.tagsHash = FetchChore.ComputeHashCodeForTags(tags);
		this.requiredTag = required_tag;
		this.forbiddenTags = ((forbidden_tags != null) ? forbidden_tags : new Tag[0]);
		this.forbidHash = FetchChore.ComputeHashCodeForTags(this.forbiddenTags);
		DebugUtil.DevAssert(!tags.Contains(GameTags.Preserved), "Fetch chore fetching invalid tags.", null);
		if (destination.GetOnlyFetchMarkedItems())
		{
			DebugUtil.DevAssert(!this.requiredTag.IsValid, "Only one requiredTag is supported at a time, this will stomp!", null);
			this.requiredTag = GameTags.Garbage;
		}
		this.AddPrecondition(ChorePreconditions.instance.IsScheduledTime, Db.Get().ScheduleBlockTypes.Work);
		this.AddPrecondition(ChorePreconditions.instance.CanMoveTo, destination);
		this.AddPrecondition(FetchChore.IsFetchTargetAvailable, null);
		this.AddPrecondition(FetchChore.CanFetchDroneComplete, destination.gameObject);
		Deconstructable component = this.target.GetComponent<Deconstructable>();
		if (component != null)
		{
			this.AddPrecondition(ChorePreconditions.instance.IsNotMarkedForDeconstruction, component);
		}
		BuildingEnabledButton component2 = this.target.GetComponent<BuildingEnabledButton>();
		if (component2 != null)
		{
			this.AddPrecondition(ChorePreconditions.instance.IsNotMarkedForDisable, component2);
		}
		if (operational_requirement != Operational.State.None)
		{
			Operational component3 = destination.GetComponent<Operational>();
			if (component3 != null)
			{
				Chore.Precondition precondition = ChorePreconditions.instance.IsOperational;
				if (operational_requirement == Operational.State.Functional)
				{
					precondition = ChorePreconditions.instance.IsFunctional;
				}
				this.AddPrecondition(precondition, component3);
			}
		}
		this.partitionerEntry = GameScenePartitioner.Instance.Add(destination.name, this, Grid.PosToCell(destination), GameScenePartitioner.Instance.fetchChoreLayer, null);
		this.onOnlyFetchMarkedItemsSettingChangedHandle = destination.Subscribe(644822890, new Action<object>(this.OnOnlyFetchMarkedItemsSettingChanged));
		this.automatable = destination.GetComponent<Automatable>();
		if (this.automatable)
		{
			this.AddPrecondition(ChorePreconditions.instance.IsAllowedByAutomation, this.automatable);
		}
	}

	private void OnOnlyFetchMarkedItemsSettingChanged(object data)
	{
		if (this.destination != null)
		{
			if (this.destination.GetOnlyFetchMarkedItems())
			{
				DebugUtil.DevAssert(!this.requiredTag.IsValid, "Only one requiredTag is supported at a time, this will stomp!", null);
				this.requiredTag = GameTags.Garbage;
				return;
			}
			this.requiredTag = Tag.Invalid;
		}
	}

	private void OnMasterPriorityChanged(PriorityScreen.PriorityClass priorityClass, int priority_value)
	{
		this.masterPriority.priority_class = priorityClass;
		this.masterPriority.priority_value = priority_value;
	}

	public override void CollectChores(ChoreConsumerState consumer_state, List<Chore.Precondition.Context> succeeded_contexts, List<Chore.Precondition.Context> incomplete_contexts, List<Chore.Precondition.Context> failed_contexts, bool is_attempting_override)
	{
	}

	public void CollectChoresFromGlobalChoreProvider(ChoreConsumerState consumer_state, List<Chore.Precondition.Context> succeeded_contexts, List<Chore.Precondition.Context> failed_contexts, bool is_attempting_override)
	{
		this.CollectChoresFromGlobalChoreProvider(consumer_state, succeeded_contexts, null, failed_contexts, is_attempting_override);
	}

	public void CollectChoresFromGlobalChoreProvider(ChoreConsumerState consumer_state, List<Chore.Precondition.Context> succeeded_contexts, List<Chore.Precondition.Context> incomplete_contexts, List<Chore.Precondition.Context> failed_contexts, bool is_attempting_override)
	{
		base.CollectChores(consumer_state, succeeded_contexts, incomplete_contexts, failed_contexts, is_attempting_override);
	}

	public override void Cleanup()
	{
		base.Cleanup();
		GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
		if (this.destination != null)
		{
			this.destination.Unsubscribe(ref this.onOnlyFetchMarkedItemsSettingChangedHandle);
		}
	}

	public static int ComputeHashCodeForTags(IEnumerable<Tag> tags)
	{
		int num = 0;
		foreach (Tag tag in tags)
		{
			num ^= tag.GetHash();
		}
		return num;
	}

	public HashSet<Tag> tags;

	public Tag tagsFirst;

	public FetchChore.MatchCriteria criteria;

	public int tagsHash;

	public bool validateRequiredTagOnTagChange;

	public Tag requiredTag;

	public Tag[] forbiddenTags;

	public int forbidHash;

	public Automatable automatable;

	public bool allowMultifetch = true;

	private HandleVector<int>.Handle partitionerEntry;

	private int onOnlyFetchMarkedItemsSettingChangedHandle = -1;

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
				flag = FetchManager.IsFetchablePickup(pickupable, fetchChore, context.consumerState.storage);
			}
			if (flag)
			{
				if (pickupable == null)
				{
					global::Debug.Log(string.Format("Failed to find fetch target for {0}", fetchChore.destination));
					return false;
				}
				context.data = pickupable;
				int num;
				if (context.consumerState.worker.IsFetchDrone())
				{
					if ((pickupable.targetWorkable == null || pickupable.targetWorkable.GetComponent<Pickupable>() != null) && context.consumerState.consumer.GetNavigationCost(pickupable, out num))
					{
						context.cost += num;
						return true;
					}
				}
				else if (context.consumerState.consumer.GetNavigationCost(pickupable, out num))
				{
					context.cost += num;
					return true;
				}
			}
			return false;
		},
		canExecuteOnAnyThread = false
	};

	public static readonly Chore.Precondition CanFetchDroneComplete = new Chore.Precondition
	{
		id = "CanFetchDroneComplete",
		description = DUPLICANTS.CHORES.PRECONDITIONS.CAN_FETCH_DRONE_COMPLETE_FETCH,
		canExecuteOnAnyThread = false,
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			if (!context.consumerState.worker.IsFetchDrone())
			{
				return true;
			}
			FetchChore fetchChore2 = (FetchChore)context.chore;
			Pickupable pickupable2 = (Pickupable)context.data;
			bool flag2;
			if (pickupable2 == null)
			{
				pickupable2 = fetchChore2.FindFetchTarget(context.consumerState);
				flag2 = pickupable2 != null;
			}
			else
			{
				flag2 = FetchManager.IsFetchablePickup(pickupable2, fetchChore2, context.consumerState.storage);
			}
			return flag2 && !((GameObject)data == context.consumerState.gameObject) && ((pickupable2.targetWorkable == null || pickupable2.targetWorkable as Pickupable != null) && context.consumerState.consumer.navigator.CanReach(pickupable2.cachedCell, pickupable2.GetOffsets()));
		}
	};

	public enum MatchCriteria
	{
		MatchID,
		MatchTags
	}

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

		public StateMachine<FetchChore.States, FetchChore.StatesInstance, FetchChore, object>.FloatParameter requestedamount;

		public StateMachine<FetchChore.States, FetchChore.StatesInstance, FetchChore, object>.FloatParameter actualamount;
	}
}
