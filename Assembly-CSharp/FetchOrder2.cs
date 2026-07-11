using System;
using System.Collections.Generic;
using UnityEngine;

public class FetchOrder2
{
	public FetchOrder2(ChoreType chore_type, Tag[] tags, Tag[] required_tags, Tag[] forbidden_tags, Storage destination, float amount, FetchOrder2.OperationalRequirement operationalRequirement = FetchOrder2.OperationalRequirement.None, int priorityMod = 0, Tag[] chore_tags = null)
	{
		if (amount <= 0f)
		{
			Output.LogError(new object[] { "Requesting an invalid FetchOrder2 amount" });
		}
		this.choreType = chore_type;
		this.Tags = tags;
		this.RequiredTags = required_tags;
		this.ForbiddenTags = forbidden_tags;
		this.Destination = destination;
		this.TotalAmount = amount;
		this.UnfetchedAmount = amount;
		this.PriorityMod = priorityMod;
		this.ChoreTags = chore_tags;
		this.operationalRequirement = operationalRequirement;
	}

	public float TotalAmount { get; set; }

	public int PriorityMod { get; set; }

	public Tag[] Tags { get; protected set; }

	public Tag[] RequiredTags { get; protected set; }

	public Tag[] ForbiddenTags { get; protected set; }

	public Tag[] ChoreTags { get; protected set; }

	public Storage Destination { get; set; }

	private float UnfetchedAmount
	{
		get
		{
			return this._UnfetchedAmount;
		}
		set
		{
			this._UnfetchedAmount = value;
			this.Assert(this._UnfetchedAmount <= this.TotalAmount, "_UnfetchedAmount <= TotalAmount");
			this.Assert(this._UnfetchedAmount >= 0f, "_UnfetchedAmount >= 0");
		}
	}

	public bool InProgress
	{
		get
		{
			bool flag = false;
			foreach (FetchChore fetchChore in this.Chores)
			{
				if (fetchChore.InProgress())
				{
					flag = true;
					break;
				}
			}
			return flag;
		}
	}

	private void IssueTask()
	{
		if (this.UnfetchedAmount > 0f)
		{
			this.SetFetchTask(this.UnfetchedAmount);
			this.UnfetchedAmount = 0f;
		}
	}

	public void SetPriorityMod(int priorityMod)
	{
		this.PriorityMod = priorityMod;
		for (int i = 0; i < this.Chores.Count; i++)
		{
			this.Chores[i].SetPriorityMod(this.PriorityMod);
		}
	}

	private void SetFetchTask(float amount)
	{
		FetchChore fetchChore = new FetchChore(this.choreType, this.Destination, amount, this.Tags, this.RequiredTags, this.ForbiddenTags, null, true, new Action<Chore>(this.OnFetchChoreComplete), new Action<Chore>(this.OnFetchChoreBegin), new Action<Chore>(this.OnFetchChoreEnd), this.operationalRequirement, this.PriorityMod, this.ChoreTags);
		this.Chores.Add(fetchChore);
	}

	private void OnFetchChoreEnd(Chore chore)
	{
		FetchChore fetchChore = (FetchChore)chore;
		if (this.Chores.Contains(fetchChore))
		{
			this.UnfetchedAmount += fetchChore.amount;
			fetchChore.Cancel("FetchChore Redistribution");
			this.Chores.Remove(fetchChore);
			this.IssueTask();
		}
	}

	private void OnFetchChoreComplete(Chore chore)
	{
		FetchChore fetchChore = (FetchChore)chore;
		this.Chores.Remove(fetchChore);
		if (this.Chores.Count == 0 && this.OnComplete != null)
		{
			this.OnComplete(this, fetchChore.fetchTarget);
		}
	}

	private void OnFetchChoreBegin(Chore chore)
	{
		FetchChore fetchChore = (FetchChore)chore;
		this.UnfetchedAmount += fetchChore.originalAmount - fetchChore.amount;
		this.IssueTask();
		if (this.OnBegin != null)
		{
			this.OnBegin(this, fetchChore.fetchTarget);
		}
	}

	public void Cancel(string reason)
	{
		while (this.Chores.Count > 0)
		{
			FetchChore fetchChore = this.Chores[0];
			fetchChore.Cancel(reason);
			this.Chores.Remove(fetchChore);
		}
	}

	public void Suspend(string reason)
	{
		global::Debug.LogError("UNIMPLEMENTED!", null);
	}

	public void Resume(string reason)
	{
		global::Debug.LogError("UNIMPLEMENTED!", null);
	}

	public void Submit(Action<FetchOrder2, Pickupable> on_complete, bool check_storage_contents, Action<FetchOrder2, Pickupable> on_begin = null)
	{
		this.OnComplete = on_complete;
		this.OnBegin = on_begin;
		this.checkStorageContents = check_storage_contents;
		if (check_storage_contents)
		{
			Pickupable pickupable = null;
			this.UnfetchedAmount = this.GetRemaining(out pickupable);
			if (this.UnfetchedAmount == 0f)
			{
				if (this.OnComplete != null)
				{
					this.OnComplete(this, pickupable);
				}
			}
			else
			{
				this.IssueTask();
			}
		}
		else
		{
			this.IssueTask();
		}
	}

	public bool IsMaterialOnStorage(Storage storage, ref float amount, ref Pickupable out_item)
	{
		foreach (GameObject gameObject in this.Destination.items)
		{
			if (gameObject != null)
			{
				Pickupable component = gameObject.GetComponent<Pickupable>();
				if (component != null)
				{
					KPrefabID kprefabID = component.KPrefabID;
					foreach (Tag tag in this.Tags)
					{
						if (kprefabID.HasTag(tag))
						{
							amount = component.TotalAmount;
							out_item = component;
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	public float AmountWaitingToFetch()
	{
		if (!this.checkStorageContents)
		{
			float num = this.UnfetchedAmount;
			for (int i = 0; i < this.Chores.Count; i++)
			{
				num += this.Chores[i].AmountWaitingToFetch();
			}
			return num;
		}
		Pickupable pickupable;
		return this.GetRemaining(out pickupable);
	}

	public float GetRemaining(out Pickupable out_item)
	{
		float num = this.TotalAmount;
		float num2 = 0f;
		out_item = null;
		if (this.IsMaterialOnStorage(this.Destination, ref num2, ref out_item))
		{
			num = Math.Max(num - num2, 0f);
		}
		return num;
	}

	public bool IsComplete()
	{
		for (int i = 0; i < this.Chores.Count; i++)
		{
			if (!this.Chores[i].isComplete)
			{
				return false;
			}
		}
		return true;
	}

	private void Assert(bool condition, string message)
	{
		if (condition)
		{
			return;
		}
		string text = "FetchOrder error: " + message;
		if (this.Destination == null)
		{
			text += "\nDestination: None";
		}
		else
		{
			text = text + "\nDestination: " + this.Destination.name;
		}
		text = text + "\nTotal Amount: " + this.TotalAmount;
		text = text + "\nUnfetched Amount: " + this._UnfetchedAmount;
		global::Debug.LogError(text, null);
	}

	public Action<FetchOrder2, Pickupable> OnComplete;

	public Action<FetchOrder2, Pickupable> OnBegin;

	public List<FetchChore> Chores = new List<FetchChore>();

	private ChoreType choreType;

	private float _UnfetchedAmount;

	private bool checkStorageContents;

	private FetchOrder2.OperationalRequirement operationalRequirement = FetchOrder2.OperationalRequirement.None;

	public enum OperationalRequirement
	{
		Operational,
		Functional,
		None
	}
}
