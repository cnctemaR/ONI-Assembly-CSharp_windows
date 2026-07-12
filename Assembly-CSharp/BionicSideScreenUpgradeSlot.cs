using System;
using STRINGS;
using UnityEngine;

public class BionicSideScreenUpgradeSlot : KMonoBehaviour
{
	public BionicUpgradesMonitor.UpgradeComponentSlot upgradeSlot { get; private set; }

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		MultiToggle multiToggle = this.toggle;
		multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(this.OnSlotClicked));
	}

	public void Setup(BionicUpgradesMonitor.UpgradeComponentSlot upgradeSlot)
	{
		if (this.upgradeSlot != null)
		{
			BionicUpgradesMonitor.UpgradeComponentSlot upgradeSlot2 = this.upgradeSlot;
			upgradeSlot2.OnAssignedUpgradeChanged = (Action<BionicUpgradesMonitor.UpgradeComponentSlot>)Delegate.Remove(upgradeSlot2.OnAssignedUpgradeChanged, new Action<BionicUpgradesMonitor.UpgradeComponentSlot>(this.OnAssignedUpgradeChanged));
		}
		this.upgradeSlot = upgradeSlot;
		if (upgradeSlot != null)
		{
			upgradeSlot.OnAssignedUpgradeChanged = (Action<BionicUpgradesMonitor.UpgradeComponentSlot>)Delegate.Combine(upgradeSlot.OnAssignedUpgradeChanged, new Action<BionicUpgradesMonitor.UpgradeComponentSlot>(this.OnAssignedUpgradeChanged));
		}
		this.Refresh();
	}

	private void OnAssignedUpgradeChanged(BionicUpgradesMonitor.UpgradeComponentSlot slot)
	{
		this.Refresh();
	}

	public void Refresh()
	{
		this.label.color = this.standardColor;
		BionicSideScreenUpgradeSlot.State state = (this.upgradeSlot.IsLocked ? BionicSideScreenUpgradeSlot.State.Locked : BionicSideScreenUpgradeSlot.State.Empty);
		if (state == BionicSideScreenUpgradeSlot.State.Empty && this.upgradeSlot.HasUpgradeInstalled)
		{
			state = BionicSideScreenUpgradeSlot.State.Installed;
		}
		else if (state == BionicSideScreenUpgradeSlot.State.Empty && this.upgradeSlot.HasUpgradeComponentAssigned && !this.upgradeSlot.GetAssignableSlotInstance().IsUnassigning())
		{
			state = BionicSideScreenUpgradeSlot.State.Assigned;
		}
		switch (state)
		{
		case BionicSideScreenUpgradeSlot.State.Locked:
			this.tooltip.SizingSetting = ToolTip.ToolTipSizeSetting.DynamicWidthNoWrap;
			this.tooltip.SetSimpleTooltip(BionicSideScreenUpgradeSlot.TEXT_TOOLTIP_BLOCKED);
			this.label.SetText(BionicSideScreenUpgradeSlot.TEXT_BLOCKED_SLOT);
			this.label.Opacity(0.5f);
			this.icon.gameObject.SetActive(false);
			break;
		case BionicSideScreenUpgradeSlot.State.Empty:
			this.tooltip.SizingSetting = ToolTip.ToolTipSizeSetting.DynamicWidthNoWrap;
			this.tooltip.SetSimpleTooltip(BionicSideScreenUpgradeSlot.TEXT_TOOLTIP_EMPTY);
			this.label.SetText(BionicSideScreenUpgradeSlot.TEXT_NO_UPGRADE_INSTALLED);
			this.label.Opacity(1f);
			this.icon.gameObject.SetActive(false);
			break;
		case BionicSideScreenUpgradeSlot.State.Assigned:
			this.icon.sprite = Def.GetUISprite(this.upgradeSlot.assignedUpgradeComponent.gameObject, "ui", false).first;
			this.icon.Opacity(0.5f);
			this.icon.gameObject.SetActive(true);
			this.label.SetText(BionicSideScreenUpgradeSlot.TEXT_UPGRADE_ASSIGNED_NOT_INSTALLED);
			this.label.Opacity(1f);
			this.tooltip.SizingSetting = ToolTip.ToolTipSizeSetting.MaxWidthWrapContent;
			this.tooltip.SetSimpleTooltip(string.Format(BionicSideScreenUpgradeSlot.TEXT_TOOLTIP_ASSIGNED, this.upgradeSlot.assignedUpgradeComponent.GetProperName()));
			break;
		case BionicSideScreenUpgradeSlot.State.Installed:
			this.icon.sprite = Def.GetUISprite(this.upgradeSlot.installedUpgradeComponent.gameObject, "ui", false).first;
			this.icon.Opacity(1f);
			this.icon.gameObject.SetActive(true);
			this.label.SetText(BionicSideScreenUpgradeSlot.TEXT_UPGRADE_INSTALLED);
			this.label.Opacity(1f);
			this.tooltip.SizingSetting = ToolTip.ToolTipSizeSetting.MaxWidthWrapContent;
			this.tooltip.SetSimpleTooltip(string.Format(BionicSideScreenUpgradeSlot.TEXT_TOOLTIP_INSTALLED, BionicUpgradeComponentConfig.GenerateTooltipForBooster(this.upgradeSlot.installedUpgradeComponent)));
			break;
		}
		this.SetSelected(this._isSelected);
	}

	private void OnSlotClicked()
	{
		Action<BionicSideScreenUpgradeSlot> onClick = this.OnClick;
		if (onClick == null)
		{
			return;
		}
		onClick(this);
	}

	public void SetSelected(bool isSelected)
	{
		this._isSelected = isSelected;
		bool flag = this.upgradeSlot == null || this.upgradeSlot.IsLocked;
		bool flag2 = this.upgradeSlot != null && this.upgradeSlot.HasUpgradeComponentAssigned && !this.upgradeSlot.GetAssignableSlotInstance().IsUnassigning();
		bool flag3 = flag2 && this.upgradeSlot.assignedUpgradeComponent.Booster == BionicUpgradeComponentConfig.BoosterType.Basic;
		this.toggle.ChangeState((flag ? 0 : 2) + (flag2 ? 2 : 0) + ((flag2 && flag3) ? 2 : 0) + (isSelected ? 1 : 0));
	}

	public static string TEXT_BLOCKED_SLOT = UI.UISIDESCREENS.BIONIC_SIDE_SCREEN.UPGRADE_SLOT_LOCKED;

	public static string TEXT_NO_UPGRADE_INSTALLED = UI.UISIDESCREENS.BIONIC_SIDE_SCREEN.UPGRADE_SLOT_EMPTY;

	public static string TEXT_UPGRADE_ASSIGNED_NOT_INSTALLED = UI.UISIDESCREENS.BIONIC_SIDE_SCREEN.UPGRADE_SLOT_ASSIGNED;

	public static string TEXT_UPGRADE_INSTALLED = UI.UISIDESCREENS.BIONIC_SIDE_SCREEN.UPGRADE_SLOT_INSTALLED;

	public static string TEXT_TOOLTIP_BLOCKED = UI.UISIDESCREENS.BIONIC_SIDE_SCREEN.TOOLTIP.SLOT_LOCKED;

	public static string TEXT_TOOLTIP_EMPTY = UI.UISIDESCREENS.BIONIC_SIDE_SCREEN.TOOLTIP.SLOT_EMPTY;

	public static string TEXT_TOOLTIP_ASSIGNED = UI.UISIDESCREENS.BIONIC_SIDE_SCREEN.TOOLTIP.SLOT_ASSIGNED;

	public static string TEXT_TOOLTIP_INSTALLED = UI.UISIDESCREENS.BIONIC_SIDE_SCREEN.TOOLTIP.SLOT_INSTALLED;

	public MultiToggle toggle;

	public KImage icon;

	public LocText label;

	public ToolTip tooltip;

	[Header("Effects settings")]
	public float inUseAnimationDuration = 0.5f;

	public Color standardColor = Color.black;

	public Color activeColor = Color.blue;

	public Color activeColorTooltip = Color.blue;

	public Action<BionicSideScreenUpgradeSlot> OnClick;

	private bool _isSelected;

	public enum State
	{
		Locked,
		Empty,
		Assigned,
		Installed
	}
}
