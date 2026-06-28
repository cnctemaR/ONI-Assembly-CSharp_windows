using System;

public class OwnableSlotInstance : AssignableSlotInstance
{
	public OwnableSlotInstance(Assignables assignables, OwnableSlot slot)
		: base(assignables, slot)
	{
	}

	public override AssignableSlotInstance.AssignableSaveData Save()
	{
		OwnableSlotInstance.SaveData saveData = new OwnableSlotInstance.SaveData();
		base.Save(saveData);
		return saveData;
	}

	public class SaveData : AssignableSlotInstance.AssignableSaveData
	{
	}
}
