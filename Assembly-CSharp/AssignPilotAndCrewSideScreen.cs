using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class AssignPilotAndCrewSideScreen : SideScreenContent
{
	protected override void OnSpawn()
	{
		this.editCrewButton.onClick += this.OnChangeCrewButtonPressed;
	}

	public override int GetSideScreenSortOrder()
	{
		return 102;
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
		Game.Instance.Unsubscribe(-1123234494, new Action<object>(this.OnAssignmentGroupChanged));
		Game.Instance.Subscribe(-1123234494, new Action<object>(this.OnAssignmentGroupChanged));
		this.Refresh();
	}

	public override void ClearTarget()
	{
		if (this.craftModuleInterface != null)
		{
			this.craftModuleInterface.Unsubscribe(1512695988, new Action<object>(this.OnRocketModuleCountChanged));
		}
		base.ClearTarget();
		Game.Instance.Unsubscribe(-1123234494, new Action<object>(this.OnAssignmentGroupChanged));
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

	private void OnChangeCrewButtonPressed()
	{
		if (this.activeChangeCrewSideScreen == null)
		{
			this.activeChangeCrewSideScreen = (AssignmentGroupControllerSideScreen)DetailsScreen.Instance.SetSecondarySideScreen(this.changeCrewSideScreenPrefab, UI.UISIDESCREENS.ASSIGNMENTGROUPCONTROLLER.TITLE);
			this.activeChangeCrewSideScreen.SetTarget(this.craftModuleInterface.GetPassengerModule().gameObject);
			return;
		}
		this.CloseSecondaryScreen();
	}

	private void CloseSecondaryScreen()
	{
		DetailsScreen.Instance.ClearSecondarySideScreen();
		this.activeChangeCrewSideScreen = null;
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (!show)
		{
			DetailsScreen.Instance.ClearSecondarySideScreen();
			this.activeChangeCrewSideScreen = null;
		}
	}

	private void Refresh()
	{
		PassengerRocketModule passengerModule = this.craftModuleInterface.GetPassengerModule();
		GameObject gameObject = ((passengerModule == null) ? null : passengerModule.GetDupePilot());
		bool flag = this.craftModuleInterface.GetRobotPilotModule() != null;
		bool flag2 = gameObject != null;
		bool flag3 = flag && !flag2;
		bool flag4 = flag || flag2;
		bool flag5 = flag && flag2;
		if (passengerModule == null && this.activeChangeCrewSideScreen != null)
		{
			this.CloseSecondaryScreen();
		}
		if (flag5)
		{
			this.copilotImage.sprite = Assets.GetSprite("Dreamicon_robopilot");
		}
		this.copilotImage.gameObject.SetActive(flag5);
		this.editCrewButton.isInteractable = passengerModule != null;
		this.editCrewTooltip.SetSimpleTooltip((passengerModule != null) ? UI.UISIDESCREENS.PILOT_AND_CREW_SIDESCREEN.EDIT_CREW_BUTTON_TOOLTIP : UI.UISIDESCREENS.PILOT_AND_CREW_SIDESCREEN.EDIT_CREW_BUTTON_DISABLED_TOOLTIP);
		Sprite sprite;
		if (!flag4)
		{
			sprite = Assets.GetSprite("dreamIcon_Unknown");
			this.infoLabel.SetText(GameUtil.SafeStringFormat(UI.UISIDESCREENS.PILOT_AND_CREW_SIDESCREEN.INFO_LABEL, new object[] { UI.UISIDESCREENS.PILOT_AND_CREW_SIDESCREEN.NO_ASSIGNED_NAME }));
		}
		else
		{
			sprite = (flag3 ? Assets.GetSprite("Dreamicon_robopilot") : Db.Get().Personalities.Get(gameObject.GetComponent<MinionIdentity>().personalityResourceId).GetMiniIcon());
			if (flag3)
			{
				this.infoLabel.SetText(UI.UISIDESCREENS.PILOT_AND_CREW_SIDESCREEN.INFO_LABEL_ROBOT_ONLY);
			}
			else
			{
				this.infoLabel.SetText(GameUtil.SafeStringFormat(UI.UISIDESCREENS.PILOT_AND_CREW_SIDESCREEN.INFO_LABEL, new object[] { gameObject.GetProperName() }));
			}
		}
		this.pilotImage.sprite = sprite;
	}

	public const string NO_PILOT_SPRITE_NAME = "dreamIcon_Unknown";

	public const string ROBOPILOT_SPRITE_NAME = "Dreamicon_robopilot";

	public LocText infoLabel;

	public ToolTip editCrewTooltip;

	public Image pilotImage;

	public Image copilotImage;

	private Dictionary<KToggle, PassengerRocketModule.RequestCrewState> toggleMap = new Dictionary<KToggle, PassengerRocketModule.RequestCrewState>();

	public KButton editCrewButton;

	public KScreen changeCrewSideScreenPrefab;

	private CraftModuleInterface craftModuleInterface;

	private AssignmentGroupControllerSideScreen activeChangeCrewSideScreen;
}
