using System;
using Klei.AI;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class Equipment : Assignables
{
	public bool destroyed { get; private set; }

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Components.Equipment.Add(this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Subscribe<Equipment>(1502190696, Equipment.SetDestroyedTrueDelegate);
		base.Subscribe<Equipment>(1969584890, Equipment.SetDestroyedTrueDelegate);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		this.refreshHandle.ClearScheduler();
		Components.Equipment.Remove(this);
	}

	public void Equip(Equippable equippable)
	{
		AssignableSlotInstance slot = base.GetSlot(equippable.slot);
		slot.Assign(equippable);
		base.GetTargetGameObject().Trigger(-448952673, equippable.GetComponent<KPrefabID>());
		equippable.Trigger(-1617557748, this);
		Attributes attributes = base.GetTargetGameObject().GetAttributes();
		if (attributes != null)
		{
			foreach (AttributeModifier attributeModifier in equippable.def.AttributeModifiers)
			{
				attributes.Add(attributeModifier);
			}
		}
		SnapOn component = base.GetTargetGameObject().GetComponent<SnapOn>();
		if (component != null)
		{
			component.AttachSnapOnByName(equippable.def.SnapOn);
			if (equippable.def.SnapOn1 != null)
			{
				component.AttachSnapOnByName(equippable.def.SnapOn1);
			}
		}
		KBatchedAnimController component2 = base.GetTargetGameObject().GetComponent<KBatchedAnimController>();
		if (component2 != null && equippable.def.BuildOverride != null)
		{
			component2.GetComponent<SymbolOverrideController>().AddBuildOverride(equippable.def.BuildOverride.GetData(), equippable.def.BuildOverridePriority);
		}
		equippable.GetComponent<KBatchedAnimController>().enabled = false;
		equippable.OnEquip(slot);
		if (this.refreshHandle.TimeRemaining > 0f)
		{
			Debug.LogWarning(base.GetTargetGameObject().GetProperName() + " is already in the process of changing equipment", null);
			this.refreshHandle.ClearScheduler();
		}
		CreatureSimTemperatureTransfer transferer = base.GetTargetGameObject().GetComponent<CreatureSimTemperatureTransfer>();
		if (!(component2 == null))
		{
			this.refreshHandle = GameScheduler.Instance.Schedule("ChangeEquipment", 2f, delegate(object obj)
			{
				if (transferer != null)
				{
					transferer.RefreshRegistration();
				}
			}, null, null);
		}
		Game.Instance.Trigger(-2146166042, null);
	}

	public void Unequip(Equippable equippable)
	{
		equippable.GetComponent<KBatchedAnimController>().enabled = true;
		AssignableSlotInstance slot = base.GetSlot(equippable.slot);
		slot.Unassign(true);
		base.GetTargetGameObject().Trigger(-1285462312, equippable.GetComponent<KPrefabID>());
		equippable.Trigger(-170173755, this);
		KBatchedAnimController component = base.GetTargetGameObject().GetComponent<KBatchedAnimController>();
		if (!this.destroyed)
		{
			if (equippable.def.BuildOverride != null && component != null)
			{
				component.GetComponent<SymbolOverrideController>().TryRemoveBuildOverride(equippable.def.BuildOverride.GetData(), equippable.def.BuildOverridePriority);
			}
			Attributes attributes = base.GetTargetGameObject().GetAttributes();
			if (attributes != null)
			{
				foreach (AttributeModifier attributeModifier in equippable.def.AttributeModifiers)
				{
					attributes.Remove(attributeModifier);
				}
			}
			if (!equippable.def.IsBody)
			{
				SnapOn component2 = base.GetTargetGameObject().GetComponent<SnapOn>();
				component2.DetachSnapOnByName(equippable.def.SnapOn);
				if (equippable.def.SnapOn1 != null)
				{
					component2.DetachSnapOnByName(equippable.def.SnapOn1);
				}
			}
			if (!(component == null))
			{
				this.refreshHandle = GameScheduler.Instance.Schedule("ChangeEquipment", 1f, delegate(object obj)
				{
					if (this != null && this.GetTargetGameObject() != null)
					{
						CreatureSimTemperatureTransfer component3 = this.GetTargetGameObject().GetComponent<CreatureSimTemperatureTransfer>();
						if (component3 != null)
						{
							component3.RefreshRegistration();
						}
					}
				}, null, null);
			}
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

	private SchedulerHandle refreshHandle;

	private static readonly EventSystem.IntraObjectHandler<Equipment> SetDestroyedTrueDelegate = new EventSystem.IntraObjectHandler<Equipment>(delegate(Equipment component, object data)
	{
		component.destroyed = true;
	});
}
