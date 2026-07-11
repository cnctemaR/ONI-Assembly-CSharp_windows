using System;
using STRINGS;

public class ConditionDestinationReachable : RocketLaunchCondition
{
	public ConditionDestinationReachable(CommandModule module)
	{
		this.commandModule = module;
	}

	public override RocketLaunchCondition GetParentCondition()
	{
		return null;
	}

	public override RocketLaunchCondition.LaunchStatus EvaluateLaunchCondition()
	{
		int id = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this.commandModule.GetComponent<LaunchConditionManager>()).id;
		SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(id);
		if (spacecraftDestination != null && this.CanReachDestination(spacecraftDestination) && spacecraftDestination.GetDestinationType().visitable)
		{
			return RocketLaunchCondition.LaunchStatus.Ready;
		}
		return RocketLaunchCondition.LaunchStatus.Failure;
	}

	public bool CanReachDestination(SpaceDestination destination)
	{
		float rocketMaxDistance = this.commandModule.rocketStats.GetRocketMaxDistance();
		return (float)destination.OneBasedDistance * 10000f <= rocketMaxDistance;
	}

	public SpaceDestination GetDestination()
	{
		int id = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this.commandModule.GetComponent<LaunchConditionManager>()).id;
		return SpacecraftManager.instance.GetSpacecraftDestination(id);
	}

	public override string GetLaunchStatusMessage(bool ready)
	{
		if (ready && this.GetDestination() != null)
		{
			return UI.STARMAP.DESTINATIONSELECTION.REACHABLE;
		}
		if (this.GetDestination() != null)
		{
			return UI.STARMAP.DESTINATIONSELECTION.UNREACHABLE;
		}
		return UI.STARMAP.DESTINATIONSELECTION.NOTSELECTED;
	}

	public override string GetLaunchStatusTooltip(bool ready)
	{
		if (ready && this.GetDestination() != null)
		{
			return UI.STARMAP.DESTINATIONSELECTION_TOOLTIP.REACHABLE;
		}
		if (this.GetDestination() != null)
		{
			return UI.STARMAP.DESTINATIONSELECTION_TOOLTIP.UNREACHABLE;
		}
		return UI.STARMAP.DESTINATIONSELECTION_TOOLTIP.NOTSELECTED;
	}

	private CommandModule commandModule;
}
