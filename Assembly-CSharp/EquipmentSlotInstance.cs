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
		saveData.isEquipped = this.isEquipped;
		return saveData;
	}

	public override void Load(AssignableSlotInstance.AssignableSaveData assignable_save_data)
	{
		base.Load(assignable_save_data);
		EquipmentSlotInstance.SaveData saveData = assignable_save_data as EquipmentSlotInstance.SaveData;
		if (saveData.isEquipped)
		{
			this.Equip(saveData.assignable.Get<Equippable>());
		}
	}

	public override void Unassign(bool trigger_event = true)
	{
		if (this.assignable != null)
		{
			if (this.isEquipped)
			{
				this.Unequip();
			}
			base.Unassign(trigger_event);
		}
	}

	public void Equip(Equippable equippable)
	{
		base.Assign(equippable);
		this.isEquipped = true;
		equippable.gameObject.SetActive(false);
		equippable.OnEquip(this);
	}

	public void Unequip()
	{
		if (this.assignable != null)
		{
			Equippable equippable = this.assignable as Equippable;
			this.assignable.gameObject.SetActive(true);
			this.assignable.gameObject.transform.SetPosition(base.assignables.gameObject.transform.position);
			equippable.OnUnequip(this);
			this.isEquipped = false;
			this.Unassign(true);
		}
	}

	public bool isEquipped;

	public class SaveData : AssignableSlotInstance.AssignableSaveData
	{
		public bool isEquipped;
	}
}
