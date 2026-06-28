using System;

public class AssignableReachabilitySensor : Sensor
{
	public AssignableReachabilitySensor(Sensors sensors)
		: base(sensors)
	{
		Assignables[] components = base.gameObject.GetComponents<Assignables>();
		int num = 0;
		for (int i = 0; i < components.Length; i++)
		{
			num += components[i].Count;
		}
		this.slots = new AssignableReachabilitySensor.SlotEntry[num];
		int num2 = 0;
		foreach (Assignables assignables in components)
		{
			for (int k = 0; k < assignables.Count; k++)
			{
				this.slots[num2++].slot = assignables[k];
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
		Debug.LogError("Could not find slot: " + slot, null);
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
				bool flag = slot.assignable.GetNavigationCost(this.navigator) != PathProber.InvalidCost;
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
