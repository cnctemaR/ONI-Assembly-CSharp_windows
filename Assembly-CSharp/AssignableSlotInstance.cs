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
			return;
		}
		this.Unassign();
		this.assignable = assignable;
	}

	public virtual void Unassign()
	{
		if (this.IsAssigned())
		{
			this.assignable = null;
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
