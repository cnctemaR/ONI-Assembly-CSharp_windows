using System;
using TUNING;

public class EquipmentSet : KMonoBehaviour
{
	public static EquipmentSet Get()
	{
		return EquipmentSet.Instance;
	}

	protected override void OnPrefabInit()
	{
		EquipmentSet.Instance = this;
		this.LoadSlots();
	}

	private void LoadSlots()
	{
		foreach (EquipmentSlot equipmentSlot in EQUIPMENT.SLOTS)
		{
			this.slotSet.Add(equipmentSlot);
		}
	}

	public EquipmentSet.SlotSet slotSet;

	private static EquipmentSet Instance;

	[Serializable]
	public class SlotSet : ResourceSet<EquipmentSlot>
	{
	}
}
