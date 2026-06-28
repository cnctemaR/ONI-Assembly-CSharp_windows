using System;
using STRINGS;
using UnityEngine;

public class EquippableSideScreenRow : KMonoBehaviour
{
	public void SetIdentity(MinionIdentity identity, Equippable equippable)
	{
		if (this.portraitInstance == null)
		{
			this.portraitInstance = Util.KInstantiateUI<CrewPortrait>(this.portraitPrefab.gameObject, base.gameObject, true);
			this.portraitInstance.transform.SetAsFirstSibling();
		}
		this.targetIdentity = identity;
		this.ClearAllButtons();
		this.portraitInstance.SetCrewMember(identity, true);
		this.portraitInstance.SetAlpha(1f);
		this.targetEquippable = equippable;
		this.targetAssignables = identity.GetComponent<Assignables>();
		Equipment component = identity.GetComponent<Equipment>();
		this.currentEquippable = (Equippable)component.GetAssignable(equippable.slot);
		if (this.currentEquippable == null)
		{
			this.equipButton.onClick += this.Equip;
			this.currentlyEquipped.text = string.Format(UI.UISIDESCREENS.EQUIPPABLESIDESCREEN.CURRENTLY_EQUIPPED, UI.UISIDESCREENS.EQUIPPABLESIDESCREEN.NONE_EQUIPPED);
			this.SetButtonState(false);
		}
		else
		{
			this.swapButton.onClick += this.Equip;
			this.dropButton.onClick += this.Unequip;
			this.currentlyEquipped.text = string.Format(UI.UISIDESCREENS.EQUIPPABLESIDESCREEN.CURRENTLY_EQUIPPED, this.currentEquippable.GetProperName());
			this.SetButtonState(true);
		}
	}

	private void Equip()
	{
		this.targetEquippable.ClickAssign(this.targetAssignables);
		this.SetIdentity(this.targetIdentity, this.targetEquippable);
	}

	private void Unequip()
	{
		this.targetEquippable.assignee.GetComponent<Equipment>().Unequip(this.targetEquippable);
		this.SetIdentity(this.targetIdentity, this.targetEquippable);
	}

	private void SetButtonState(bool hasEquipment)
	{
		this.equipButton.gameObject.SetActive(!hasEquipment);
		this.dropButton.gameObject.SetActive(hasEquipment);
	}

	private void ClearAllButtons()
	{
		this.equipButton.ClearOnClick();
		this.dropButton.ClearOnClick();
		this.swapButton.ClearOnClick();
	}

	[SerializeField]
	private CrewPortrait portraitPrefab;

	private CrewPortrait portraitInstance;

	[SerializeField]
	private KButton equipButton;

	[SerializeField]
	private KButton dropButton;

	[SerializeField]
	private KButton swapButton;

	[SerializeField]
	private LocText currentlyEquipped;

	private Equippable targetEquippable;

	private Equippable currentEquippable;

	private Assignables targetAssignables;

	private MinionIdentity targetIdentity;
}
