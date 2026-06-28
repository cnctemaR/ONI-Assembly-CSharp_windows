using System;
using System.Collections.Generic;

public class SchedulerGroup
{
	public SchedulerGroup(Scheduler scheduler)
	{
		this.scheduler = scheduler;
		this.Reset();
	}

	public Scheduler scheduler { get; private set; }

	public void Reset()
	{
		if (this.scheduler != null)
		{
			foreach (Guid guid in this.guids)
			{
				if (guid != Guid.Empty)
				{
					this.scheduler.Clear(guid);
				}
			}
			this.guids.Clear();
		}
	}

	public void Add(Guid guid)
	{
		this.guids.Add(guid);
	}

	public void Remove(Guid guid)
	{
		this.guids.Remove(guid);
	}

	private List<Guid> guids = new List<Guid>();
}
