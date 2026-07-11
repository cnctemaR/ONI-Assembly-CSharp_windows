using System;
using STRINGS;
using UnityEngine;

public class CargoBayIsEmpty : RocketLaunchCondition
{
	public CargoBayIsEmpty(CommandModule module)
	{
		this.commandModule = module;
	}

	public override RocketLaunchCondition GetParentCondition()
	{
		return null;
	}

	public override RocketLaunchCondition.LaunchStatus EvaluateLaunchCondition()
	{
		foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(this.commandModule.GetComponent<AttachableBuilding>()))
		{
			CargoBay component = gameObject.GetComponent<CargoBay>();
			if (component != null && component.storage.MassStored() != 0f)
			{
				return RocketLaunchCondition.LaunchStatus.Failure;
			}
		}
		return RocketLaunchCondition.LaunchStatus.Ready;
	}

	public override string GetLaunchStatusMessage(bool ready)
	{
		if (ready)
		{
			return UI.STARMAP.CARGOEMPTY.NAME;
		}
		return UI.STARMAP.CARGOEMPTY.NAME;
	}

	public override string GetLaunchStatusTooltip(bool ready)
	{
		if (ready)
		{
			return UI.STARMAP.CARGOEMPTY.TOOLTIP;
		}
		return UI.STARMAP.CARGOEMPTY.TOOLTIP;
	}

	private CommandModule commandModule;
}
