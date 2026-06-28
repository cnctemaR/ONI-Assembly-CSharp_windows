using System;
using UnityEngine;

public class EquipmentSlotInstance : AssignableSlotInstance
{
	public EquipmentSlotInstance(Assignables assignables, EquipmentSlot slot)
		: base(assignables, slot)
	{
	}

	public override AssignableSlotInstance.AssignableSaveData Save()
	{
		EquipmentSlotInstance.SaveData saveData = new EquipmentSlotInstance.SaveData();
		saveData.isEquipped = this.isEquipped;
		base.Save(saveData);
		return saveData;
	}

	public override void Load(AssignableSlotInstance.AssignableSaveData assignable_save_data)
	{
		base.Load(assignable_save_data);
		EquipmentSlotInstance.SaveData saveData = assignable_save_data as EquipmentSlotInstance.SaveData;
		if (saveData.isEquipped)
		{
			if (saveData.assignable == null || saveData.assignable.Get<Equippable>() == null)
			{
				global::Debug.LogWarning("Equippable was not loaded because it was not saved properly. This is expected for save games prior to March 8 2017", null);
			}
			else
			{
				Equippable equippable = saveData.assignable.Get<Equippable>();
				if (equippable != null)
				{
					this.Equip(equippable);
				}
				else
				{
					global::Debug.LogWarning("Equippable was not loaded because it was null", null);
				}
			}
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
			this.assignable.gameObject.transform.SetPosition(base.assignables.gameObject.transform.position + Vector3.up / 2f);
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
