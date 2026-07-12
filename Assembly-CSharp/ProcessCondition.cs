using System;

public abstract class ProcessCondition
{
	public abstract ProcessCondition.Status EvaluateCondition();

	public abstract bool ShowInUI();

	public abstract string GetStatusMessage(ProcessCondition.Status status);

	public string GetStatusMessage()
	{
		return this.GetStatusMessage(this.EvaluateCondition());
	}

	public abstract string GetStatusTooltip(ProcessCondition.Status status);

	public string GetStatusTooltip()
	{
		return this.GetStatusTooltip(this.EvaluateCondition());
	}

	public virtual StatusItem GetStatusItem(ProcessCondition.Status status)
	{
		return null;
	}

	public virtual ProcessCondition GetParentCondition()
	{
		return this.parentCondition;
	}

	protected ProcessCondition parentCondition;

	public enum ProcessConditionType
	{
		RocketFlight,
		RocketPrep,
		RocketStorage,
		RocketBoard,
		All
	}

	public enum Status
	{
		Failure,
		Warning,
		Ready
	}
}
