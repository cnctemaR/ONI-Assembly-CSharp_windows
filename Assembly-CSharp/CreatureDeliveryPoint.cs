using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

public class CreatureDeliveryPoint : StateMachineComponent<CreatureDeliveryPoint.SMInstance>, IUserControlledCapacity
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.fetches = new List<FetchOrder2>();
		TreeFilterable component = base.GetComponent<TreeFilterable>();
		component.OnFilterChanged = (Action<Tag[]>)Delegate.Combine(component.OnFilterChanged, new Action<Tag[]>(this.OnFilterChanged));
		base.GetComponent<Storage>().SetOffsets(this.deliveryOffsets);
		Prioritizable.AddRef(base.gameObject);
		if (CreatureDeliveryPoint.capacityStatusItem == null)
		{
			CreatureDeliveryPoint.capacityStatusItem = new StatusItem("StorageLocker", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, true, 129022);
			CreatureDeliveryPoint.capacityStatusItem.resolveStringCallback = delegate(string str, object data)
			{
				IUserControlledCapacity userControlledCapacity = (IUserControlledCapacity)data;
				string text = Util.FormatWholeNumber(Mathf.Floor(userControlledCapacity.AmountStored));
				string text2 = Util.FormatWholeNumber(userControlledCapacity.UserMaxCapacity);
				str = str.Replace("{Stored}", text).Replace("{Capacity}", text2).Replace("{Units}", userControlledCapacity.CapacityUnits);
				return str;
			};
		}
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, CreatureDeliveryPoint.capacityStatusItem, this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		base.Subscribe<CreatureDeliveryPoint>(-905833192, CreatureDeliveryPoint.OnCopySettingsDelegate);
		base.Subscribe<CreatureDeliveryPoint>(643180843, CreatureDeliveryPoint.RefreshCreatureCountDelegate);
		this.RefreshCreatureCount(null);
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		if (gameObject == null)
		{
			return;
		}
		CreatureDeliveryPoint component = gameObject.GetComponent<CreatureDeliveryPoint>();
		if (component == null)
		{
			return;
		}
		this.creatureLimit = component.creatureLimit;
		this.RebalanceFetches();
	}

	private void OnFilterChanged(Tag[] tags)
	{
		base.GetComponent<KBatchedAnimController>().TintColour = ((tags != null && tags.Length != 0) ? this.filterTint : this.noFilterTint);
		this.ClearFetches();
		this.RebalanceFetches();
	}

	private void RefreshCreatureCount(object data = null)
	{
		int num = Grid.PosToCell(this);
		CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(num);
		int num2 = this.storedCreatureCount;
		this.storedCreatureCount = 0;
		if (cavityForCell != null)
		{
			foreach (KPrefabID kprefabID in cavityForCell.creatures)
			{
				if (!kprefabID.HasTag(GameTags.Creatures.Bagged) && !kprefabID.HasTag(GameTags.Trapped))
				{
					this.storedCreatureCount++;
				}
			}
		}
		if (this.storedCreatureCount != num2)
		{
			this.RebalanceFetches();
		}
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
		Tag[] tags = base.GetComponent<TreeFilterable>().GetTags();
		ChoreType creatureFetch = Db.Get().ChoreTypes.CreatureFetch;
		Storage component = base.GetComponent<Storage>();
		int num = this.creatureLimit - this.storedCreatureCount;
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
			FetchOrder2 fetchOrder = new FetchOrder2(creatureFetch, tags, this.requiredFetchTags, null, component, 1f, FetchOrder2.OperationalRequirement.Operational, 0);
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
		component.OnFilterChanged = (Action<Tag[]>)Delegate.Remove(component.OnFilterChanged, new Action<Tag[]>(this.OnFilterChanged));
		base.OnCleanUp();
	}

	float IUserControlledCapacity.UserMaxCapacity
	{
		get
		{
			return (float)this.creatureLimit;
		}
		set
		{
			this.creatureLimit = Mathf.RoundToInt(value);
			this.RebalanceFetches();
		}
	}

	float IUserControlledCapacity.AmountStored
	{
		get
		{
			return (float)this.storedCreatureCount;
		}
	}

	float IUserControlledCapacity.MinCapacity
	{
		get
		{
			return 0f;
		}
	}

	float IUserControlledCapacity.MaxCapacity
	{
		get
		{
			return 20f;
		}
	}

	bool IUserControlledCapacity.WholeValues
	{
		get
		{
			return true;
		}
	}

	LocString IUserControlledCapacity.CapacityUnits
	{
		get
		{
			return UI.UISIDESCREENS.CAPTURE_POINT_SIDE_SCREEN.UNITS_SUFFIX;
		}
	}

	[MyCmpAdd]
	private Prioritizable prioritizable;

	[SerializeField]
	public Color noFilterTint = FilteredStorage.NO_FILTER_TINT;

	[SerializeField]
	public Color filterTint = FilteredStorage.FILTER_TINT;

	[Serialize]
	private int creatureLimit = 20;

	private int storedCreatureCount;

	public CellOffset[] deliveryOffsets = new CellOffset[1];

	public CellOffset spawnOffset = new CellOffset(0, 0);

	private List<FetchOrder2> fetches;

	private static StatusItem capacityStatusItem;

	public bool playAnimsOnFetch;

	private static readonly EventSystem.IntraObjectHandler<CreatureDeliveryPoint> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<CreatureDeliveryPoint>(delegate(CreatureDeliveryPoint component, object data)
	{
		component.OnCopySettings(data);
	});

	private static readonly EventSystem.IntraObjectHandler<CreatureDeliveryPoint> RefreshCreatureCountDelegate = new EventSystem.IntraObjectHandler<CreatureDeliveryPoint>(delegate(CreatureDeliveryPoint component, object data)
	{
		component.RefreshCreatureCount(data);
	});

	private Tag[] requiredFetchTags = new Tag[] { GameTags.Creatures.Deliverable };

	public class SMInstance : GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.GameInstance
	{
		public SMInstance(CreatureDeliveryPoint master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.waiting;
			this.root.Update("RefreshCreatureCount", delegate(CreatureDeliveryPoint.SMInstance smi, float dt)
			{
				smi.master.RefreshCreatureCount(null);
			}, UpdateRate.SIM_1000ms, false).EventHandler(GameHashes.OnStorageChange, new StateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.State.Callback(CreatureDeliveryPoint.States.DropAllCreatures));
			this.waiting.EnterTransition(this.interact_waiting, (CreatureDeliveryPoint.SMInstance smi) => smi.master.playAnimsOnFetch);
			this.interact_waiting.WorkableStartTransition((CreatureDeliveryPoint.SMInstance smi) => smi.master.GetComponent<Storage>(), this.interact_delivery);
			this.interact_delivery.PlayAnim("working_pre").QueueAnim("working_pst", false, null).OnAnimQueueComplete(this.interact_waiting);
		}

		public static void DropAllCreatures(CreatureDeliveryPoint.SMInstance smi)
		{
			Storage component = smi.master.GetComponent<Storage>();
			if (component.IsEmpty())
			{
				return;
			}
			List<GameObject> items = component.items;
			int count = items.Count;
			Vector3 vector = Grid.CellToPosCBC(Grid.OffsetCell(Grid.PosToCell(smi.transform.GetPosition()), smi.master.spawnOffset), Grid.SceneLayer.Creatures);
			for (int i = count - 1; i >= 0; i--)
			{
				GameObject gameObject = items[i];
				component.Drop(gameObject, true);
				gameObject.transform.SetPosition(vector);
				gameObject.GetComponent<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.Creatures);
			}
			smi.master.RefreshCreatureCount(null);
		}

		public GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.State waiting;

		public GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.State interact_waiting;

		public GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.State interact_delivery;
	}
}
