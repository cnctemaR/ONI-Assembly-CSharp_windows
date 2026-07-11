using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ManualDeliveryKG : KMonoBehaviour, ISim200ms
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

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (!this.choreTypeIDHash.IsValid)
		{
			this.choreTypeIDHash = Db.Get().ChoreTypes.Fetch.IdHash;
		}
		base.Subscribe<ManualDeliveryKG>(493375141, ManualDeliveryKG.OnRefreshUserMenuDelegate);
		base.Subscribe<ManualDeliveryKG>(-111137758, ManualDeliveryKG.OnRefreshUserMenuDelegate);
		if (this.storage != null)
		{
			this.SetStorage(this.storage);
		}
		this.UpdateFilteredItems();
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
		this.filteredStoredItems.Clear();
		this.storage = storage;
		if (this.storage != null && base.isSpawned)
		{
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

	public void Sim200ms(float dt)
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
		if (!this.paused)
		{
			this.RequestDelivery();
		}
	}

	private void RequestDelivery()
	{
		float fetchAmount = this.GetFetchAmount();
		if (fetchAmount > 0f)
		{
			if (this.fetchList == null || this.fetchList.IsComplete)
			{
				if (this.fetchList != null)
				{
					this.fetchList.Cancel("Request Delivery");
				}
				ChoreType byHash = Db.Get().ChoreTypes.GetByHash(this.choreTypeIDHash);
				this.fetchList = new FetchList2(this.storage, byHash, this.choreTags);
				this.fetchList.ShowStatusItem = this.ShowStatusItem;
				this.fetchList.MinimumAmount[this.requestedItemTag] = this.minimumMass;
				FetchList2 fetchList = this.fetchList;
				Tag[] array = new Tag[] { this.requestedItemTag };
				float num = fetchAmount;
				FetchOrder2.OperationalRequirement operationalRequirement = this.operationalRequirement;
				fetchList.Add(array, null, null, num, operationalRequirement);
				this.fetchList.Submit(null, false);
			}
		}
		else if (this.fetchList != null)
		{
			this.fetchList.Cancel("Storage is full");
			this.fetchList = null;
		}
	}

	private float GetFetchAmount()
	{
		float num = 0f;
		float num2 = 0f;
		for (int i = 0; i < this.filteredStoredItems.Count; i++)
		{
			num2 += this.filteredStoredItems[i].Mass;
		}
		if (num2 < this.refillMass)
		{
			num = Mathf.Max(0f, this.capacity - num2);
		}
		return num;
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
			this.UpdateFilteredItems();
			this.UpdateDeliveryState();
		}
	}

	private void UpdateFilteredItems()
	{
		this.filteredStoredItems.Clear();
		int num = 0;
		while (this.storage != null && num < this.storage.items.Count)
		{
			GameObject gameObject = this.storage.items[num];
			if (gameObject.HasTag(this.requestedItemTag))
			{
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				this.filteredStoredItems.Add(component);
			}
			num++;
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
		KIconButtonMenu.ButtonInfo buttonInfo;
		if (!this.paused)
		{
			string text = "action_move_to_storage";
			string text2 = UI.USERMENUACTIONS.MANUAL_DELIVERY.NAME;
			global::System.Action action = new global::System.Action(this.OnPause);
			string text3 = UI.USERMENUACTIONS.MANUAL_DELIVERY.TOOLTIP;
			buttonInfo = new KIconButtonMenu.ButtonInfo(text, text2, action, global::Action.NumActions, null, null, null, text3, true);
		}
		else
		{
			string text3 = "action_move_to_storage";
			string text2 = UI.USERMENUACTIONS.MANUAL_DELIVERY.NAME_OFF;
			global::System.Action action = new global::System.Action(this.OnResume);
			string text = UI.USERMENUACTIONS.MANUAL_DELIVERY.TOOLTIP_OFF;
			buttonInfo = new KIconButtonMenu.ButtonInfo(text3, text2, action, global::Action.NumActions, null, null, null, text, true);
		}
		KIconButtonMenu.ButtonInfo buttonInfo2 = buttonInfo;
		Game.Instance.userMenu.AddButton(base.gameObject, buttonInfo2, 1f);
	}

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

	[SerializeField]
	public Tag[] choreTags;

	[Serialize]
	private bool userPaused;

	[NonSerialized]
	public bool ShowStatusItem = true;

	private FetchList2 fetchList;

	private List<PrimaryElement> filteredStoredItems = new List<PrimaryElement>();

	private int onStorageChangeSubscription = -1;

	private static readonly EventSystem.IntraObjectHandler<ManualDeliveryKG> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<ManualDeliveryKG>(delegate(ManualDeliveryKG component, object data)
	{
		component.OnRefreshUserMenu(data);
	});
}
