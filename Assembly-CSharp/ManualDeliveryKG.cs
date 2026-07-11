using System;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/ManualDeliveryKG")]
public class ManualDeliveryKG : KMonoBehaviour, ISim1000ms
{
	public float Capacity
	{
		get
		{
			return this.capacity;
		}
	}

	public Tag RequestedItemTag
	{
		get
		{
			return this.requestedItemTag;
		}
		set
		{
			this.requestedItemTag = value;
			this.AbortDelivery("Requested Item Tag Changed");
		}
	}

	public Storage DebugStorage
	{
		get
		{
			return this.storage;
		}
	}

	public FetchList2 DebugFetchList
	{
		get
		{
			return this.fetchList;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		DebugUtil.Assert(this.choreTypeIDHash.IsValid, "ManualDeliveryKG Must have a valid chore type specified!", base.name);
		if (this.allowPause)
		{
			base.Subscribe<ManualDeliveryKG>(493375141, ManualDeliveryKG.OnRefreshUserMenuDelegate);
			base.Subscribe<ManualDeliveryKG>(-111137758, ManualDeliveryKG.OnRefreshUserMenuDelegate);
		}
		base.Subscribe<ManualDeliveryKG>(-592767678, ManualDeliveryKG.OnOperationalChangedDelegate);
		if (this.storage != null)
		{
			this.SetStorage(this.storage);
		}
		Prioritizable.AddRef(base.gameObject);
		if (this.userPaused && this.allowPause)
		{
			this.OnPause();
		}
	}

	protected override void OnCleanUp()
	{
		this.AbortDelivery("ManualDeliverKG destroyed");
		Prioritizable.RemoveRef(base.gameObject);
		base.OnCleanUp();
	}

	public void SetStorage(Storage storage)
	{
		if (this.storage != null)
		{
			this.storage.Unsubscribe(this.onStorageChangeSubscription);
			this.onStorageChangeSubscription = -1;
		}
		this.AbortDelivery("storage pointer changed");
		this.storage = storage;
		if (this.storage != null && base.isSpawned)
		{
			global::Debug.Assert(this.onStorageChangeSubscription == -1);
			this.onStorageChangeSubscription = this.storage.Subscribe(-1697596308, delegate(object eventData)
			{
				this.OnStorageChanged(this.storage);
			});
		}
	}

	public void Pause(bool pause, string reason)
	{
		if (this.paused != pause)
		{
			this.paused = pause;
			if (pause)
			{
				this.AbortDelivery(reason);
			}
		}
	}

	public void Sim1000ms(float dt)
	{
		this.UpdateDeliveryState();
	}

	[ContextMenu("UpdateDeliveryState")]
	public void UpdateDeliveryState()
	{
		if (!this.requestedItemTag.IsValid)
		{
			return;
		}
		if (this.storage == null)
		{
			return;
		}
		this.UpdateFetchList();
	}

	private void UpdateFetchList()
	{
		if (this.paused)
		{
			return;
		}
		if (this.fetchList != null && this.fetchList.IsComplete)
		{
			this.fetchList = null;
		}
		if (!this.OperationalRequirementsMet())
		{
			if (this.fetchList != null)
			{
				this.fetchList.Cancel("Operational requirements");
				this.fetchList = null;
				return;
			}
		}
		else if (this.fetchList == null)
		{
			float massAvailable = this.storage.GetMassAvailable(this.requestedItemTag);
			if (massAvailable < this.refillMass)
			{
				float num = this.capacity - massAvailable;
				num = Mathf.Max(PICKUPABLETUNING.MINIMUM_PICKABLE_AMOUNT, num);
				ChoreType byHash = Db.Get().ChoreTypes.GetByHash(this.choreTypeIDHash);
				this.fetchList = new FetchList2(this.storage, byHash);
				this.fetchList.ShowStatusItem = this.ShowStatusItem;
				this.fetchList.MinimumAmount[this.requestedItemTag] = Mathf.Max(PICKUPABLETUNING.MINIMUM_PICKABLE_AMOUNT, this.minimumMass);
				this.fetchList.Add(new Tag[] { this.requestedItemTag }, null, null, num, FetchOrder2.OperationalRequirement.None);
				this.fetchList.Submit(null, false);
			}
		}
	}

	private bool OperationalRequirementsMet()
	{
		if (this.operational)
		{
			if (this.operationalRequirement == FetchOrder2.OperationalRequirement.Operational)
			{
				return this.operational.IsOperational;
			}
			if (this.operationalRequirement == FetchOrder2.OperationalRequirement.Functional)
			{
				return this.operational.IsFunctional;
			}
		}
		return true;
	}

	public void AbortDelivery(string reason)
	{
		if (this.fetchList != null)
		{
			FetchList2 fetchList = this.fetchList;
			this.fetchList = null;
			fetchList.Cancel(reason);
		}
	}

	private void OnStorageChanged(Storage storage)
	{
		if (storage == this.storage)
		{
			this.UpdateDeliveryState();
		}
	}

	private void OnPause()
	{
		this.userPaused = true;
		this.Pause(true, "Forbid manual delivery");
	}

	private void OnResume()
	{
		this.userPaused = false;
		this.Pause(false, "Allow manual delivery");
	}

	private void OnRefreshUserMenu(object data)
	{
		if (!this.allowPause)
		{
			return;
		}
		KIconButtonMenu.ButtonInfo buttonInfo = ((!this.paused) ? new KIconButtonMenu.ButtonInfo("action_move_to_storage", UI.USERMENUACTIONS.MANUAL_DELIVERY.NAME, new global::System.Action(this.OnPause), global::Action.NumActions, null, null, null, UI.USERMENUACTIONS.MANUAL_DELIVERY.TOOLTIP, true) : new KIconButtonMenu.ButtonInfo("action_move_to_storage", UI.USERMENUACTIONS.MANUAL_DELIVERY.NAME_OFF, new global::System.Action(this.OnResume), global::Action.NumActions, null, null, null, UI.USERMENUACTIONS.MANUAL_DELIVERY.TOOLTIP_OFF, true));
		Game.Instance.userMenu.AddButton(base.gameObject, buttonInfo, 1f);
	}

	private void OnOperationalChanged(object data)
	{
		this.UpdateDeliveryState();
	}

	[MyCmpGet]
	private Operational operational;

	[SerializeField]
	private Storage storage;

	[SerializeField]
	public Tag requestedItemTag;

	[SerializeField]
	public float capacity = 100f;

	[SerializeField]
	public float refillMass = 10f;

	[SerializeField]
	public float minimumMass = 10f;

	[SerializeField]
	public FetchOrder2.OperationalRequirement operationalRequirement;

	[SerializeField]
	public bool allowPause;

	[SerializeField]
	private bool paused;

	[SerializeField]
	public HashedString choreTypeIDHash;

	[Serialize]
	private bool userPaused;

	public bool ShowStatusItem = true;

	private FetchList2 fetchList;

	private int onStorageChangeSubscription = -1;

	private static readonly EventSystem.IntraObjectHandler<ManualDeliveryKG> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<ManualDeliveryKG>(delegate(ManualDeliveryKG component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<ManualDeliveryKG> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<ManualDeliveryKG>(delegate(ManualDeliveryKG component, object data)
	{
		component.OnOperationalChanged(data);
	});
}
