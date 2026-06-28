using System;
using System.Collections.Generic;
using System.Threading;

public class Job<WorkItemType, SharedDataType> : Job where WorkItemType : IWorkItem<SharedDataType>
{
	public Job(List<WorkItemType> work_items, SharedDataType shared_data, int worker_thread_count)
		: base(worker_thread_count)
	{
		this.workItems = work_items;
		this.sharedData = shared_data;
	}

	public override bool DoNextWorkItem()
	{
		int num = Interlocked.Increment(ref this.nextWorkIndex);
		if (num < this.workItems.Count)
		{
			WorkItemType workItemType = this.workItems[num];
			workItemType.Run(this.sharedData);
			return true;
		}
		return false;
	}

	private List<WorkItemType> workItems;

	private int nextWorkIndex = -1;

	private SharedDataType sharedData;
}
