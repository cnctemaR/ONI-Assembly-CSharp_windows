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

	public override bool EvaluateLaunchCondition()
	{
		return this.module.storage.GetAmountAvailable(GameTags.Edible) > 1f;
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
