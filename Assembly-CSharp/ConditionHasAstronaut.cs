using System;
using STRINGS;

public class ConditionHasAstronaut : RocketLaunchCondition
{
	public ConditionHasAstronaut(CommandModule module)
	{
		this.module = module;
	}

	public override RocketLaunchCondition GetParentCondition()
	{
		return null;
	}

	public override bool EvaluateLaunchCondition()
	{
		return this.module.GetComponent<MinionStorage>().GetStoredMinionInfo().Count > 0 && this.module.GetComponent<MinionStorage>().GetStoredMinionInfo()[0].serializedMinion != null;
	}

	public override string GetLaunchStatusMessage(bool ready)
	{
		if (ready)
		{
			return UI.STARMAP.LAUNCHCHECKLIST.ASTRONAUT_TITLE;
		}
		return UI.STARMAP.LAUNCHCHECKLIST.ASTRONAUGHT;
	}

	public override string GetLaunchStatusTooltip(bool ready)
	{
		if (ready)
		{
			return UI.STARMAP.LAUNCHCHECKLIST.HASASTRONAUT;
		}
		return UI.STARMAP.LAUNCHCHECKLIST.ASTRONAUGHT;
	}

	private CommandModule module;
}
