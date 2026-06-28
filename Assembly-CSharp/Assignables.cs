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
		this.Subscribe(1623392196, new Action<object>(this.OnDeath));
	}

	private void OnDeath(object data)
	{
		foreach (AssignableSlotInstance assignableSlotInstance in this.slots)
		{
			assignableSlotInstance.Unassign();
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

	public bool IsAssigned(AssignableSlot slot)
	{
		DebugUtil.Assert(slot != null, "Assert!");
		AssignableSlotInstance slot2 = this.GetSlot(slot);
		return slot2.assignable != null;
	}

	public bool IsAssigned(Assignable assignable)
	{
		return this.IsAssigned(assignable.slot);
	}

	public void Unassign(Assignable assignable)
	{
		if (assignable != null)
		{
			AssignableSlotInstance assignableSlotInstance = this.GetSlot(assignable.slot);
			if (assignableSlotInstance != null)
			{
				assignableSlotInstance.Unassign();
				if (assignable.subSlots != null)
				{
					foreach (AssignableSlot assignableSlot in assignable.subSlots)
					{
						assignableSlotInstance = this.GetSlot(assignableSlot);
						assignableSlotInstance.Unassign();
					}
				}
			}
		}
	}

	protected void Save<SaveDataType>(ref SaveDataType[] save_data) where SaveDataType : AssignableSlotInstance.AssignableSaveData
	{
		save_data = new SaveDataType[this.slots.Count];
		for (int i = 0; i < this.slots.Count; i++)
		{
			save_data[i] = this.slots[i].Save() as SaveDataType;
		}
	}

	protected void Load(AssignableSlotInstance.AssignableSaveData[] save_data_array)
	{
		foreach (AssignableSlotInstance.AssignableSaveData assignableSaveData in save_data_array)
		{
			if (assignableSaveData != null)
			{
				foreach (AssignableSlotInstance assignableSlotInstance in this.slots)
				{
					if (assignableSlotInstance.slot.Id == assignableSaveData.id)
					{
						assignableSlotInstance.Load(assignableSaveData);
					}
				}
			}
		}
	}

	public Assignable AutoAssignSlot(Navigator navigator, AssignableSlot slot)
	{
		Assignable assignable = this.GetAssignable(slot);
		if (assignable != null)
		{
			return assignable;
		}
		int num = int.MaxValue;
		foreach (Assignable assignable2 in Game.Instance.assignmentManager)
		{
			if (!(assignable2 == null))
			{
				if (!assignable2.IsAssigned())
				{
					if (assignable2.slot == slot)
					{
						if (assignable2.CanAutoAssignTo(navigator))
						{
							int navigationCost = assignable2.GetNavigationCost(navigator);
							if (navigationCost != PathProber.InvalidCost && navigationCost < num)
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
			assignableSlotInstance.Unassign();
		}
	}

	protected List<AssignableSlotInstance> slots = new List<AssignableSlotInstance>();
}
