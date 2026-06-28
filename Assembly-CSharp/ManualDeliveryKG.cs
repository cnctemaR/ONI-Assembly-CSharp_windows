using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
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
		if (this.storage != null)
		{
			this.Subscribe(-1697596308, new EventSystem.EventHandler(this.OnStorageChanged));
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.UpdateFilteredItems();
	}

	public void SetStorage(Storage storage)
	{
		if (this.storage != null)
		{
			this.Unsubscribe(-1697596308, new EventSystem.EventHandler(this.OnStorageChanged));
		}
		this.AbortDelivery("storage pointer changed");
		this.filteredStoredItems.Clear();
		this.storage = storage;
		if (this.storage != null)
		{
			this.storage.Subscribe(-1697596308, new EventSystem.EventHandler(this.OnStorageChanged));
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
		float fetchAmount = this.GetFetchAmount();
		if (fetchAmount > 0f && (this.fetchList == null || this.fetchList.IsComplete))
		{
			if (this.fetchList != null)
			{
				this.fetchList.Cancel("Request Delivery");
			}
			this.fetchList = new FetchList2(this.storage);
			this.fetchList.ShowStatusItem = this.ShowStatusItem;
			this.fetchList.MinimumAmount[this.requestedItemTag] = this.minimumMass;
			this.fetchList.Add(new Tag[] { this.requestedItemTag }, fetchAmount, true);
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

	[MyCmpGet]
	private Storage storage;

	[SerializeField]
	public Tag requestedItemTag;

	[SerializeField]
	public float capacity = 100f;

	[SerializeField]
	public float refillMass = 10f;

	[SerializeField]
	public float minimumMass = 10f;

	[NonSerialized]
	public bool ShowStatusItem = true;

	private FetchList2 fetchList;

	private List<PrimaryElement> filteredStoredItems = new List<PrimaryElement>();

	private bool paused;
}
