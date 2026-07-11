using System;

public abstract class RocketLaunchCondition
{
	public abstract bool EvaluateLaunchCondition();

	public abstract string GetLaunchStatusMessage(bool ready);

	public abstract string GetLaunchStatusTooltip(bool ready);

	public virtual RocketLaunchCondition GetParentCondition()
	{
		return null;
	}

	private void TryRegister(LaunchConditionManager manager)
	{
		if (manager != null)
		{
			manager.RegisterCondition(this);
		}
	}

	private void CleanUp(LaunchConditionManager manager)
	{
		manager.UnregisterCondition(this);
	}
}
