using System;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class SummonCrewSideScreen : SideScreenContent, ISim1000ms
{
	protected override void OnSpawn()
	{
		this.button.onClick += this.OnButtonPressed;
	}

	public override int GetSideScreenSortOrder()
	{
		return 101;
	}

	public override bool IsValidForTarget(GameObject target)
	{
		RocketModuleCluster component = target.GetComponent<RocketModuleCluster>();
		RocketControlStation component2 = target.GetComponent<RocketControlStation>();
		bool flag = component != null && component.GetComponent<PassengerRocketModule>() != null;
		bool flag2 = component != null && component.GetComponent<RoboPilotModule>() != null;
		if (flag || flag2)
		{
			return true;
		}
		if (component2 != null)
		{
			RocketControlStation.StatesInstance smi = component2.GetSMI<RocketControlStation.StatesInstance>();
			return !smi.sm.IsInFlight(smi) && !smi.sm.IsLaunching(smi);
		}
		return false;
	}

	public override void SetTarget(GameObject target)
	{
		RocketModuleCluster component = target.GetComponent<RocketModuleCluster>();
		if (component != null)
		{
			this.craftModuleInterface = component.CraftInterface;
		}
		else if (target.GetComponent<RocketControlStation>() != null)
		{
			this.craftModuleInterface = target.GetMyWorld().GetComponent<Clustercraft>().ModuleInterface;
		}
		this.craftModuleInterface.Unsubscribe(1512695988, new Action<object>(this.OnRocketModuleCountChanged));
		this.craftModuleInterface.Subscribe(1512695988, new Action<object>(this.OnRocketModuleCountChanged));
		Game.Instance.Unsubscribe(586301400, new Action<object>(this.OnMinionsChangedWorld));
		Game.Instance.Unsubscribe(-1123234494, new Action<object>(this.OnAssignmentGroupChanged));
		Game.Instance.Subscribe(586301400, new Action<object>(this.OnMinionsChangedWorld));
		Game.Instance.Subscribe(-1123234494, new Action<object>(this.OnAssignmentGroupChanged));
		this.Refresh();
	}

	private void OnMinionsChangedWorld(object o)
	{
		this.Refresh();
	}

	public override void ClearTarget()
	{
		this.refreshInUpdate = false;
		if (this.craftModuleInterface != null)
		{
			this.craftModuleInterface.Unsubscribe(1512695988, new Action<object>(this.OnRocketModuleCountChanged));
		}
		base.ClearTarget();
		Game.Instance.Unsubscribe(-1123234494, new Action<object>(this.OnAssignmentGroupChanged));
		Game.Instance.Unsubscribe(586301400, new Action<object>(this.OnMinionsChangedWorld));
		this.craftModuleInterface = null;
	}

	private void OnRocketModuleCountChanged(object o)
	{
		this.Refresh();
	}

	private void OnAssignmentGroupChanged(object o)
	{
		this.Refresh();
	}

	private void OnButtonPressed()
	{
		this.ToggleCrewRequestState();
		this.Refresh();
	}

	private void ToggleCrewRequestState()
	{
		PassengerRocketModule passengerModule = this.craftModuleInterface.GetPassengerModule();
		if (passengerModule != null)
		{
			if (passengerModule.PassengersRequested == PassengerRocketModule.RequestCrewState.Request)
			{
				passengerModule.RequestCrewBoard(PassengerRocketModule.RequestCrewState.Release);
				return;
			}
			passengerModule.RequestCrewBoard(PassengerRocketModule.RequestCrewState.Request);
		}
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
	}

	private void Refresh()
	{
		this.refreshInUpdate = false;
		PassengerRocketModule passengerModule = this.craftModuleInterface.GetPassengerModule();
		global::UnityEngine.Object robotPilotModule = this.craftModuleInterface.GetRobotPilotModule();
		int num = ((passengerModule == null) ? 0 : passengerModule.GetCrewCount());
		bool flag = passengerModule != null;
		bool flag2 = num > 0;
		bool flag3 = robotPilotModule != null;
		global::Tuple<int, int> tuple = null;
		this.button.isInteractable = passengerModule != null && flag2;
		SummonCrewSideScreen.CurrentState currentState;
		if (!flag || !flag2)
		{
			currentState = SummonCrewSideScreen.CurrentState.NoCrewFound;
			if (flag3)
			{
				currentState = SummonCrewSideScreen.CurrentState.NoCrewNeeded;
			}
		}
		else if (passengerModule.PassengersRequested == PassengerRocketModule.RequestCrewState.Release)
		{
			currentState = SummonCrewSideScreen.CurrentState.PublicAccess;
		}
		else
		{
			tuple = passengerModule.GetCrewBoardedFraction();
			if (tuple.first < tuple.second)
			{
				currentState = SummonCrewSideScreen.CurrentState.AwaitingCrew;
			}
			else
			{
				currentState = SummonCrewSideScreen.CurrentState.Ready;
			}
		}
		Sprite sprite = null;
		Color color = this.defaultColor;
		string text = "";
		string text2 = "";
		string text3 = "";
		string text4 = "";
		switch (currentState)
		{
		case SummonCrewSideScreen.CurrentState.NoCrewFound:
			sprite = Assets.GetSprite("rocket_red_icon");
			color = this.noCrewColor;
			text = UI.UISIDESCREENS.SUMMON_CREW_SIDESCREEN.INFO_LABEL_NO_CREW_FOUND;
			text2 = UI.UISIDESCREENS.SUMMON_CREW_SIDESCREEN.INFO_LABEL_TOOLTIP_NO_CREW_FOUND;
			text3 = UI.UISIDESCREENS.SUMMON_CREW_SIDESCREEN.SUMMON_CREW_BUTTON_LABEL;
			text4 = UI.UISIDESCREENS.SUMMON_CREW_SIDESCREEN.SUMMON_CREW_BUTTON_TOOLTIP;
			break;
		case SummonCrewSideScreen.CurrentState.NoCrewNeeded:
			sprite = Assets.GetSprite("ic_checklist");
			color = this.readyColor;
			text = UI.UISIDESCREENS.SUMMON_CREW_SIDESCREEN.INFO_LABEL_NO_CREW_NEEDED;
			text2 = UI.UISIDESCREENS.SUMMON_CREW_SIDESCREEN.INFO_LABEL_TOOLTIP_NO_CREW_NEEDED;
			text3 = UI.UISIDESCREENS.SUMMON_CREW_SIDESCREEN.SUMMON_CREW_BUTTON_LABEL;
			text4 = UI.UISIDESCREENS.SUMMON_CREW_SIDESCREEN.SUMMON_CREW_BUTTON_TOOLTIP;
			break;
		case SummonCrewSideScreen.CurrentState.PublicAccess:
			sprite = Assets.GetSprite("status_item_change_door_control_state");
			text = UI.UISIDESCREENS.SUMMON_CREW_SIDESCREEN.INFO_LABEL_PUBLIC_ACCESS;
			text2 = UI.UISIDESCREENS.SUMMON_CREW_SIDESCREEN.INFO_LABEL_TOOLTIP_PUBLIC_ACCESS;
			text3 = UI.UISIDESCREENS.SUMMON_CREW_SIDESCREEN.SUMMON_CREW_BUTTON_LABEL;
			text4 = UI.UISIDESCREENS.SUMMON_CREW_SIDESCREEN.SUMMON_CREW_BUTTON_TOOLTIP;
			break;
		case SummonCrewSideScreen.CurrentState.AwaitingCrew:
			this.refreshInUpdate = true;
			sprite = Assets.GetSprite("crew_boarded");
			text = GameUtil.SafeStringFormat(UI.UISIDESCREENS.SUMMON_CREW_SIDESCREEN.INFO_LABEL_AWAITING_CREW, new object[]
			{
				GameUtil.GetFormattedInt((float)tuple.first, GameUtil.TimeSlice.None),
				GameUtil.GetFormattedInt((float)tuple.second, GameUtil.TimeSlice.None)
			});
			text2 = UI.UISIDESCREENS.SUMMON_CREW_SIDESCREEN.INFO_LABEL_TOOLTIP_AWAITING_CREW;
			text3 = UI.UISIDESCREENS.SUMMON_CREW_SIDESCREEN.CANCEL_BUTTON_LABEL;
			text4 = UI.UISIDESCREENS.SUMMON_CREW_SIDESCREEN.CANCEL_BUTTON_TOOLTIP;
			break;
		case SummonCrewSideScreen.CurrentState.Ready:
			sprite = Assets.GetSprite("ic_checklist");
			color = this.readyColor;
			text = UI.UISIDESCREENS.SUMMON_CREW_SIDESCREEN.INFO_LABEL_CREW_READY;
			text2 = UI.UISIDESCREENS.SUMMON_CREW_SIDESCREEN.INFO_LABEL_TOOLTIP_CREW_READY;
			text3 = UI.UISIDESCREENS.SUMMON_CREW_SIDESCREEN.CANCEL_BUTTON_LABEL;
			text4 = UI.UISIDESCREENS.SUMMON_CREW_SIDESCREEN.CANCEL_BUTTON_TOOLTIP;
			break;
		}
		this.infoLabel.SetText(text);
		this.infoLabelTooltip.SetSimpleTooltip(text2);
		this.buttonLabel.SetText(text3);
		this.buttonTooltip.SetSimpleTooltip(text4);
		this.image.sprite = sprite;
		this.image.color = color;
	}

	public void Sim1000ms(float dt)
	{
		if (this.refreshInUpdate)
		{
			this.Refresh();
		}
	}

	public const string READY_ICON_NAME = "ic_checklist";

	public const string NOT_APPLICABLE_ICON_NAME = "rocket_red_icon";

	public const string PUBLIC_ACCESS_ICON_NAME = "status_item_change_door_control_state";

	public const string AWAITING_ICON_NAME = "crew_boarded";

	public Image image;

	public LocText infoLabel;

	public ToolTip infoLabelTooltip;

	public KButton button;

	public LocText buttonLabel;

	public ToolTip buttonTooltip;

	private CraftModuleInterface craftModuleInterface;

	private Color noCrewColor = Color.white;

	private Color defaultColor = new Color(0.5568628f, 0.5568628f, 0.5568628f, 1f);

	private Color readyColor = new Color(0f, 0.58431375f, 0.23137255f, 1f);

	private bool refreshInUpdate;

	private enum CurrentState
	{
		NoCrewFound,
		NoCrewNeeded,
		PublicAccess,
		AwaitingCrew,
		Ready
	}
}
