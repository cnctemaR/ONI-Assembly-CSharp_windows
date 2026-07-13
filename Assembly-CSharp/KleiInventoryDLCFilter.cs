using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class KleiInventoryDLCFilter : KMonoBehaviour
{
	[HideInInspector]
	public string SelectedDLCID { get; set; }

	private void ShowDropdown(bool show)
	{
		this.dlcFilterButtonContainer.gameObject.SetActive(show);
	}

	public void ResetToDefault()
	{
		this.SetDLCFilter(null);
	}

	public void ConfigButtons()
	{
		this.dropdownButton.ClearOnClick();
		this.dropdownButton.onClick += delegate
		{
			this.ShowDropdown(!this.dlcFilterButtonContainer.gameObject.activeSelf);
		};
		this.MakeButton(null);
		List<string> list = new List<string>(DlcManager.GetActiveDLCIds());
		for (int i = list.Count - 1; i >= 0; i--)
		{
			this.MakeButton(list[i]);
		}
		this.SetDLCFilter(null);
	}

	private void MakeButton(string dlcID)
	{
		HierarchyReferences component = Util.KInstantiateUI(this.dlcFilterButtonPrefab, this.dlcFilterButtonContainer.gameObject, true).GetComponent<HierarchyReferences>();
		component.GetReference<Image>("Logo").sprite = ((dlcID == null) ? Assets.GetSprite("ONI_mini_logo") : Assets.GetSprite(DlcManager.GetDlcSmallLogo(dlcID)));
		component.GetReference<Image>("Stripe").sprite = Assets.GetSprite(DlcManager.GetDlcBannerSprite(dlcID));
		component.GetReference<Image>("Stripe").color = ((dlcID == null) ? Color.white : DlcManager.GetDlcBannerColor(dlcID));
		component.GetReference<KButton>("Button").ClearOnClick();
		component.GetReference<KButton>("Button").onClick += delegate
		{
			this.SetDLCFilter(dlcID);
			this.ShowDropdown(false);
		};
		this.ShowDropdown(false);
	}

	private void SetDLCFilter(string DLCID)
	{
		this.SelectedDLCID = DLCID;
		global::System.Action action = this.onDLCFilterChanged;
		if (action != null)
		{
			action();
		}
		this.selectedDLCIcon.sprite = ((DLCID == null) ? Assets.GetSprite("ONI_mini_logo") : Assets.GetSprite(DlcManager.GetDlcSmallLogo(DLCID)));
		this.selectedDLCStripe.color = ((DLCID == null) ? Color.white : DlcManager.GetDlcBannerColor(DLCID));
		this.dropdownButton.GetComponent<ToolTip>().SetSimpleTooltip(GameUtil.SafeStringFormat(UI.KLEI_INVENTORY_SCREEN.TOOLTIP_DLC_FILTER, new object[] { (DLCID == null) ? UI.KLEI_INVENTORY_SCREEN.TOOLTIP_DLC_FILTER_ALL : DlcManager.GetDlcTitle(DLCID) }));
	}

	public void HideDropdown()
	{
		this.ShowDropdown(false);
	}

	public bool IsDropdownVisible()
	{
		return this.dlcFilterButtonContainer.gameObject.activeSelf;
	}

	[SerializeField]
	private Transform dlcFilterButtonContainer;

	[SerializeField]
	private GameObject dlcFilterButtonPrefab;

	[SerializeField]
	private Image selectedDLCIcon;

	[SerializeField]
	private Image selectedDLCStripe;

	[SerializeField]
	private KButton dropdownButton;

	public global::System.Action onDLCFilterChanged;
}
