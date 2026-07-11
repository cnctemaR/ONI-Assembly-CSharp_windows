using System;

public abstract class RocketLaunchCondition
{
	public abstract RocketLaunchCondition.LaunchStatus EvaluateLaunchCondition();

	public abstract string GetLaunchStatusMessage(bool ready);

	public abstract string GetLaunchStatusTooltip(bool ready);

	public virtual RocketLaunchCondition GetParentCondition()
	{
		return null;
	}

	public enum LaunchStatus
	{
		Ready,
		Warning,
		Failure
	}
}
