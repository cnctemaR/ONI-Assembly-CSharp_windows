using System;
using System.Runtime.Serialization;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class Equipment : Assignables
{
	[OnSerializing]
	protected void OnSerializing()
	{
		base.Save<EquipmentSlotInstance.SaveData>(ref this.saveData);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Load(this.saveData);
		this.Subscribe(493375141, new EventSystem.EventHandler(this.OnRefreshUserMenu));
		NameDisplayScreen.Instance.RegisterComponent(base.gameObject, this);
	}

	public void Equip(Equippable equippable)
	{
		EquipmentSlotInstance equipmentSlotInstance = base.GetSlot(equippable.slot) as EquipmentSlotInstance;
		equipmentSlotInstance.Equip(equippable);
		this.Trigger(-1195989806, equippable.GetComponent<KPrefabID>());
	}

	public void Unequip(Equippable equippable)
	{
		EquipmentSlotInstance equipmentSlotInstance = base.GetSlot(equippable.slot) as EquipmentSlotInstance;
		equipmentSlotInstance.Unequip();
		this.Trigger(-272419061, equippable.GetComponent<KPrefabID>());
	}

	public bool IsEquipped(Equippable equippable)
	{
		EquipmentSlotInstance equipmentSlotInstance = base.GetSlot(equippable.slot) as EquipmentSlotInstance;
		return equipmentSlotInstance.isEquipped;
	}

	public bool IsEquipped(EquipmentSlot slot)
	{
		EquipmentSlotInstance equipmentSlotInstance = base.GetSlot(slot) as EquipmentSlotInstance;
		return equipmentSlotInstance.isEquipped;
	}

	private void OnClickUnequip(EquipmentSlotInstance slot)
	{
		slot.Unequip();
	}

	private void OnRefreshUserMenu(object data)
	{
		foreach (AssignableSlotInstance assignableSlotInstance in this)
		{
			EquipmentSlotInstance equipmentSlotInstance = (EquipmentSlotInstance)assignableSlotInstance;
			if (equipmentSlotInstance.assignable != null)
			{
				EquipmentSlotInstance slot_iter = equipmentSlotInstance;
				string text = "Unequip " + equipmentSlotInstance.assignable.GetComponent<KSelectable>().GetName();
				this.userMenu.AddButton(new KIconButtonMenu.ButtonInfo("iconDown", text, delegate
				{
					this.Unequip((Equippable)slot_iter.assignable);
				}, global::Action.NumActions, null, null, null, null, string.Empty));
			}
		}
	}

	[MyCmpAdd]
	private UserMenu userMenu;

	[Serialize]
	private EquipmentSlotInstance.SaveData[] saveData = new EquipmentSlotInstance.SaveData[0];
}
