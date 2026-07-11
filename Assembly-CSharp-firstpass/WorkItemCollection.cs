using System;
using System.Collections.Generic;

public class WorkItemCollection<WorkItemType, SharedDataType> : IWorkItemCollection where WorkItemType : IWorkItem<SharedDataType>
{
	public int Count
	{
		get
		{
			return this.items.Count;
		}
	}

	public WorkItemType GetWorkItem(int idx)
	{
		return this.items[idx];
	}

	public void Add(WorkItemType work_item)
	{
		this.items.Add(work_item);
	}

	public void InternalDoWorkItem(int work_item_idx)
	{
		WorkItemType workItemType = this.items[work_item_idx];
		workItemType.Run(this.sharedData);
		this.items[work_item_idx] = workItemType;
	}

	public void Reset(SharedDataType shared_data)
	{
		this.sharedData = shared_data;
		this.items.Clear();
	}

	private List<WorkItemType> items = new List<WorkItemType>();

	private SharedDataType sharedData;
}
