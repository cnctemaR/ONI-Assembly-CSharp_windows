using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Assignables : KMonoBehaviour, ISaveLoadableJson
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
		this.Subscribe(1623392196, new EventSystem.EventHandler(this.OnDeath));
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
		Debug.LogError("Missing slot " + slot.Id + " on GameObject " + base.gameObject.name);
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

	public void Assign(Assignable assignable)
	{
		if (assignable != null)
		{
			AssignableSlotInstance slot = this.GetSlot(assignable.slot);
			slot.Assign(assignable);
		}
	}

	public void Unassign(Assignable assignable)
	{
		if (assignable != null)
		{
			AssignableSlotInstance slot = this.GetSlot(assignable.slot);
			if (slot != null)
			{
				slot.Unassign(true);
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
		foreach (Assignable assignable2 in AssignmentManager.Instance)
		{
			if (!(assignable2 == null))
			{
				if (!assignable2.IsAssigned())
				{
					if (assignable2.slot == slot)
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
		if (assignable != null)
		{
			assignable.Assign(this);
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
