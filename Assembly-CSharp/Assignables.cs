using System;
using System.Collections.Generic;

public class Assignables : KMonoBehaviour
{
	public IEnumerator<AssignableSlotInstance> GetEnumerator()
	{
		return this.slots.GetEnumerator();
	}

	public AssignableSlotInstance this[int idx]
	{
		get
		{
			return this.slots[idx];
		}
	}

	public int Count
	{
		get
		{
			return this.slots.Count;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Subscribe(1623392196, new Action<object>(this.OnDeath));
	}

	private void OnDeath(object data)
	{
		foreach (AssignableSlotInstance assignableSlotInstance in this.slots)
		{
			assignableSlotInstance.Unassign(true);
		}
	}

	public void Add(AssignableSlotInstance slot_instance)
	{
		this.slots.Add(slot_instance);
	}

	public Assignable GetAssignable(AssignableSlot slot)
	{
		AssignableSlotInstance slot2 = this.GetSlot(slot);
		return (slot2 == null) ? null : slot2.assignable;
	}

	public AssignableSlotInstance GetSlot(AssignableSlot slot)
	{
		if (slot == null)
		{
			return null;
		}
		foreach (AssignableSlotInstance assignableSlotInstance in this)
		{
			if (assignableSlotInstance.slot == slot)
			{
				return assignableSlotInstance;
			}
		}
		return null;
	}

	public Assignable AutoAssignSlot(AssignableSlot slot)
	{
		Assignable assignable = this.GetAssignable(slot);
		if (assignable != null)
		{
			return assignable;
		}
		Navigator component = base.GetComponent<Navigator>();
		MinionIdentity component2 = base.GetComponent<MinionIdentity>();
		int num = int.MaxValue;
		foreach (Assignable assignable2 in Game.Instance.assignmentManager)
		{
			if (!(assignable2 == null))
			{
				if (!assignable2.IsAssigned())
				{
					if (assignable2.slot == slot)
					{
						if (assignable2.CanAutoAssignTo(component2))
						{
							int navigationCost = assignable2.GetNavigationCost(component);
							if (navigationCost != -1 && navigationCost < num)
							{
								num = navigationCost;
								assignable = assignable2;
							}
						}
					}
				}
			}
		}
		if (assignable != null)
		{
			assignable.Assign(base.GetComponent<IAssignableIdentity>());
		}
		return assignable;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		foreach (AssignableSlotInstance assignableSlotInstance in this)
		{
			assignableSlotInstance.Unassign(true);
		}
	}

	protected List<AssignableSlotInstance> slots = new List<AssignableSlotInstance>();
}
