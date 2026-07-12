using System;
using System.Collections;
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
		if (this.wattageLabelEffectCoroutine != null)
		{
			base.StopCoroutine(this.wattageLabelEffectCoroutine);
			this.wattageLabelEffectCoroutine = null;
		}
		this.label.color = this.standardColor;
		if (this.upgradeSlot != null && this.upgradeSlot.HasUpgradeInstalled)
		{
			this.icon.sprite = Def.GetUISprite(this.upgradeSlot.installedUpgradeComponent.gameObject, "ui", false).first;
			this.icon.Opacity(1f);
			float currentWattage = this.upgradeSlot.installedUpgradeComponent.CurrentWattage;
			float potentialWattage = this.upgradeSlot.installedUpgradeComponent.PotentialWattage;
			bool flag = currentWattage != 0f;
			string text = (flag ? ("<b>" + ((currentWattage >= 0f) ? "+" : "-") + "</b>") : "");
			this.label.SetText(string.Format(BionicSideScreenUpgradeSlot.TEXT_UPGRADE_WATTS, text + GameUtil.GetFormattedWattage((currentWattage != 0f) ? currentWattage : potentialWattage, GameUtil.WattageFormatterUnit.Automatic, true)));
			this.label.Opacity((currentWattage != 0f) ? 1f : 0.5f);
			this.icon.gameObject.SetActive(true);
			if (flag && base.gameObject.activeInHierarchy)
			{
				this.wattageLabelEffectCoroutine = base.StartCoroutine(this.UpgradeInUse_WattageLabelEffects());
			}
			this.tooltip.SizingSetting = ToolTip.ToolTipSizeSetting.MaxWidthWrapContent;
			if (flag)
			{
				string text2 = this.activeColorTooltip.ToHexString();
				text = string.Concat(new string[]
				{
					"<color=#",
					text2,
					"><b>",
					(currentWattage >= 0f) ? "+" : "-",
					"</b>"
				});
				this.tooltip.SetSimpleTooltip(string.Format(BionicSideScreenUpgradeSlot.TEXT_TOOLTIP_INSTALLED_IN_USE, this.upgradeSlot.installedUpgradeComponent.GetProperName(), text + GameUtil.GetFormattedWattage(currentWattage, GameUtil.WattageFormatterUnit.Automatic, true) + "</color>", this.upgradeSlot.installedUpgradeComponent.GetComponent<InfoDescription>().description));
			}
			else
			{
				this.tooltip.SetSimpleTooltip(string.Format(BionicSideScreenUpgradeSlot.TEXT_TOOLTIP_INSTALLED_NOT_IN_USE, this.upgradeSlot.installedUpgradeComponent.GetProperName(), GameUtil.GetFormattedWattage(potentialWattage, GameUtil.WattageFormatterUnit.Automatic, true), this.upgradeSlot.installedUpgradeComponent.GetComponent<InfoDescription>().description));
			}
		}
		else if (this.upgradeSlot != null && this.upgradeSlot.HasUpgradeComponentAssigned && !this.upgradeSlot.GetAssignableSlotInstance().IsUnassigning())
		{
			this.icon.sprite = Def.GetUISprite(this.upgradeSlot.assignedUpgradeComponent.gameObject, "ui", false).first;
			this.icon.Opacity(0.5f);
			this.label.SetText(BionicSideScreenUpgradeSlot.TEXT_UPGRADE_ASSIGNED_NOT_INSTALLED);
			this.label.Opacity(1f);
			this.icon.gameObject.SetActive(true);
			this.tooltip.SizingSetting = ToolTip.ToolTipSizeSetting.MaxWidthWrapContent;
			this.tooltip.SetSimpleTooltip(string.Format(BionicSideScreenUpgradeSlot.TEXT_TOOLTIP_ASSIGNED, this.upgradeSlot.assignedUpgradeComponent.GetProperName()));
		}
		else
		{
			this.tooltip.SizingSetting = ToolTip.ToolTipSizeSetting.DynamicWidthNoWrap;
			this.tooltip.SetSimpleTooltip(BionicSideScreenUpgradeSlot.TEXT_TOOLTIP_EMPTY);
			this.label.SetText(BionicSideScreenUpgradeSlot.TEXT_NO_UPGRADE_INSTALLED);
			this.label.Opacity(1f);
			this.icon.gameObject.SetActive(false);
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
		bool flag = this.upgradeSlot != null && this.upgradeSlot.HasUpgradeComponentAssigned && !this.upgradeSlot.GetAssignableSlotInstance().IsUnassigning();
		bool flag2 = flag && this.upgradeSlot.assignedUpgradeComponent.Rarity == BionicUpgradeComponentConfig.RarityType.Special;
		this.toggle.ChangeState((isSelected ? 1 : 0) + (flag ? 2 : 0) + ((flag && flag2) ? 2 : 0));
	}

	private IEnumerator UpgradeInUse_WattageLabelEffects()
	{
		while (base.gameObject.activeInHierarchy)
		{
			float num = (Mathf.Sin(((this.inUseAnimationDuration <= 0f) ? 0f : (Time.time / this.inUseAnimationDuration * 2f * 3.1415927f)) - 1.5707964f) + 1f) * 0.5f;
			Color color = Color.Lerp(this.standardColor, this.activeColor, num);
			this.label.color = color;
			yield return null;
		}
		yield break;
	}

	public static string TEXT_NO_UPGRADE_INSTALLED = UI.UISIDESCREENS.BIONIC_SIDE_SCREEN.UPGRADE_SLOT_EMPTY;

	public static string TEXT_UPGRADE_ASSIGNED_NOT_INSTALLED = UI.UISIDESCREENS.BIONIC_SIDE_SCREEN.UPGRADE_SLOT_ASSIGNED;

	public static string TEXT_UPGRADE_WATTS = UI.UISIDESCREENS.BIONIC_SIDE_SCREEN.UPGRADE_SLOT_WATTAGE;

	public static string TEXT_TOOLTIP_EMPTY = UI.UISIDESCREENS.BIONIC_SIDE_SCREEN.TOOLTIP.SLOT_EMPTY;

	public static string TEXT_TOOLTIP_ASSIGNED = UI.UISIDESCREENS.BIONIC_SIDE_SCREEN.TOOLTIP.SLOT_ASSIGNED;

	public static string TEXT_TOOLTIP_INSTALLED_NOT_IN_USE = UI.UISIDESCREENS.BIONIC_SIDE_SCREEN.TOOLTIP.SLOT_INSTALLED_NOT_IN_USE;

	public static string TEXT_TOOLTIP_INSTALLED_IN_USE = UI.UISIDESCREENS.BIONIC_SIDE_SCREEN.TOOLTIP.SLOT_INSTALLED_IN_USE;

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

	private Coroutine wattageLabelEffectCoroutine;
}
