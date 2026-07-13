using System;
using STRINGS;

public interface IEmptyableCargo
{
	bool CanEmptyCargo();

	void EmptyCargo();

	IStateMachineTarget master { get; }

	bool CanAutoDeploy { get; }

	bool AutoDeploy { get; set; }

	bool ChooseDuplicant { get; }

	bool ModuleDeployed { get; }

	MinionIdentity ChosenDuplicant { get; set; }

	bool CanTargetClusterGridEntities
	{
		get
		{
			return false;
		}
	}

	string GetButtonText
	{
		get
		{
			return UI.UISIDESCREENS.MODULEFLIGHTUTILITYSIDESCREEN.DEPLOY_BUTTON;
		}
	}

	string GetButtonToolip
	{
		get
		{
			return UI.UISIDESCREENS.MODULEFLIGHTUTILITYSIDESCREEN.DEPLOY_BUTTON_TOOLTIP;
		}
	}
}
