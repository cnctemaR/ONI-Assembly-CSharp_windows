using System;
using STRINGS;

public class ConditionPilotOnBoard : ProcessCondition
{
	public ConditionPilotOnBoard(PassengerRocketModule module)
	{
		this.module = module;
	}

	public override ProcessCondition.Status EvaluateCondition()
	{
		if (!this.module.CheckPilotBoarded())
		{
			return ProcessCondition.Status.Failure;
		}
		return ProcessCondition.Status.Ready;
	}

	public override string GetStatusMessage(ProcessCondition.Status status)
	{
		if (status == ProcessCondition.Status.Ready)
		{
			return UI.STARMAP.LAUNCHCHECKLIST.PILOT_BOARDED.READY;
		}
		return UI.STARMAP.LAUNCHCHECKLIST.PILOT_BOARDED.FAILURE;
	}

	public override string GetStatusTooltip(ProcessCondition.Status status)
	{
		if (status == ProcessCondition.Status.Ready)
		{
			return UI.STARMAP.LAUNCHCHECKLIST.PILOT_BOARDED.TOOLTIP.READY;
		}
		return UI.STARMAP.LAUNCHCHECKLIST.PILOT_BOARDED.TOOLTIP.FAILURE;
	}

	public override bool ShowInUI()
	{
		return true;
	}

	private PassengerRocketModule module;
}
