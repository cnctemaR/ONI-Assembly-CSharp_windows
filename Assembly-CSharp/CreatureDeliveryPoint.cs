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
			CreatureDeliveryPoint.capacityStatusItem = new StatusItem("StorageLocker", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, true, 63486);
			CreatureDeliveryPoint.capacityStatusItem.resolveStringCallback = delegate(string str, object data)
			{
				IUserControlledCapacity userControlledCapacity = (IUserControlledCapacity)data;
				string text = Util.FormatWholeNumber(Mathf.Floor(userControlledCapacity.AmountStored));
				float userMaxCapacity = userControlledCapacity.UserMaxCapacity;
				string text2 = Util.FormatWholeNumber(userMaxCapacity);
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
		this.RefreshCreatureCount();
	}

	private void OnFilterChanged(Tag[] tags)
	{
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		bool flag = tags != null && tags.Length != 0;
		component.TintColour = ((!flag) ? this.noFilterTint : this.filterTint);
		this.ClearFetches();
		this.RebalanceFetches();
	}

	private void RefreshCreatureCount()
	{
		int num = Grid.PosToCell(this);
		CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(num);
		int num2 = this.storedCreatureCount;
		this.storedCreatureCount = 0;
		if (cavityForCell != null)
		{
			foreach (KPrefabID kprefabID in cavityForCell.creatures)
			{
				if (!kprefabID.HasTag(GameTags.Creatures.Bagged) && !kprefabID.HasTag(GameTags.Trapped) && !kprefabID.HasTag(GameTags.Egg))
				{
					this.storedCreatureCount++;
				}
			}
		}
		this.storedCreatureCount += Mathf.RoundToInt(base.GetComponent<Storage>().UnitsStored());
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
		TreeFilterable component = base.GetComponent<TreeFilterable>();
		Tag[] tags = component.GetTags();
		ChoreType creatureFetch = Db.Get().ChoreTypes.CreatureFetch;
		Storage component2 = base.GetComponent<Storage>();
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
			FetchOrder2 fetchOrder = new FetchOrder2(creatureFetch, tags, this.requiredFetchTags, null, component2, 1f, FetchOrder2.OperationalRequirement.Operational, 0, null);
			fetchOrder.Submit(new Action<FetchOrder2, Pickupable>(this.OnFetchComplete), false, new Action<FetchOrder2, Pickupable>(this.OnFetchBegun));
			this.fetches.Add(fetchOrder);
			num3++;
		}
		int num7 = this.fetches.Count - num;
		int num8 = this.fetches.Count - 1;
		while (num8 >= 0 && num7 > 0)
		{
			if (!this.fetches[num8].InProgress)
			{
				this.fetches[num8].Cancel("fewer creatures in room");
				this.fetches.RemoveAt(num8);
				num7--;
				num4++;
			}
			num8--;
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

	public CellOffset[] deliveryOffsets = new CellOffset[] { default(CellOffset) };

	public CellOffset spawnOffset = new CellOffset(0, 0);

	private List<FetchOrder2> fetches;

	private static StatusItem capacityStatusItem;

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
				smi.master.RefreshCreatureCount();
			}, UpdateRate.SIM_1000ms, false);
			this.waiting.EventTransition(GameHashes.OnStorageChange, this.creatureDelivered, (CreatureDeliveryPoint.SMInstance smi) => !smi.GetComponent<Storage>().IsEmpty());
			this.creatureDelivered.Enter(delegate(CreatureDeliveryPoint.SMInstance smi)
			{
				Storage component = smi.master.GetComponent<Storage>();
				List<GameObject> items = component.items;
				int count = items.Count;
				int num = Grid.OffsetCell(Grid.PosToCell(smi.transform.GetPosition()), smi.master.spawnOffset);
				Vector3 vector = Grid.CellToPosCBC(num, Grid.SceneLayer.Creatures);
				for (int i = count - 1; i >= 0; i--)
				{
					GameObject gameObject = items[i];
					component.Drop(gameObject);
					gameObject.transform.SetPosition(vector);
				}
				smi.master.RefreshCreatureCount();
			}).GoTo(this.waiting);
		}

		public GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.State waiting;

		public GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.State creatureDelivered;
	}
}
