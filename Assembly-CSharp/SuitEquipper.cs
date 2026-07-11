using System;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SuitEquipper")]
public class SuitEquipper : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Subscribe<SuitEquipper>(493375141, SuitEquipper.OnRefreshUserMenuDelegate);
	}

	private void OnRefreshUserMenu(object data)
	{
		foreach (AssignableSlotInstance assignableSlotInstance in base.GetComponent<MinionIdentity>().GetEquipment().Slots)
		{
			EquipmentSlotInstance equipmentSlotInstance = (EquipmentSlotInstance)assignableSlotInstance;
			Equippable equippable = equipmentSlotInstance.assignable as Equippable;
			if (equippable && equippable.unequippable)
			{
				string text = string.Format(UI.USERMENUACTIONS.UNEQUIP.NAME, equippable.def.GenericName);
				Game.Instance.userMenu.AddButton(base.gameObject, new KIconButtonMenu.ButtonInfo("iconDown", text, delegate
				{
					equippable.Unassign();
				}, global::Action.NumActions, null, null, null, "", true), 2f);
			}
		}
	}

	public Equippable IsWearingAirtightSuit()
	{
		Equippable equippable = null;
		foreach (AssignableSlotInstance assignableSlotInstance in base.GetComponent<MinionIdentity>().GetEquipment().Slots)
		{
			Equippable equippable2 = ((EquipmentSlotInstance)assignableSlotInstance).assignable as Equippable;
			if (equippable2 && equippable2.GetComponent<KPrefabID>().HasTag(GameTags.AirtightSuit))
			{
				equippable = equippable2;
				break;
			}
		}
		return equippable;
	}

	private static readonly EventSystem.IntraObjectHandler<SuitEquipper> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<SuitEquipper>(delegate(SuitEquipper component, object data)
	{
		component.OnRefreshUserMenu(data);
	});
}
