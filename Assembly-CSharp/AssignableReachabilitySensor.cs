using System;

public class AssignableReachabilitySensor : Sensor
{
	public AssignableReachabilitySensor(Sensors sensors)
		: base(sensors)
	{
		MinionAssignablesProxy minionAssignablesProxy = base.gameObject.GetComponent<MinionIdentity>().assignableProxy.Get();
		minionAssignablesProxy.ConfigureAssignableSlots();
		Assignables[] components = minionAssignablesProxy.GetComponents<Assignables>();
		if (components.Length == 0)
		{
			Debug.LogError(base.gameObject.GetProperName() + ": No 'Assignables' components found for AssignableReachabilitySensor");
		}
		int num = 0;
		foreach (Assignables assignables in components)
		{
			num += assignables.Slots.Count;
		}
		this.slots = new AssignableReachabilitySensor.SlotEntry[num];
		int num2 = 0;
		foreach (Assignables assignables2 in components)
		{
			for (int k = 0; k < assignables2.Slots.Count; k++)
			{
				this.slots[num2++].slot = assignables2.Slots[k];
			}
		}
		this.navigator = base.GetComponent<Navigator>();
	}

	public bool IsReachable(AssignableSlot slot)
	{
		for (int i = 0; i < this.slots.Length; i++)
		{
			if (this.slots[i].slot.slot == slot)
			{
				return this.slots[i].isReachable;
			}
		}
		Debug.LogError("Could not find slot: " + slot);
		return false;
	}

	public override void Update()
	{
		for (int i = 0; i < this.slots.Length; i++)
		{
			AssignableReachabilitySensor.SlotEntry slotEntry = this.slots[i];
			AssignableSlotInstance slot = slotEntry.slot;
			if (slot.IsAssigned())
			{
				bool flag = slot.assignable.GetNavigationCost(this.navigator) != -1;
				Operational component = slot.assignable.GetComponent<Operational>();
				if (component != null)
				{
					flag = flag && component.IsOperational;
				}
				if (flag != slotEntry.isReachable)
				{
					slotEntry.isReachable = flag;
					this.slots[i] = slotEntry;
					base.Trigger(334784980, slotEntry);
				}
			}
			else if (slotEntry.isReachable)
			{
				slotEntry.isReachable = false;
				this.slots[i] = slotEntry;
				base.Trigger(334784980, slotEntry);
			}
		}
	}

	private AssignableReachabilitySensor.SlotEntry[] slots;

	private Navigator navigator;

	private struct SlotEntry
	{
		public AssignableSlotInstance slot;

		public bool isReachable;
	}
}
