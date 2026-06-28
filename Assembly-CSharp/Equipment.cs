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

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Components.Equipment.Add(this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Load(this.saveData);
		this.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.Equipment.Remove(this);
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
				string text = "Unequip " + equipmentSlotInstance.assignable.GetComponent<Equippable>().def.GenericName;
				this.userMenu.AddButton(new KIconButtonMenu.ButtonInfo("iconDown", text, delegate
				{
					this.Unequip((Equippable)slot_iter.assignable);
				}, global::Action.NumActions, null, null, null, string.Empty, true), 2f);
			}
		}
	}

	[MyCmpAdd]
	private UserMenu userMenu;

	[Serialize]
	private EquipmentSlotInstance.SaveData[] saveData = new EquipmentSlotInstance.SaveData[0];
}
