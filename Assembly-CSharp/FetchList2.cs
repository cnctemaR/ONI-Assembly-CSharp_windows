using System;
using System.Collections.Generic;
using UnityEngine;

public class FetchList2 : IFetchList, IRender1000ms
{
	public FetchList2(Storage destination, ChoreType chore_type, Tag[] chore_tags)
	{
		this.Destination = destination;
		this.choreType = chore_type;
		this.choreTags = chore_tags;
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

	public int PriorityMod { get; private set; }

	public void SetPriorityMod(int priorityMod)
	{
		this.PriorityMod = priorityMod;
		for (int i = 0; i < this.FetchOrders.Count; i++)
		{
			this.FetchOrders[i].SetPriorityMod(this.PriorityMod);
		}
	}

	public void Add(Tag[] tags, Tag[] forbidden_tags = null, float amount = 1f, FetchOrder2.OperationalRequirement operationalRequirement = FetchOrder2.OperationalRequirement.None)
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
		FetchOrder2 fetchOrder = new FetchOrder2(this.choreType, tags, forbidden_tags, this.Destination, amount, operationalRequirement, this.PriorityMod, this.choreTags);
		this.FetchOrders.Add(fetchOrder);
	}

	public void Add(Tag tag, Tag[] forbidden_tags = null, float amount = 1f, FetchOrder2.OperationalRequirement operationalRequirement = FetchOrder2.OperationalRequirement.None)
	{
		this.Add(new Tag[] { tag }, forbidden_tags, amount, operationalRequirement);
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
			SimAndRenderScheduler.instance.Remove(this);
			this.ClearStatus();
		}
		else
		{
			this.UpdateStatus();
		}
	}

	public void Cancel(string reason)
	{
		foreach (FetchOrder2 fetchOrder in this.FetchOrders)
		{
			fetchOrder.Cancel(reason);
		}
		this.ClearStatus();
		SimAndRenderScheduler.instance.Remove(this);
	}

	private void UpdateRemaining()
	{
		this.Remaining.Clear();
		for (int i = 0; i < this.FetchOrders.Count; i++)
		{
			FetchOrder2 fetchOrder = this.FetchOrders[i];
			for (int j = 0; j < fetchOrder.Tags.Length; j++)
			{
				Tag tag = fetchOrder.Tags[j];
				float num = 0f;
				this.Remaining.TryGetValue(tag, out num);
				this.Remaining[tag] = num + fetchOrder.AmountWaitingToFetch();
			}
		}
	}

	public Dictionary<Tag, float> GetRemaining()
	{
		return this.Remaining;
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
			SimAndRenderScheduler.instance.Add(this, false);
			this.UpdateStatus();
		}
	}

	private void ClearStatus()
	{
		if (this.Destination != null)
		{
			KSelectable component = this.Destination.GetComponent<KSelectable>();
			if (component != null)
			{
				this.waitingForMaterialsHandle = component.RemoveStatusItem(this.waitingForMaterialsHandle, false);
				this.materialsUnavailableHandle = component.RemoveStatusItem(this.materialsUnavailableHandle, false);
				this.materialsUnavailableForRefillHandle = component.RemoveStatusItem(this.materialsUnavailableForRefillHandle, false);
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
					handle = component2.RemoveStatusItem(handle, false);
				}
			}
		}
	}

	public void Render1000ms(float dt)
	{
		this.UpdateStatus();
	}

	private void UpdateStatus()
	{
		if (this.Destination != null)
		{
			this.UpdateRemaining();
			Dictionary<Tag, float> remaining = this.GetRemaining();
			this.UpdateStatusItem(Db.Get().BuildingStatusItems.WaitingForMaterials, ref this.waitingForMaterialsHandle, remaining);
			this.UpdateStatusItem(Db.Get().BuildingStatusItems.MaterialsUnavailable, ref this.materialsUnavailableHandle, remaining);
			this.UpdateStatusItem(Db.Get().BuildingStatusItems.MaterialsUnavailableForRefill, ref this.materialsUnavailableForRefillHandle, remaining);
		}
	}

	private global::System.Action OnComplete;

	private ChoreType choreType;

	private Tag[] choreTags;

	private Guid waitingForMaterialsHandle = Guid.Empty;

	private Guid materialsUnavailableForRefillHandle = Guid.Empty;

	private Guid materialsUnavailableHandle = Guid.Empty;

	public Dictionary<Tag, float> MinimumAmount = new Dictionary<Tag, float>();

	public List<FetchOrder2> FetchOrders = new List<FetchOrder2>();

	private Dictionary<Tag, float> Remaining = new Dictionary<Tag, float>();

	private bool bShowStatusItem = true;
}
