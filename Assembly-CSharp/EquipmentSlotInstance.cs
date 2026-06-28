using System;

public class EquipmentSlotInstance : AssignableSlotInstance
{
	public EquipmentSlotInstance(Assignables assignables, EquipmentSlot slot)
		: base(assignables, slot)
	{
	}

	public override AssignableSlotInstance.AssignableSaveData Save()
	{
		EquipmentSlotInstance.SaveData saveData = new EquipmentSlotInstance.SaveData();
		base.Save(saveData);
		return saveData;
	}

	public class SaveData : AssignableSlotInstance.AssignableSaveData
	{
	}
}
