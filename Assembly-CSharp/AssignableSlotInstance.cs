using System;
using UnityEngine;

public abstract class AssignableSlotInstance
{
	public AssignableSlotInstance(Assignables assignables, AssignableSlot slot)
	{
		this.slot = slot;
		this.assignables = assignables;
	}

	public Assignables assignables { get; private set; }

	public GameObject gameObject
	{
		get
		{
			return this.assignables.gameObject;
		}
	}

	public abstract AssignableSlotInstance.AssignableSaveData Save();

	protected void Save(AssignableSlotInstance.AssignableSaveData save_data)
	{
		save_data.id = this.slot.Id;
		save_data.assignable = new Ref<Assignable>(this.assignable);
	}

	public virtual void Load(AssignableSlotInstance.AssignableSaveData save_data)
	{
		this.Assign(save_data.assignable.Get());
	}

	public void Assign(Assignable assignable)
	{
		if (this.assignable == assignable)
		{
			if (assignable != null)
			{
				Debug.Log("Assign() Already assigned " + assignable.name);
			}
			return;
		}
		this.Unassign(false);
		this.assignable = assignable;
		if (this.assignable != null)
		{
			this.assignable.Assign(this.assignables);
		}
		this.assignables.Trigger(-1585839766, this);
	}

	public virtual void Unassign(bool trigger_event = true)
	{
		if (this.IsAssigned())
		{
			Assignable assignable = this.assignable;
			this.assignable = null;
			assignable.Unassign();
			if (trigger_event)
			{
				this.assignables.Trigger(-1585839766, this);
			}
		}
	}

	public bool IsAssigned()
	{
		return this.assignable != null;
	}

	public AssignableSlot slot;

	public Assignable assignable;

	public class AssignableSaveData
	{
		public string id;

		public Ref<Assignable> assignable = new Ref<Assignable>();
	}
}
