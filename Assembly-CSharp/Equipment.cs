using System;
using Klei.AI;
using KSerialization;
using STRINGS;

[SerializationConfig(MemberSerialization.OptIn)]
public class Equipment : Assignables
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Components.Equipment.Add(this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
		base.Subscribe(1502190696, delegate(object o)
		{
			this.destroyed = true;
			Debug.Log("QueueDestroyed", null);
		});
		base.Subscribe(1969584890, delegate(object o)
		{
			this.destroyed = true;
			Debug.Log("Destroyed", null);
		});
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.Equipment.Remove(this);
	}

	public void Equip(Equippable equippable)
	{
		EquipmentSlotInstance equipmentSlotInstance = base.GetSlot(equippable.slot) as EquipmentSlotInstance;
		equipmentSlotInstance.Assign(equippable);
		base.Trigger(-448952673, equippable.GetComponent<KPrefabID>());
		equippable.Trigger(-1617557748, this);
		KBatchedAnimController component = equipmentSlotInstance.gameObject.GetComponent<KBatchedAnimController>();
		Attributes attributes = base.gameObject.GetAttributes();
		string name = base.GetComponent<KSelectable>().GetName();
		foreach (AttributeModifier attributeModifier in equippable.def.AttributeModifiers)
		{
			attributes.Add(name, attributeModifier);
		}
		SnapOn component2 = equipmentSlotInstance.gameObject.GetComponent<SnapOn>();
		component2.AttachSnapOnByName(equippable.def.SnapOn);
		if (equippable.def.SnapOn1 != null)
		{
			component2.AttachSnapOnByName(equippable.def.SnapOn1);
		}
		if (equippable.def.BuildOverride != null)
		{
			component.AddBuildOverride(equippable.def.BuildOverride, true, false);
		}
		equippable.GetComponent<KBatchedAnimController>().enabled = false;
		equippable.OnEquip(equipmentSlotInstance);
		if (this.refreshHandle.TimeRemaining > 0f)
		{
			Debug.LogWarning(base.gameObject.GetProperName() + " is already in the process of changing equipment", null);
			this.refreshHandle.ClearScheduler();
		}
		this.refreshHandle = GameScheduler.Instance.Schedule("ChangeEquipment", 2f, delegate(object obj)
		{
			if (base.gameObject != null)
			{
				CreatureSimTemperatureTransfer component3 = base.gameObject.GetComponent<CreatureSimTemperatureTransfer>();
				if (component3 != null)
				{
					component3.RefreshRegistration();
				}
			}
		}, null, null);
		Game.Instance.Trigger(-2146166042, null);
	}

	public void Unequip(Equippable equippable)
	{
		equippable.GetComponent<KBatchedAnimController>().enabled = true;
		AssignableSlotInstance slot = base.GetSlot(equippable.slot);
		slot.Unassign();
		base.Trigger(-1285462312, equippable.GetComponent<KPrefabID>());
		equippable.Trigger(-170173755, this);
		KBatchedAnimController component = slot.gameObject.GetComponent<KBatchedAnimController>();
		if (!this.destroyed)
		{
			if (equippable.def.BuildOverride != null)
			{
				component.ClearBuildOverride(equippable.def.BuildOverride, !equippable.def.IsBody);
			}
			Attributes attributes = slot.gameObject.GetAttributes();
			foreach (AttributeModifier attributeModifier in equippable.def.AttributeModifiers)
			{
				attributes.Remove(attributeModifier);
			}
			if (!equippable.def.IsBody)
			{
				SnapOn component2 = slot.gameObject.GetComponent<SnapOn>();
				component2.DetachSnapOnByName(equippable.def.SnapOn);
				if (equippable.def.SnapOn1 != null)
				{
					component2.DetachSnapOnByName(equippable.def.SnapOn1);
				}
			}
			this.refreshHandle = GameScheduler.Instance.Schedule("ChangeEquipment", 1f, delegate(object obj)
			{
				if (base.gameObject != null)
				{
					CreatureSimTemperatureTransfer component3 = base.gameObject.GetComponent<CreatureSimTemperatureTransfer>();
					if (component3 != null)
					{
						component3.RefreshRegistration();
					}
				}
			}, null, null);
		}
		Game.Instance.Trigger(-2146166042, null);
	}

	public bool IsEquipped(Equippable equippable)
	{
		return equippable.assignee is Equipment && (Equipment)equippable.assignee == this && equippable.isEquipped;
	}

	public bool IsSlotOccupied(AssignableSlot slot)
	{
		EquipmentSlotInstance equipmentSlotInstance = base.GetSlot(slot) as EquipmentSlotInstance;
		return equipmentSlotInstance.IsAssigned() && (equipmentSlotInstance.assignable as Equippable).isEquipped;
	}

	private void OnRefreshUserMenu(object data)
	{
		foreach (AssignableSlotInstance assignableSlotInstance in this)
		{
			EquipmentSlotInstance equipmentSlotInstance = (EquipmentSlotInstance)assignableSlotInstance;
			if (equipmentSlotInstance.assignable != null)
			{
				EquipmentSlotInstance slot_iter = equipmentSlotInstance;
				string text = string.Format(UI.USERMENUACTIONS.UNEQUIP.NAME, equipmentSlotInstance.assignable.GetComponent<Equippable>().def.GenericName);
				this.userMenu.AddButton(new KIconButtonMenu.ButtonInfo("iconDown", text, delegate
				{
					((Equippable)slot_iter.assignable).Unassign();
				}, global::Action.NumActions, null, null, null, "", true), 2f);
			}
		}
	}

	public void UnequipAll()
	{
		foreach (AssignableSlotInstance assignableSlotInstance in this.slots)
		{
			if (assignableSlotInstance.assignable != null)
			{
				assignableSlotInstance.assignable.Unassign();
			}
		}
	}

	[MyCmpAdd]
	private UserMenu userMenu;

	private SchedulerHandle refreshHandle;

	private bool destroyed = false;
}
