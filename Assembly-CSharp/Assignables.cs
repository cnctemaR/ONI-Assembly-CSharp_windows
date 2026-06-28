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
		AssignableSlotInstance assignableSlotInstance;
		if (slot == null)
		{
			assignableSlotInstance = null;
		}
		else
		{
			foreach (AssignableSlotInstance assignableSlotInstance2 in this)
			{
				if (assignableSlotInstance2.slot == slot)
				{
					return assignableSlotInstance2;
				}
			}
			assignableSlotInstance = null;
		}
		return assignableSlotInstance;
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
		Assignable assignable2;
		if (assignable != null)
		{
			assignable2 = assignable;
		}
		else
		{
			int num = int.MaxValue;
			foreach (Assignable assignable3 in Game.Instance.assignmentManager)
			{
				if (!(assignable3 == null))
				{
					if (!assignable3.IsAssigned())
					{
						if (assignable3.slot == slot)
						{
							if (assignable3.CanAutoAssignTo(navigator))
							{
								int navigationCost = assignable3.GetNavigationCost(navigator);
								if (navigationCost != PathProber.InvalidCost && navigationCost < num)
								{
									num = navigationCost;
									assignable = assignable3;
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
			assignable2 = assignable;
		}
		return assignable2;
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
