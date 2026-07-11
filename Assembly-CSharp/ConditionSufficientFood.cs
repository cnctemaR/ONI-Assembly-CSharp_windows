using System;
using STRINGS;

public class ConditionSufficientFood : RocketLaunchCondition
{
	public ConditionSufficientFood(CommandModule module)
	{
		this.module = module;
	}

	public override RocketLaunchCondition GetParentCondition()
	{
		return null;
	}

	public override RocketLaunchCondition.LaunchStatus EvaluateLaunchCondition()
	{
		bool flag = this.module.storage.GetAmountAvailable(GameTags.Edible) > 1f;
		return (!flag) ? RocketLaunchCondition.LaunchStatus.Failure : RocketLaunchCondition.LaunchStatus.Ready;
	}

	public override string GetLaunchStatusMessage(bool ready)
	{
		if (ready)
		{
			return UI.STARMAP.HASFOOD.NAME;
		}
		return UI.STARMAP.NOFOOD.NAME;
	}

	public override string GetLaunchStatusTooltip(bool ready)
	{
		if (ready)
		{
			return UI.STARMAP.HASFOOD.TOOLTIP;
		}
		return UI.STARMAP.NOFOOD.TOOLTIP;
	}

	private CommandModule module;
}
