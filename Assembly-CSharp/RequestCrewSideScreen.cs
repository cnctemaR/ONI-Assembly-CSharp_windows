using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class RequestCrewSideScreen : SideScreenContent
{
	protected override void OnSpawn()
	{
		this.changeCrewButton.onClick += this.OnChangeCrewButtonPressed;
		this.crewReleaseButton.onClick += this.CrewRelease;
		this.crewRequestButton.onClick += this.CrewRequest;
		this.toggleMap.Add(this.crewReleaseButton, PassengerRocketModule.RequestCrewState.Release);
		this.toggleMap.Add(this.crewRequestButton, PassengerRocketModule.RequestCrewState.Request);
		this.Refresh();
	}

	public override int GetSideScreenSortOrder()
	{
		return 100;
	}

	public override bool IsValidForTarget(GameObject target)
	{
		PassengerRocketModule component = target.GetComponent<PassengerRocketModule>();
		RocketControlStation component2 = target.GetComponent<RocketControlStation>();
		if (component != null)
		{
			return component.GetMyWorld() != null;
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
		if (target.GetComponent<RocketControlStation>() != null)
		{
			this.rocketModule = target.GetMyWorld().GetComponent<Clustercraft>().ModuleInterface.GetPassengerModule();
		}
		else
		{
			this.rocketModule = target.GetComponent<PassengerRocketModule>();
		}
		this.Refresh();
	}

	private void Refresh()
	{
		this.RefreshRequestButtons();
	}

	private void CrewRelease()
	{
		this.rocketModule.RequestCrewBoard(PassengerRocketModule.RequestCrewState.Release);
		this.RefreshRequestButtons();
	}

	private void CrewRequest()
	{
		this.rocketModule.RequestCrewBoard(PassengerRocketModule.RequestCrewState.Request);
		this.RefreshRequestButtons();
	}

	private void RefreshRequestButtons()
	{
		foreach (KeyValuePair<KToggle, PassengerRocketModule.RequestCrewState> keyValuePair in this.toggleMap)
		{
			this.RefreshRequestButton(keyValuePair.Key);
		}
	}

	private void RefreshRequestButton(KToggle button)
	{
		ImageToggleState[] array;
		if (this.toggleMap[button] == this.rocketModule.PassengersRequested)
		{
			button.isOn = true;
			array = button.GetComponentsInChildren<ImageToggleState>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive();
			}
			button.GetComponent<ImageToggleStateThrobber>().enabled = false;
			return;
		}
		button.isOn = false;
		array = button.GetComponentsInChildren<ImageToggleState>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetInactive();
		}
		button.GetComponent<ImageToggleStateThrobber>().enabled = false;
	}

	private void OnChangeCrewButtonPressed()
	{
		if (this.activeChangeCrewSideScreen == null)
		{
			this.activeChangeCrewSideScreen = (AssignmentGroupControllerSideScreen)DetailsScreen.Instance.SetSecondarySideScreen(this.changeCrewSideScreenPrefab, UI.UISIDESCREENS.ASSIGNMENTGROUPCONTROLLER.TITLE);
			this.activeChangeCrewSideScreen.SetTarget(this.rocketModule.gameObject);
			return;
		}
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

	private PassengerRocketModule rocketModule;

	public KToggle crewReleaseButton;

	public KToggle crewRequestButton;

	private Dictionary<KToggle, PassengerRocketModule.RequestCrewState> toggleMap = new Dictionary<KToggle, PassengerRocketModule.RequestCrewState>();

	public KButton changeCrewButton;

	public KScreen changeCrewSideScreenPrefab;

	private AssignmentGroupControllerSideScreen activeChangeCrewSideScreen;
}
