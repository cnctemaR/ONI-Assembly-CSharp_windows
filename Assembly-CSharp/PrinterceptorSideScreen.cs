using System;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class PrinterceptorSideScreen : SideScreenContent
{
	public override bool IsValidForTarget(GameObject target)
	{
		HijackedHeadquarters.Instance smi = target.GetSMI<HijackedHeadquarters.Instance>();
		return smi != null && smi.IsInsideState(smi.sm.operational);
	}

	public override int GetSideScreenSortOrder()
	{
		return 0;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	public override void ScreenUpdate(bool topLevel)
	{
		base.ScreenUpdate(topLevel);
		this.RefreshDisplay();
	}

	private void RefreshDisplay()
	{
		HijackedHeadquarters.Instance smi = this.target.GetSMI<HijackedHeadquarters.Instance>();
		this.interceptStateLabel.text = string.Format(UI.UISIDESCREENS.PRINTERCEPTORSIDESCREEN.INTERCEPT_METER, smi.sm.interceptCharges.Get(smi), 3);
		bool flag = smi.sm.passcodeUnlocked.Get(smi) && Immigration.Instance.ImmigrantsAvailable && smi.sm.interceptCharges.Get(smi) < 3;
		bool flag2 = this.target.IsInsideState(this.target.sm.operational.readyToPrint.pre) || this.target.IsInsideState(this.target.sm.operational.readyToPrint.loop);
		this.interceptButton.isInteractable = flag;
		this.printButton.isInteractable = flag2;
		this.interceptButton.GetComponent<ToolTip>().SetSimpleTooltip(flag ? UI.UISIDESCREENS.PRINTERCEPTORSIDESCREEN.INTERCEPT_TOOLTIP : ((smi.sm.interceptCharges.Get(smi) >= 3) ? UI.UISIDESCREENS.PRINTERCEPTORSIDESCREEN.INTERCEPT_TOOLTIP_DISABLED_TOO_FULL : UI.UISIDESCREENS.PRINTERCEPTORSIDESCREEN.INTERCEPT_TOOLTIP_DISABLED));
		this.printButton.GetComponent<ToolTip>().SetSimpleTooltip(flag2 ? UI.UISIDESCREENS.PRINTERCEPTORSIDESCREEN.PRINT_TOOLTIP : UI.UISIDESCREENS.PRINTERCEPTORSIDESCREEN.PRINT_TOOLTIP_DISABLED);
		for (int i = 0; i < this.progressIndicators.Length; i++)
		{
			Image componentInChildren = this.progressIndicators[i].GetComponentInChildren<Image>();
			componentInChildren.sprite = Def.GetUISprite("Headquarters", "ui", false).first;
			componentInChildren.color = ((i < smi.sm.interceptCharges.Get(smi)) ? Color.white : Color.gray);
		}
		this.databankCountLabel.SetText(GameUtil.SafeStringFormat(UI.UISIDESCREENS.PRINTERCEPTORSIDESCREEN.DATABANK_COUNT, new object[] { this.target.GetComponent<Storage>().GetAmountAvailable(DatabankHelper.ID).ToString() }));
		Image[] array = this.databankIcon;
		for (int j = 0; j < array.Length; j++)
		{
			array[j].sprite = Def.GetUISprite(DatabankHelper.ID, "ui", false).first;
		}
		if (this.target.GetSMI<HijackedHeadquarters.Instance>().sm.passcodeUnlocked.Get(this.target))
		{
			this.lockedSection.SetActive(false);
			this.meterSection.SetActive(true);
			return;
		}
		this.lockedSection.SetActive(true);
		this.meterSection.SetActive(false);
	}

	public override void SetTarget(GameObject new_target)
	{
		this.target = new_target.GetSMI<HijackedHeadquarters.Instance>();
		this.printButton.ClearOnClick();
		this.interceptButton.ClearOnClick();
		this.printButton.onClick += delegate
		{
			this.target.ActivatePrintInterface();
		};
		this.interceptButton.onClick += delegate
		{
			this.target.Intercept();
		};
		this.RefreshDisplay();
	}

	private HijackedHeadquarters.Instance target;

	[SerializeField]
	private KButton printButton;

	[SerializeField]
	private KButton interceptButton;

	[SerializeField]
	private LocText interceptStateLabel;

	[SerializeField]
	private GameObject[] progressIndicators;

	[SerializeField]
	private Image[] databankIcon;

	[SerializeField]
	private LocText databankCountLabel;

	[SerializeField]
	private GameObject meterSection;

	[SerializeField]
	private GameObject lockedSection;
}
