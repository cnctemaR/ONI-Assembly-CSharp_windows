using System;
using System.Collections.Generic;

public class SchedulerGroup
{
	public Scheduler scheduler { get; private set; }

	public SchedulerGroup(Scheduler scheduler)
	{
		this.scheduler = scheduler;
		this.Reset();
	}

	public void FreeResources()
	{
		if (this.scheduler != null)
		{
			this.scheduler.FreeResources();
		}
		this.scheduler = null;
		if (this.handles != null)
		{
			this.handles.Clear();
		}
		this.handles = null;
	}

	public void Reset()
	{
		foreach (SchedulerHandle schedulerHandle in this.handles)
		{
			schedulerHandle.ClearScheduler();
		}
		this.handles.Clear();
	}

	public void Add(SchedulerHandle handle)
	{
		this.handles.Add(handle);
	}

	private List<SchedulerHandle> handles = new List<SchedulerHandle>();
}
