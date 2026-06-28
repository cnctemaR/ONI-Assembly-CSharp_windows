using System;
using System.Collections.Generic;
using UnityEngine;

public class FetchOrder2
{
	public FetchOrder2(Tag[] tags, Storage destination, float amount, bool is_operational_task = false)
	{
		if (amount <= 0f)
		{
			Output.LogError(new object[] { "Requesting an invalid FetchOrder2 amount" });
		}
		this.Tags = tags;
		this.Destination = destination;
		this.TotalAmount = amount;
		this.UnfetchedAmount = amount;
		this.IsOperationalTask = is_operational_task;
	}

	public float TotalAmount { get; set; }

	public Tag[] Tags { get; protected set; }

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

	private void SetFetchTask(float amount)
	{
		FetchChore fetchChore = new FetchChore(this.Destination, amount, this.Tags, null, true, new Action<Chore>(this.OnFetchChoreComplete), new Action<Chore>(this.OnFetchChoreBegin), new Action<Chore>(this.OnFetchChoreEnd), true);
		if (this.IsOperationalTask)
		{
			fetchChore.AddPrecondition(ChorePreconditions.IsOperational, this.Destination.gameObject);
		}
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
		Debug.LogError("UNIMPLEMENTED!");
	}

	public void Resume(string reason)
	{
		Debug.LogError("UNIMPLEMENTED!");
	}

	public void Submit(Action<FetchOrder2, Pickupable> on_complete, bool check_storage_contents)
	{
		this.OnComplete = on_complete;
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

	public void RefreshChoreType()
	{
		foreach (FetchChore fetchChore in this.Chores)
		{
			fetchChore.RefreshChoreType();
		}
	}

	public bool IsMaterialOnStorage(Storage storage, ref float amount, ref Pickupable out_item)
	{
		foreach (GameObject gameObject in this.Destination)
		{
			if (gameObject != null)
			{
				Pickupable component = gameObject.GetComponent<Pickupable>();
				if (component != null)
				{
					KPrefabID component2 = component.GetComponent<KPrefabID>();
					foreach (Tag tag in this.Tags)
					{
						if (component2.HasTag(tag))
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
		Debug.LogError(text);
	}

	public Action<FetchOrder2, Pickupable> OnComplete;

	public List<FetchChore> Chores = new List<FetchChore>();

	private float _UnfetchedAmount;

	private bool IsOperationalTask;
}
