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
		if (this.IsAssigned())
		{
			this.assignable.Unassign();
			this.assignable = null;
		}
		if (trigger_event)
		{
			this.assignables.Trigger(-1585839766, this);
		}
	}

	public bool IsAssigned()
	{
		return this.assignable != null;
	}

	public AssignableSlot slot;

	public Assignable assignable;
}
