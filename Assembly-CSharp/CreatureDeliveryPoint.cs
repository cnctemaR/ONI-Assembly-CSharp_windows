using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;
using UnityEngine;

public class CreatureDeliveryPoint : StateMachineComponent<CreatureDeliveryPoint.SMInstance>
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.fetches = new List<FetchOrder2>();
		base.Subscribe(360192579, new Action<object>(this.OnBuildingStrawChanged));
		TreeFilterable component = base.GetComponent<TreeFilterable>();
		component.OnFilterChanged = (Action<HashSet<Tag>>)Delegate.Combine(component.OnFilterChanged, new Action<HashSet<Tag>>(this.OnFilterChanged));
		base.GetComponent<Storage>().SetOffsets(this.deliveryOffsets);
		Prioritizable.AddRef(base.gameObject);
	}

	private void OnBuildingStrawChanged(object o)
	{
		BuildingPointStraw buildingPointStraw = (BuildingPointStraw)o;
		this.spawnOffset = buildingPointStraw.GetBottomCellOffset();
		this.largeCritterSpawnOffset = new CellOffset(0, this.spawnOffset.y - 1);
		this.animSuffix = buildingPointStraw.GetAnimSuffix();
		StateMachine.BaseState currentState = base.smi.GetCurrentState();
		if (currentState == null)
		{
			return;
		}
		if (currentState == base.smi.sm.operational.interact_pre || currentState == base.smi.sm.operational.interact_pst)
		{
			return;
		}
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		if (currentState == base.smi.sm.unoperational)
		{
			component.Play("off" + this.animSuffix, KAnim.PlayMode.Once, 1f, 0f);
			return;
		}
		component.Play("on" + this.animSuffix, KAnim.PlayMode.Once, 1f, 0f);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		base.Subscribe<CreatureDeliveryPoint>(-905833192, CreatureDeliveryPoint.OnCopySettingsDelegate);
		base.Subscribe<CreatureDeliveryPoint>(643180843, CreatureDeliveryPoint.RefreshCreatureCountDelegate);
		this.critterCapacity = base.GetComponent<BaggableCritterCapacityTracker>();
		BaggableCritterCapacityTracker baggableCritterCapacityTracker = this.critterCapacity;
		baggableCritterCapacityTracker.onCountChanged = (global::System.Action)Delegate.Combine(baggableCritterCapacityTracker.onCountChanged, new global::System.Action(this.RebalanceFetches));
		this.critterCapacity.RefreshCreatureCount(null);
		this.logicPorts = base.GetComponent<LogicPorts>();
		if (this.logicPorts != null)
		{
			this.logicPorts.Subscribe(-801688580, new Action<object>(this.OnLogicChanged));
		}
	}

	private void OnLogicChanged(object data)
	{
		LogicValueChanged logicValueChanged = (LogicValueChanged)data;
		if (logicValueChanged.portID == "CritterDropOffInput")
		{
			if (logicValueChanged.newValue > 0)
			{
				this.RebalanceFetches();
				return;
			}
			this.ClearFetches();
		}
	}

	[Obsolete]
	[OnDeserialized]
	private void OnDeserialized()
	{
		if (this.critterCapacity != null && this.creatureLimit > 0)
		{
			this.critterCapacity.creatureLimit = this.creatureLimit;
			this.creatureLimit = -1;
		}
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		if (gameObject == null)
		{
			return;
		}
		if (gameObject.GetComponent<CreatureDeliveryPoint>() == null)
		{
			return;
		}
		this.RebalanceFetches();
	}

	private void OnFilterChanged(HashSet<Tag> tags)
	{
		this.ClearFetches();
		this.RebalanceFetches();
	}

	private void ClearFetches()
	{
		for (int i = this.fetches.Count - 1; i >= 0; i--)
		{
			this.fetches[i].Cancel("clearing all fetches");
		}
		this.fetches.Clear();
	}

	private void RebalanceFetches()
	{
		if (!this.LogicEnabled())
		{
			return;
		}
		if (!CreatureDeliveryPoint.States.ShouldBeOn(base.smi))
		{
			return;
		}
		HashSet<Tag> tags = base.GetComponent<TreeFilterable>().GetTags();
		ChoreType creatureFetch = Db.Get().ChoreTypes.CreatureFetch;
		Storage component = base.GetComponent<Storage>();
		int num = this.critterCapacity.creatureLimit - this.critterCapacity.storedCreatureCount;
		int count = this.fetches.Count;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		for (int i = this.fetches.Count - 1; i >= 0; i--)
		{
			if (this.fetches[i].IsComplete())
			{
				this.fetches.RemoveAt(i);
				num2++;
			}
		}
		int num6 = 0;
		for (int j = 0; j < this.fetches.Count; j++)
		{
			if (!this.fetches[j].InProgress)
			{
				num6++;
			}
		}
		if (num6 == 0 && this.fetches.Count < num)
		{
			float minimumFetchAmount = FetchChore.GetMinimumFetchAmount(tags);
			FetchOrder2 fetchOrder = new FetchOrder2(creatureFetch, tags, FetchChore.MatchCriteria.MatchID, GameTags.Creatures.Deliverable, null, component, minimumFetchAmount, Operational.State.Operational, 0);
			fetchOrder.validateRequiredTagOnTagChange = true;
			fetchOrder.Submit(new Action<FetchOrder2, Pickupable>(this.OnFetchComplete), false, new Action<FetchOrder2, Pickupable>(this.OnFetchBegun));
			this.fetches.Add(fetchOrder);
			num3++;
		}
		int num7 = this.fetches.Count - num;
		for (int k = this.fetches.Count - 1; k >= 0; k--)
		{
			if (num7 <= 0)
			{
				break;
			}
			if (!this.fetches[k].InProgress)
			{
				this.fetches[k].Cancel("fewer creatures in room");
				this.fetches.RemoveAt(k);
				num7--;
				num4++;
			}
		}
		while (num7 > 0 && this.fetches.Count > 0)
		{
			this.fetches[this.fetches.Count - 1].Cancel("fewer creatures in room");
			this.fetches.RemoveAt(this.fetches.Count - 1);
			num7--;
			num5++;
		}
	}

	private void OnFetchComplete(FetchOrder2 fetchOrder, Pickupable fetchedItem)
	{
		this.RebalanceFetches();
	}

	private void OnFetchBegun(FetchOrder2 fetchOrder, Pickupable fetchedItem)
	{
		this.RebalanceFetches();
	}

	protected override void OnCleanUp()
	{
		base.smi.StopSM("OnCleanUp");
		TreeFilterable component = base.GetComponent<TreeFilterable>();
		component.OnFilterChanged = (Action<HashSet<Tag>>)Delegate.Remove(component.OnFilterChanged, new Action<HashSet<Tag>>(this.OnFilterChanged));
		base.OnCleanUp();
	}

	public bool LogicEnabled()
	{
		return this.logicPorts == null || !this.logicPorts.IsPortConnected("CritterDropOffInput") || this.logicPorts.GetInputValue("CritterDropOffInput") == 1;
	}

	[MyCmpAdd]
	private Prioritizable prioritizable;

	[MyCmpReq]
	public BaggableCritterCapacityTracker critterCapacity;

	[Obsolete]
	[Serialize]
	private int creatureLimit = 20;

	public CellOffset[] deliveryOffsets = new CellOffset[1];

	public CellOffset spawnOffset = new CellOffset(0, 0);

	public CellOffset largeCritterSpawnOffset = new CellOffset(0, 0);

	private List<FetchOrder2> fetches;

	public bool playAnimsOnFetch;

	public string animSuffix = "";

	private LogicPorts logicPorts;

	private static readonly EventSystem.IntraObjectHandler<CreatureDeliveryPoint> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<CreatureDeliveryPoint>(delegate(CreatureDeliveryPoint component, object data)
	{
		component.OnCopySettings(data);
	});

	private static readonly EventSystem.IntraObjectHandler<CreatureDeliveryPoint> RefreshCreatureCountDelegate = new EventSystem.IntraObjectHandler<CreatureDeliveryPoint>(delegate(CreatureDeliveryPoint component, object data)
	{
		component.critterCapacity.RefreshCreatureCount(data);
	});

	public class SMInstance : GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.GameInstance
	{
		public bool IsOperational
		{
			get
			{
				return this.operational != null && this.operational.IsOperational;
			}
		}

		public bool IsStrawInstalled
		{
			get
			{
				return this.straw != null;
			}
		}

		public bool IsStrawOutsideLiquid
		{
			get
			{
				return this.IsStrawInstalled && !this.straw.isInLiquid;
			}
		}

		public bool IsStrawBlocked
		{
			get
			{
				return this.IsStrawInstalled && this.straw.currentDepth <= 0;
			}
		}

		public SMInstance(CreatureDeliveryPoint master)
			: base(master)
		{
			this.operational = base.GetComponent<Operational>();
			this.straw = base.GetComponent<BuildingPointStraw>();
		}

		public bool isDroppingAllCreatures;

		private Operational operational;

		private BuildingPointStraw straw;
	}

	public class States : GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.operational.waiting;
			this.root.Update("RefreshCreatureCount", delegate(CreatureDeliveryPoint.SMInstance smi, float dt)
			{
				smi.master.critterCapacity.RefreshCreatureCount(null);
			}, UpdateRate.SIM_1000ms, false);
			this.root.EventHandler(GameHashes.OnStorageChange, delegate(CreatureDeliveryPoint.SMInstance smi)
			{
				if (!smi.master.playAnimsOnFetch)
				{
					CreatureDeliveryPoint.States.DropAllCreatures(smi);
				}
			});
			this.unoperational.PlayAnim("off", KAnim.PlayMode.Once, (CreatureDeliveryPoint.SMInstance smi) => smi.master.animSuffix).EventTransition(GameHashes.LogicEvent, this.operational, new StateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.Transition.ConditionCallback(CreatureDeliveryPoint.States.ShouldBeOn)).EventTransition(GameHashes.OperationalChanged, this.operational, new StateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.Transition.ConditionCallback(CreatureDeliveryPoint.States.ShouldBeOn))
				.EventTransition(GameHashes.BuildingStrawChange, this.operational, new StateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.Transition.ConditionCallback(CreatureDeliveryPoint.States.ShouldBeOn))
				.Enter(new StateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.State.Callback(CreatureDeliveryPoint.States.ClearFetches))
				.DefaultState(this.unoperational.noOperational);
			this.unoperational.noOperational.EventTransition(GameHashes.OperationalChanged, this.unoperational.strawBlocked, (CreatureDeliveryPoint.SMInstance smi) => CreatureDeliveryPoint.States.IsOperational(smi) && CreatureDeliveryPoint.States.IsStrawBlocked(smi)).EventTransition(GameHashes.OperationalChanged, this.unoperational.noLiquidOnStraw, (CreatureDeliveryPoint.SMInstance smi) => CreatureDeliveryPoint.States.IsOperational(smi) && CreatureDeliveryPoint.States.IsStrawOutsideLiquid(smi));
			this.unoperational.strawBlocked.ToggleStatusItem(Db.Get().BuildingStatusItems.OutputTileBlocked, null).EventTransition(GameHashes.OperationalChanged, this.unoperational.noOperational, GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.Not(new StateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.Transition.ConditionCallback(CreatureDeliveryPoint.States.IsOperational))).EventTransition(GameHashes.BuildingStrawChange, this.unoperational.noLiquidOnStraw, (CreatureDeliveryPoint.SMInstance smi) => !CreatureDeliveryPoint.States.IsStrawBlocked(smi) && CreatureDeliveryPoint.States.IsStrawOutsideLiquid(smi));
			this.unoperational.noLiquidOnStraw.ToggleStatusItem(Db.Get().BuildingStatusItems.NotSubmerged, null).EventTransition(GameHashes.OperationalChanged, this.unoperational.noOperational, GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.Not(new StateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.Transition.ConditionCallback(CreatureDeliveryPoint.States.IsOperational))).EventTransition(GameHashes.BuildingStrawChange, this.unoperational.strawBlocked, new StateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.Transition.ConditionCallback(CreatureDeliveryPoint.States.IsStrawBlocked));
			this.operational.PlayAnim("on", KAnim.PlayMode.Once, (CreatureDeliveryPoint.SMInstance smi) => smi.master.animSuffix).EventTransition(GameHashes.LogicEvent, this.unoperational, GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.Not(new StateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.Transition.ConditionCallback(CreatureDeliveryPoint.States.ShouldBeOn))).EventTransition(GameHashes.BuildingStrawChange, this.unoperational, GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.Not(new StateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.Transition.ConditionCallback(CreatureDeliveryPoint.States.ShouldBeOn)))
				.EventTransition(GameHashes.OperationalChanged, this.unoperational, GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.Not(new StateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.Transition.ConditionCallback(CreatureDeliveryPoint.States.ShouldBeOn)))
				.Enter(new StateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.State.Callback(CreatureDeliveryPoint.States.RefreshFetches))
				.DefaultState(this.operational.waiting);
			this.operational.waiting.EnterTransition(this.operational.interact_waiting, (CreatureDeliveryPoint.SMInstance smi) => smi.master.playAnimsOnFetch).EnterTransition(this.operational.interact_pre, new StateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.Transition.ConditionCallback(CreatureDeliveryPoint.States.HasItemInInventory)).PlayAnim("on", KAnim.PlayMode.Once, (CreatureDeliveryPoint.SMInstance smi) => smi.master.animSuffix);
			this.operational.interact_waiting.EnterTransition(this.operational.interact_pre, new StateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.Transition.ConditionCallback(CreatureDeliveryPoint.States.HasItemInInventory)).WorkableStartTransition((CreatureDeliveryPoint.SMInstance smi) => smi.master.GetComponent<Storage>(), this.operational.interact_pre);
			this.operational.interact_pre.PlayAnim("working_pre", KAnim.PlayMode.Once, (CreatureDeliveryPoint.SMInstance smi) => smi.master.animSuffix).OnAnimQueueComplete(this.operational.interact_pst);
			this.operational.interact_pst.Enter(new StateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.State.Callback(CreatureDeliveryPoint.States.DropAllCreatures)).PlayAnim("working_pst", KAnim.PlayMode.Once, (CreatureDeliveryPoint.SMInstance smi) => smi.master.animSuffix).OnAnimQueueComplete(this.operational.interact_waiting);
		}

		public static bool ShouldBeOn(CreatureDeliveryPoint.SMInstance smi)
		{
			return CreatureDeliveryPoint.States.IsLogicEnabled(smi) && CreatureDeliveryPoint.States.IsOperational(smi) && !CreatureDeliveryPoint.States.IsStrawBlocked(smi) && !CreatureDeliveryPoint.States.IsStrawOutsideLiquid(smi);
		}

		public static bool IsLogicEnabled(CreatureDeliveryPoint.SMInstance smi)
		{
			return smi.master.LogicEnabled();
		}

		public static bool IsOperational(CreatureDeliveryPoint.SMInstance smi)
		{
			return smi.IsOperational;
		}

		public static bool IsStrawBlocked(CreatureDeliveryPoint.SMInstance smi)
		{
			return smi.IsStrawBlocked;
		}

		public static bool IsStrawOutsideLiquid(CreatureDeliveryPoint.SMInstance smi)
		{
			return smi.IsStrawOutsideLiquid;
		}

		public static void ClearFetches(CreatureDeliveryPoint.SMInstance smi)
		{
			smi.master.ClearFetches();
		}

		public static void RefreshFetches(CreatureDeliveryPoint.SMInstance smi)
		{
			smi.master.RebalanceFetches();
		}

		public static bool HasItemInInventory(CreatureDeliveryPoint.SMInstance smi)
		{
			return !smi.master.GetComponent<Storage>().IsEmpty();
		}

		public static void DropAllCreatures(CreatureDeliveryPoint.SMInstance smi)
		{
			Storage component = smi.master.GetComponent<Storage>();
			if (component.IsEmpty())
			{
				return;
			}
			if (smi.isDroppingAllCreatures)
			{
				return;
			}
			smi.isDroppingAllCreatures = true;
			List<GameObject> items = component.items;
			int count = items.Count;
			Vector3 vector = Grid.CellToPosCBC(Grid.OffsetCell(Grid.PosToCell(smi.transform.GetPosition()), smi.master.spawnOffset), Grid.SceneLayer.Creatures);
			Vector3 vector2 = Grid.CellToPosCBC(Grid.OffsetCell(Grid.PosToCell(smi.transform.GetPosition()), smi.master.largeCritterSpawnOffset), Grid.SceneLayer.Creatures);
			for (int i = count - 1; i >= 0; i--)
			{
				GameObject gameObject = items[i];
				component.Drop(gameObject, true);
				KPrefabID component2 = gameObject.GetComponent<KPrefabID>();
				if (component2 == null || !component2.HasTag(GameTags.LargeCreature))
				{
					gameObject.transform.SetPosition(vector);
				}
				else
				{
					gameObject.transform.SetPosition(vector2);
				}
				gameObject.GetComponent<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.Creatures);
			}
			smi.master.critterCapacity.RefreshCreatureCount(null);
			smi.isDroppingAllCreatures = false;
		}

		public CreatureDeliveryPoint.States.OperationalState operational;

		public CreatureDeliveryPoint.States.UnoperationalStates unoperational;

		public class OperationalState : GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.State
		{
			public GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.State waiting;

			public GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.State interact_waiting;

			public GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.State interact_pre;

			public GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.State interact_pst;
		}

		public class UnoperationalStates : GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.State
		{
			public GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.State noOperational;

			public GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.State strawBlocked;

			public GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.State noLiquidOnStraw;
		}
	}
}
