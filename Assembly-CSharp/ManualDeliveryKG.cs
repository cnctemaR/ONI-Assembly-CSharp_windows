using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[SkipSaveFileSerialization]
public class ManualDeliveryKG : KMonoBehaviour
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

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.getFetchAmount = new Func<float>(this.GetFetchAmount);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
		this.Subscribe(-111137758, new Action<object>(this.OnRefreshUserMenu));
		if (this.storage != null)
		{
			this.SetStorage(this.storage);
		}
		this.UpdateFilteredItems();
		Prioritizable.AddRef(base.gameObject);
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
			this.storage.Unsubscribe(-1697596308, new Action<object>(this.OnStorageChanged));
		}
		this.AbortDelivery("storage pointer changed");
		this.filteredStoredItems.Clear();
		this.storage = storage;
		if (this.storage != null && base.isSpawned)
		{
			this.storage.Subscribe(-1697596308, new Action<object>(this.OnStorageChanged));
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

	private void SimUpdate(float dt)
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
		float num = this.getFetchAmount();
		if (num > 0f && (this.fetchList == null || this.fetchList.IsComplete))
		{
			if (this.fetchList != null)
			{
				this.fetchList.Cancel("Request Delivery");
			}
			this.fetchList = new FetchList2(this.storage);
			this.fetchList.ShowStatusItem = this.ShowStatusItem;
			this.fetchList.MinimumAmount[this.requestedItemTag] = this.minimumMass;
			this.fetchList.Add(new Tag[] { this.requestedItemTag }, null, num, this.operationalRequirement);
			this.fetchList.Submit(null, false);
		}
	}

	private float GetFetchAmount()
	{
		float num = 0f;
		float stored_mass = 0f;
		this.filteredStoredItems.ForEach(delegate(PrimaryElement pe)
		{
			stored_mass += pe.Mass;
		});
		if (stored_mass < this.refillMass)
		{
			num = Mathf.Max(0f, this.capacity - stored_mass);
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

	private void OnStorageChanged(object data)
	{
		this.UpdateFilteredItems();
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
		this.Pause(true, "Forbid manual delivery");
	}

	private void OnResume()
	{
		this.Pause(false, "Allow manual delivery");
	}

	private void OnRefreshUserMenu(object data)
	{
		if (!this.allowPause)
		{
			return;
		}
		if (!this.paused)
		{
			UserMenu userMenu = this.userMenu;
			string text = UI.USERMENUACTIONS.MANUAL_DELIVERY.TOOLTIP;
			userMenu.AddButton(new KIconButtonMenu.ButtonInfo("action_move_to_storage", UI.USERMENUACTIONS.MANUAL_DELIVERY.NAME, new global::System.Action(this.OnPause), global::Action.NumActions, null, null, null, text, true), 1f);
		}
		else
		{
			UserMenu userMenu2 = this.userMenu;
			string text = UI.USERMENUACTIONS.MANUAL_DELIVERY.TOOLTIP_OFF;
			userMenu2.AddButton(new KIconButtonMenu.ButtonInfo("action_move_to_storage", UI.USERMENUACTIONS.MANUAL_DELIVERY.NAME_OFF, new global::System.Action(this.OnResume), global::Action.NumActions, null, null, null, text, true), 1f);
		}
	}

	[MyCmpAdd]
	private UserMenu userMenu;

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

	[NonSerialized]
	public bool ShowStatusItem = true;

	private FetchList2 fetchList;

	private List<PrimaryElement> filteredStoredItems = new List<PrimaryElement>();

	public Func<float> getFetchAmount;
}
