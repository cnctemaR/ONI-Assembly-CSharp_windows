using System;

public class PriorityCommand
{
	public virtual bool CanBegin()
	{
		return true;
	}

	public virtual void Cleanup()
	{
	}

	public virtual void Begin()
	{
	}
}
