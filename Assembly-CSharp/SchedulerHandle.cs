using System;

public struct SchedulerHandle
{
	public SchedulerHandle(Scheduler scheduler, SchedulerEntry entry)
	{
		this.entry = entry;
		this.scheduler = scheduler;
	}

	public float TimeRemaining
	{
		get
		{
			float num;
			if (!this.IsValid)
			{
				num = -1f;
			}
			else
			{
				num = this.entry.time - this.scheduler.GetTime();
			}
			return num;
		}
	}

	public void FreeResources()
	{
		this.entry.FreeResources();
		this.scheduler = null;
	}

	public void ClearScheduler()
	{
		if (this.scheduler != null)
		{
			this.scheduler.Clear(this);
			this.scheduler = null;
		}
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
