using System;
using UnityEngine;

public abstract class AssignableSlotInstance
{
	public Assignables assignables { get; private set; }

	public GameObject gameObject
	{
		get
		{
			return this.assignables.gameObject;
		}
	}

	public AssignableSlotInstance(Assignables assignables, AssignableSlot slot)
	{
		this.slot = slot;
		this.assignables = assignables;
	}

	public void Assign(Assignable assignable)
	{
		if (this.assignable == assignable)
		{
			return;
		}
		this.Unassign(false);
		this.assignable = assignable;
		this.assignables.Trigger(-1585839766, this);
	}

	public virtual void Unassign(bool trigger_event = true)
	{
		if (this.unassigning)
		{
			return;
		}
		if (this.IsAssigned())
		{
			this.unassigning = true;
			this.assignable.Unassign();
			if (trigger_event)
			{
				this.assignables.Trigger(-1585839766, this);
			}
			this.assignable = null;
			this.unassigning = false;
		}
	}

	public bool IsAssigned()
	{
		return this.assignable != null;
	}

	public AssignableSlot slot;

	public Assignable assignable;

	private bool unassigning;
}
