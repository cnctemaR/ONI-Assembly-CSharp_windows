using System;
using System.Collections.Generic;
using UnityEngine;

public class FetchList2 : IFetchList
{
	public FetchList2(Storage destination)
	{
		this.Destination = destination;
	}

	public bool ShowStatusItem
	{
		get
		{
			return this.bShowStatusItem;
		}
		set
		{
			this.bShowStatusItem = value;
		}
	}

	public bool IsComplete
	{
		get
		{
			return this.FetchOrders.Count == 0;
		}
	}

	public bool InProgress
	{
		get
		{
			if (this.FetchOrders.Count < 0)
			{
				return false;
			}
			bool flag = false;
			foreach (FetchOrder2 fetchOrder in this.FetchOrders)
			{
				if (fetchOrder.InProgress)
				{
					flag = true;
					break;
				}
			}
			return flag;
		}
	}

	public Storage Destination { get; private set; }

	public ChoreType ChoreType { get; private set; }

	public void Add(Tag[] tags, float amount = 1f, bool is_operational_task = false)
	{
		if (amount <= 0f)
		{
			Output.LogError(new object[] { "Requesting an invalid FetchList2 amount" });
		}
		foreach (Tag tag in tags)
		{
			if (!this.MinimumAmount.ContainsKey(tag))
			{
				this.MinimumAmount[tag] = amount;
			}
		}
		FetchOrder2 fetchOrder = new FetchOrder2(tags, this.Destination, amount, is_operational_task);
		this.FetchOrders.Add(fetchOrder);
	}

	public void Add(Tag tag, float amount = 1f, bool is_operational_task = false)
	{
		this.Add(new Tag[] { tag }, amount, is_operational_task);
	}

	public float GetMinimumAmount(Tag tag)
	{
		float num = 0f;
		this.MinimumAmount.TryGetValue(tag, out num);
		return num;
	}

	private void OnFetchOrderComplete(FetchOrder2 fetch_order, Pickupable fetched_item)
	{
		this.FetchOrders.Remove(fetch_order);
		if (this.FetchOrders.Count == 0)
		{
			if (this.OnComplete != null)
			{
				this.OnComplete();
			}
			this.updateStatusItemsHandle.Clear();
			this.ClearStatus();
		}
		else
		{
			FetchList2.UpdateStatus(this);
		}
	}

	public void Cancel(string reason)
	{
		foreach (FetchOrder2 fetchOrder in this.FetchOrders)
		{
			fetchOrder.Cancel(reason);
		}
		this.ClearStatus();
		this.updateStatusItemsHandle.Clear();
	}

	public Dictionary<Tag, float> GetRemaining()
	{
		Dictionary<Tag, float> dictionary = new Dictionary<Tag, float>();
		foreach (FetchOrder2 fetchOrder in this.FetchOrders)
		{
			foreach (Tag tag in fetchOrder.Tags)
			{
				float num = 0f;
				if (!dictionary.TryGetValue(tag, out num))
				{
					dictionary[tag] = 0f;
				}
				dictionary[tag] = num + fetchOrder.TotalAmount;
			}
		}
		foreach (GameObject gameObject in this.Destination)
		{
			if (gameObject != null)
			{
				Pickupable component = gameObject.GetComponent<Pickupable>();
				if (component != null)
				{
					KPrefabID component2 = component.GetComponent<KPrefabID>();
					foreach (Tag tag2 in component2.Tags)
					{
						if (dictionary.ContainsKey(tag2))
						{
							dictionary[tag2] = Math.Max(dictionary[tag2] - component.TotalAmount, 0f);
						}
					}
				}
			}
		}
		return dictionary;
	}

	public Dictionary<Tag, float> GetRemainingMinimum()
	{
		Dictionary<Tag, float> dictionary = new Dictionary<Tag, float>();
		foreach (FetchOrder2 fetchOrder in this.FetchOrders)
		{
			foreach (Tag tag in fetchOrder.Tags)
			{
				dictionary[tag] = this.MinimumAmount[tag];
			}
		}
		foreach (GameObject gameObject in this.Destination)
		{
			if (gameObject != null)
			{
				Pickupable component = gameObject.GetComponent<Pickupable>();
				if (component != null)
				{
					KPrefabID component2 = component.GetComponent<KPrefabID>();
					foreach (Tag tag2 in component2.Tags)
					{
						if (dictionary.ContainsKey(tag2))
						{
							dictionary[tag2] = Math.Max(dictionary[tag2] - component.TotalAmount, 0f);
						}
					}
				}
			}
		}
		return dictionary;
	}

	public void Suspend(string reason)
	{
		foreach (FetchOrder2 fetchOrder in this.FetchOrders)
		{
			fetchOrder.Suspend(reason);
		}
	}

	public void Resume(string reason)
	{
		foreach (FetchOrder2 fetchOrder in this.FetchOrders)
		{
			fetchOrder.Resume(reason);
		}
	}

	public void Submit(global::System.Action on_complete, bool check_storage_contents)
	{
		this.OnComplete = on_complete;
		List<FetchOrder2> range = this.FetchOrders.GetRange(0, this.FetchOrders.Count);
		foreach (FetchOrder2 fetchOrder in range)
		{
			fetchOrder.Submit(new Action<FetchOrder2, Pickupable>(this.OnFetchOrderComplete), check_storage_contents);
		}
		if (!this.IsComplete)
		{
			this.updateStatusItemsHandle = UIScheduler.Instance.SchedulePeriodic(this.Destination.name + ".FetchList.Submit", 1f, new Action<object>(FetchList2.UpdateStatus), this, null);
			FetchList2.UpdateStatus(this);
		}
	}

	private void ClearStatus()
	{
		if (this.Destination != null)
		{
			KSelectable component = this.Destination.GetComponent<KSelectable>();
			if (component != null)
			{
				this.waitingForMaterialsHandle = component.RemoveStatusItem(this.waitingForMaterialsHandle);
				this.materialsUnavailableHandle = component.RemoveStatusItem(this.materialsUnavailableHandle);
				this.materialsUnavailableForRefillHandle = component.RemoveStatusItem(this.materialsUnavailableForRefillHandle);
			}
		}
	}

	private void UpdateStatusItem(MaterialsStatusItem status_item, ref Guid handle, Dictionary<Tag, float> remaining)
	{
		bool flag = status_item.ShouldAdd(this, remaining);
		if (!this.ShowStatusItem)
		{
			flag = false;
		}
		bool flag2 = handle != Guid.Empty;
		if (flag != flag2)
		{
			if (flag)
			{
				KSelectable component = this.Destination.GetComponent<KSelectable>();
				if (component != null)
				{
					handle = component.AddStatusItem(status_item, this);
				}
			}
			else
			{
				KSelectable component2 = this.Destination.GetComponent<KSelectable>();
				if (component2 != null)
				{
					handle = component2.RemoveStatusItem(handle);
				}
			}
		}
	}

	public void RefreshChoreType()
	{
		foreach (FetchOrder2 fetchOrder in this.FetchOrders)
		{
			fetchOrder.RefreshChoreType();
		}
	}

	private static void UpdateStatus(object data)
	{
		FetchList2 fetchList = (FetchList2)data;
		if (fetchList.Destination != null)
		{
			Dictionary<Tag, float> remaining = fetchList.GetRemaining();
			fetchList.UpdateStatusItem(Db.Get().BuildingStatusItems.WaitingForMaterials, ref fetchList.waitingForMaterialsHandle, remaining);
			fetchList.UpdateStatusItem(Db.Get().BuildingStatusItems.MaterialsUnavailable, ref fetchList.materialsUnavailableHandle, remaining);
			fetchList.UpdateStatusItem(Db.Get().BuildingStatusItems.MaterialsUnavailableForRefill, ref fetchList.materialsUnavailableForRefillHandle, remaining);
		}
	}

	private global::System.Action OnComplete;

	private SchedulerHandle updateStatusItemsHandle;

	private Guid waitingForMaterialsHandle = Guid.Empty;

	private Guid materialsUnavailableForRefillHandle = Guid.Empty;

	private Guid materialsUnavailableHandle = Guid.Empty;

	public Dictionary<Tag, float> MinimumAmount = new Dictionary<Tag, float>();

	public List<FetchOrder2> FetchOrders = new List<FetchOrder2>();

	private bool bShowStatusItem = true;
}
