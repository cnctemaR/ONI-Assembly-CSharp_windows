using System;

public struct SchedulerHandle
{
	public SchedulerHandle(Scheduler scheduler, SchedulerEntry entry)
	{
		this.entry = entry;
		this.scheduler = scheduler;
	}

	public void Clear()
	{
		if (this.scheduler == null)
		{
			return;
		}
		this.scheduler.Clear(this);
		this.scheduler = null;
	}

	public bool IsValid
	{
		get
		{
			return this.scheduler != null;
		}
	}

	public SchedulerEntry entry;

	private Scheduler scheduler;
}
