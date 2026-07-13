using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class ClusterDestinationSideScreen : SideScreenContent
{
	private ClusterDestinationSelector targetSelector { get; set; }

	private RocketClusterDestinationSelector targetRocketSelector { get; set; }

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.CheckShouldShowTopTitle = () => false;
	}

	protected override void OnSpawn()
	{
		this.changeDestinationButton.onClick += this.OnClickChangeDestination;
		this.clearDestinationButton.onClick += this.OnClickClearDestination;
		this.launchPadDropDown.targetDropDownContainer = GameScreenManager.Instance.ssOverlayCanvas;
		this.launchPadDropDown.CustomizeEmptyRow(UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.FIRSTAVAILABLE, null);
		this.repeatButton.onClick += this.OnRepeatClicked;
	}

	public override int GetSideScreenSortOrder()
	{
		return 103;
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (show)
		{
			this.Refresh(null);
			this.m_refreshHandle = this.targetSelector.Subscribe(543433792, delegate(object data)
			{
				this.Refresh(null);
			});
			this.m_refreshOnCancelHandle = this.targetSelector.Subscribe(94158097, delegate(object data)
			{
				this.Refresh(null);
			});
			return;
		}
		if (this.m_refreshHandle != -1)
		{
			this.targetSelector.Unsubscribe(this.m_refreshHandle);
			this.m_refreshHandle = -1;
			this.launchPadDropDown.Close();
		}
		if (this.m_refreshOnCancelHandle != -1)
		{
			this.targetSelector.Unsubscribe(this.m_refreshOnCancelHandle);
			this.m_refreshOnCancelHandle = -1;
			this.launchPadDropDown.Close();
		}
	}

	public override bool IsValidForTarget(GameObject target)
	{
		ClusterDestinationSelector component = target.GetComponent<ClusterDestinationSelector>();
		bool flag = component != null && component.assignable;
		bool flag2 = target.GetComponent<RocketModuleCluster>() != null && target.GetComponent<RocketModuleCluster>().GetComponent<PassengerRocketModule>() != null;
		bool flag3 = target.GetComponent<RocketModuleCluster>() != null && target.GetComponent<RocketModuleCluster>().GetComponent<RoboPilotModule>() != null;
		if (flag2 || flag3)
		{
			return true;
		}
		bool flag4 = target.GetComponent<RocketControlStation>() != null && target.GetComponent<RocketControlStation>().GetMyWorld().GetComponent<Clustercraft>()
			.Status != Clustercraft.CraftStatus.Launching;
		return flag || flag4;
	}

	public override void SetTarget(GameObject target)
	{
		this.targetSelector = target.GetComponent<ClusterDestinationSelector>();
		if (this.targetSelector == null)
		{
			if (target.GetComponent<RocketModuleCluster>() != null)
			{
				this.targetSelector = target.GetComponent<RocketModuleCluster>().CraftInterface.GetClusterDestinationSelector();
			}
			else if (target.GetComponent<RocketControlStation>() != null)
			{
				this.targetSelector = target.GetMyWorld().GetComponent<Clustercraft>().ModuleInterface.GetClusterDestinationSelector();
			}
		}
		this.targetRocketSelector = this.targetSelector as RocketClusterDestinationSelector;
		this.changeDestinationButtonTooltip.SetSimpleTooltip(this.targetSelector.changeTargetButtonTooltipString);
		this.clearDestinationButton.GetComponent<ToolTip>().SetSimpleTooltip(this.targetSelector.clearTargetButtonTooltipString);
	}

	private void Refresh(object data = null)
	{
		EntityLayer entityLayer = EntityLayer.None;
		bool flag = ClusterMapScreen.Instance.GetMode() == ClusterMapScreen.Mode.SelectDestination;
		if (!this.targetSelector.IsAtDestination())
		{
			ClusterGridEntity clusterEntityTarget = this.targetSelector.GetClusterEntityTarget();
			if (clusterEntityTarget != null)
			{
				this.destinationImage.sprite = clusterEntityTarget.GetUISprite();
				this.destinationInfoLabel.text = GameUtil.SafeStringFormat(UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.DESTINATION_LABEL, new object[] { clusterEntityTarget.GetProperName() });
			}
			else
			{
				Sprite sprite;
				string text;
				string text2;
				ClusterGrid.Instance.GetLocationDescription(this.targetSelector.GetDestination(), out sprite, out text, out text2, out entityLayer);
				this.destinationImage.sprite = sprite;
				this.destinationInfoLabel.text = GameUtil.SafeStringFormat(UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.DESTINATION_LABEL, new object[] { text });
			}
			this.clearDestinationButton.isInteractable = !flag;
		}
		else
		{
			string text3;
			if (this.targetRocketSelector != null && this.targetRocketSelector.Repeat && this.targetRocketSelector.PreviousDestination != AxialI.INVALID)
			{
				ClusterGridEntity visibleEntityOfLayerAtCell = ClusterGrid.Instance.GetVisibleEntityOfLayerAtCell(this.targetRocketSelector.PreviousDestination, this.targetRocketSelector.requiredEntityLayer);
				if (visibleEntityOfLayerAtCell != null)
				{
					this.destinationImage.sprite = visibleEntityOfLayerAtCell.GetUISprite();
					text3 = GameUtil.SafeStringFormat(UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.DESTINATION_ROUNTRIP_LABEL, new object[] { visibleEntityOfLayerAtCell.GetProperName() });
				}
				else
				{
					Sprite sprite2;
					string text4;
					string text5;
					ClusterGrid.Instance.GetLocationDescription(this.targetRocketSelector.PreviousDestination, out sprite2, out text4, out text5, out entityLayer);
					this.destinationImage.sprite = sprite2;
					text3 = GameUtil.SafeStringFormat(UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.DESTINATION_ROUNTRIP_LABEL, new object[] { text4 });
				}
			}
			else
			{
				this.destinationImage.sprite = Assets.GetSprite("hex_unknown");
				text3 = GameUtil.SafeStringFormat(UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.DESTINATION_LABEL, new object[] { UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.DESTINATION_LABEL_INVALID });
			}
			this.destinationInfoLabel.text = text3;
			this.clearDestinationButton.isInteractable = false;
		}
		this.changeDestinationButtonTooltip.SetSimpleTooltip(flag ? UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.CHANGE_DESTINATION_BUTTON_SELECTING_TOOLTIP : this.targetSelector.changeTargetButtonTooltipString);
		this.changeDestinationButton.isInteractable = !flag;
		if (flag)
		{
			this.destinationInfoLabel.text = UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.DESTINATION_LABEL_SELECTING;
		}
		if (this.targetRocketSelector != null)
		{
			List<LaunchPad> launchPadsForDestination = LaunchPad.GetLaunchPadsForDestination(this.targetRocketSelector.GetDestination());
			this.landingPlatformSection.gameObject.SetActive(true);
			this.roundtripSection.gameObject.SetActive(true);
			this.launchPadDropDown.Initialize(launchPadsForDestination, new Action<IListableOption, object>(this.OnLaunchPadEntryClick), new Func<IListableOption, IListableOption, object, int>(this.PadDropDownSort), new Action<DropDownEntry, object>(this.PadDropDownEntryRefreshAction), true, this.targetRocketSelector);
			if (!this.targetRocketSelector.IsAtDestination() && launchPadsForDestination.Count > 0)
			{
				this.launchPadDropDown.openButton.isInteractable = true;
				LaunchPad destinationPad = this.targetRocketSelector.GetDestinationPad();
				if (destinationPad != null)
				{
					this.launchPadDropDown.selectedLabel.text = destinationPad.GetProperName();
					this.landingPlatformInfoLabel.SetText(GameUtil.SafeStringFormat(UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.LANDING_PLATFORM_LABEL, new object[] { destinationPad.GetProperName() }));
				}
				else
				{
					this.launchPadDropDown.selectedLabel.text = UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.FIRSTAVAILABLE;
					this.landingPlatformInfoLabel.SetText(GameUtil.SafeStringFormat(UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.LANDING_PLATFORM_LABEL, new object[] { UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.FIRSTAVAILABLE }));
				}
			}
			else
			{
				this.launchPadDropDown.selectedLabel.text = UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.FIRSTAVAILABLE;
				this.landingPlatformInfoLabel.SetText(GameUtil.SafeStringFormat(UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.LANDING_PLATFORM_LABEL, new object[] { UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.FIRSTAVAILABLE }));
				this.launchPadDropDown.openButton.isInteractable = false;
			}
			this.RefreshRepeatButtonLabels();
		}
		else
		{
			this.landingPlatformSection.gameObject.SetActive(false);
			this.roundtripSection.gameObject.SetActive(false);
		}
		this.hexEmptyBG.gameObject.SetActive(entityLayer == EntityLayer.POI);
	}

	private void OnClickChangeDestination()
	{
		if (this.targetSelector.assignable)
		{
			ClusterMapScreen.Instance.ShowInSelectDestinationMode(this.targetSelector);
			AxialI myWorldLocation = this.targetSelector.GetMyWorldLocation();
			AxialI destination = this.targetSelector.GetDestination();
			AxialI randomVisibleAdjacentCellLocation = ClusterGrid.Instance.GetRandomVisibleAdjacentCellLocation(myWorldLocation, destination);
			if (randomVisibleAdjacentCellLocation != AxialI.INVALID)
			{
				ClusterMapScreen.Instance.OnHoverHex(ClusterMapScreen.Instance.GetClusterMapHexAtLocation(randomVisibleAdjacentCellLocation));
			}
		}
		this.Refresh(null);
		if (this.changeDestinationButtonTooltip.isHovering)
		{
			ToolTipScreen.Instance.ClearToolTip(this.changeDestinationButtonTooltip);
			ToolTipScreen.Instance.SetToolTip(this.changeDestinationButtonTooltip);
		}
	}

	private void OnClickClearDestination()
	{
		this.targetSelector.SetDestination(this.targetSelector.GetMyWorldLocation());
	}

	private void OnLaunchPadEntryClick(IListableOption option, object data)
	{
		LaunchPad launchPad = (LaunchPad)option;
		this.targetRocketSelector.SetDestinationPad(launchPad);
	}

	private void PadDropDownEntryRefreshAction(DropDownEntry entry, object targetData)
	{
		LaunchPad launchPad = (LaunchPad)entry.entryData;
		Clustercraft component = this.targetRocketSelector.GetComponent<Clustercraft>();
		if (!(launchPad != null))
		{
			entry.button.isInteractable = true;
			entry.image.sprite = Assets.GetBuildingDef("LaunchPad").GetUISprite("ui", false);
			entry.tooltip.SetSimpleTooltip(UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.DROPDOWN_TOOLTIP_FIRST_AVAILABLE);
			return;
		}
		string text;
		if (component.CanLandAtPad(launchPad, out text) == Clustercraft.PadLandingStatus.CanNeverLand)
		{
			entry.button.isInteractable = false;
			entry.image.sprite = Assets.GetSprite("iconWarning");
			entry.tooltip.SetSimpleTooltip(text);
			return;
		}
		entry.button.isInteractable = true;
		entry.image.sprite = launchPad.GetComponent<Building>().Def.GetUISprite("ui", false);
		entry.tooltip.SetSimpleTooltip(string.Format(UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.DROPDOWN_TOOLTIP_VALID_SITE, launchPad.GetProperName()));
	}

	private int PadDropDownSort(IListableOption a, IListableOption b, object targetData)
	{
		return 0;
	}

	private void OnRepeatClicked()
	{
		this.targetRocketSelector.Repeat = !this.targetRocketSelector.Repeat;
		this.Refresh(null);
		this.RefreshRepeatButtonLabels();
	}

	private void RefreshRepeatButtonLabels()
	{
		this.roundTripInfoLabel.SetText(this.targetRocketSelector.Repeat ? UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.ROUNDTRIP_LABEL_ROUNDTRIP : UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.ROUNDTRIP_LABEL_ONE_WAY);
		this.roundTripButtonLabel.SetText(this.targetRocketSelector.Repeat ? UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.ROUNDTRIP_BUTTON_ONE_WAY : UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.ROUNDTRIP_BUTTON_ROUNDTRIP);
		this.roundtripButtonTooltip.SetSimpleTooltip(this.targetRocketSelector.Repeat ? UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.ROUNDTRIP_BUTTON_TOOLTIP_ONE_WAY : UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.ROUNDTRIP_BUTTON_TOOLTIP_ROUNDTRIP);
	}

	public Image hexEmptyBG;

	public Image destinationImage;

	[Header("Destination selection Section")]
	public RectTransform destinationSection;

	public LocText destinationInfoLabel;

	public KButton changeDestinationButton;

	public ToolTip changeDestinationButtonTooltip;

	public KButton clearDestinationButton;

	[Header("Landing Platform Section")]
	public RectTransform landingPlatformSection;

	public LocText landingPlatformInfoLabel;

	public DropDown launchPadDropDown;

	[Header("Round Trip Section")]
	public RectTransform roundtripSection;

	public LocText roundTripInfoLabel;

	public LocText roundTripButtonLabel;

	public KButton repeatButton;

	public ToolTip roundtripButtonTooltip;

	[Space]
	public ColorStyleSetting defaultButton;

	public ColorStyleSetting highlightButton;

	private int m_refreshHandle = -1;

	private int m_refreshOnCancelHandle = -1;
}
