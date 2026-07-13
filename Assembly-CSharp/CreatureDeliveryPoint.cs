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
		TreeFilterable component = base.GetComponent<TreeFilterable>();
		component.OnFilterChanged = (Action<HashSet<Tag>>)Delegate.Combine(component.OnFilterChanged, new Action<HashSet<Tag>>(this.OnFilterChanged));
		base.GetComponent<Storage>().SetOffsets(this.deliveryOffsets);
		Prioritizable.AddRef(base.gameObject);
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
		public SMInstance(CreatureDeliveryPoint master)
			: base(master)
		{
		}

		public bool isDroppingAllCreatures;
	}

	public class States : GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.operational.waiting;
			this.root.Update("RefreshCreatureCount", delegate(CreatureDeliveryPoint.SMInstance smi, float dt)
			{
				smi.master.critterCapacity.RefreshCreatureCount(null);
			}, UpdateRate.SIM_1000ms, false).EventHandler(GameHashes.OnStorageChange, new StateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.State.Callback(CreatureDeliveryPoint.States.DropAllCreatures));
			this.unoperational.EventTransition(GameHashes.LogicEvent, this.operational, (CreatureDeliveryPoint.SMInstance smi) => smi.master.LogicEnabled());
			this.operational.EventTransition(GameHashes.LogicEvent, this.unoperational, (CreatureDeliveryPoint.SMInstance smi) => !smi.master.LogicEnabled());
			this.operational.waiting.EnterTransition(this.operational.interact_waiting, (CreatureDeliveryPoint.SMInstance smi) => smi.master.playAnimsOnFetch);
			this.operational.interact_waiting.WorkableStartTransition((CreatureDeliveryPoint.SMInstance smi) => smi.master.GetComponent<Storage>(), this.operational.interact_delivery);
			this.operational.interact_delivery.PlayAnim("working_pre").QueueAnim("working_pst", false, null).OnAnimQueueComplete(this.operational.interact_waiting);
		}

		public static void DropAllCreatures(CreatureDeliveryPoint.SMInstance smi)
		{
			if (smi.isDroppingAllCreatures)
			{
				return;
			}
			smi.isDroppingAllCreatures = true;
			Storage component = smi.master.GetComponent<Storage>();
			if (component.IsEmpty())
			{
				return;
			}
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

		public GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.State unoperational;

		public class OperationalState : GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.State
		{
			public GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.State waiting;

			public GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.State interact_waiting;

			public GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.State interact_delivery;
		}
	}
}
